using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Lychee.Publisher.Infrastructure.Services;

public sealed class OpenAiService(HttpClient httpClient, IOptions<OpenAiOptions> options)
{
	private sealed class ChatCompletionResponse
	{
		[JsonPropertyName("choices")]
		public List<Choice>? Choices { get; set; }
	}

	private sealed class Choice
	{
		[JsonPropertyName("message")]
		public MessageDetail? Message { get; set; }
	}

	private sealed class MessageDetail
	{
		[JsonPropertyName("content")]
		public string? Content { get; set; }
	}

	private readonly OpenAiOptions _options = GetEffectiveOptions(options.Value);

	private static OpenAiOptions GetEffectiveOptions(OpenAiOptions configured)
	{
		string environmentVariable = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
		if (!string.IsNullOrWhiteSpace(environmentVariable) && string.IsNullOrWhiteSpace(configured.ApiKey))
		{
			configured.ApiKey = environmentVariable;
		}
		return configured;
	}

	public async Task<string> TranscribeAudioAsync(string audioFilePath, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(_options.ApiKey))
		{
			return "Mock transcript: This is a sample video content containing valuable startup advice and marketing tips.";
		}
		using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/audio/transcriptions");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
		MultipartFormDataContent content = new MultipartFormDataContent();
		FileStream fileStream = File.OpenRead(audioFilePath);
		StreamContent fileContent = new StreamContent(fileStream);
		fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("audio/mpeg");
		content.Add(fileContent, "file", Path.GetFileName(audioFilePath));
		content.Add(new StringContent("whisper-1"), "model");
		content.Add(new StringContent("verbose_json"), "response_format");
		request.Content = content;
		HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
		response.EnsureSuccessStatusCode();
		using JsonDocument doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
		return doc.RootElement.GetProperty("text").GetString() ?? string.Empty;
	}

	public async Task<IReadOnlyCollection<ShortSegmentCandidate>> IdentifySegmentsAsync(string transcriptText, CancellationToken cancellationToken)
	{
		if (string.IsNullOrWhiteSpace(_options.ApiKey))
		{
			return new global::_003C_003Ez__ReadOnlyArray<ShortSegmentCandidate>(new ShortSegmentCandidate[2]
			{
				new ShortSegmentCandidate(0.0, 15.0, 0.88m, "Start with 'Why'", "Learn the core value of purpose in building startups.", "#startups #advice #motivation"),
				new ShortSegmentCandidate(20.0, 45.0, 0.92m, "The Scale Trick", "A simple hack to scale your early traction.", "#startup #traction #hacks")
			});
		}
		using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");
		request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);
		string systemPrompt = "You are an expert AI video editor. Analyze the transcript. Identify the most engaging, coherent parts (15 to 60 seconds long) that can be converted into standalone viral short clips (vertical format). Rate each candidate with a viralityScore between 0.0 and 1.0. For each candidate, provide start time, end time, a catchy title, description, and suggested hashtags. Respond ONLY with a JSON object containing a property 'clips' which is an array of candidates. Schema: { \"clips\": [ { \"startTimeSeconds\": 12.5, \"endTimeSeconds\": 45.0, \"viralityScore\": 0.85, \"title\": \"Title\", \"description\": \"Description\", \"hashtags\": \"#hashtag1 #hashtag2\" } ] }";
		var payload = new
		{
			model = _options.Model,
			response_format = new
			{
				type = "json_object"
			},
			messages = new[]
			{
				new
				{
					role = "system",
					content = systemPrompt
				},
				new
				{
					role = "user",
					content = "Here is the video transcript:\n\n" + transcriptText
				}
			}
		};
		request.Content = JsonContent.Create(payload);
		HttpResponseMessage response = await httpClient.SendAsync(request, cancellationToken);
		response.EnsureSuccessStatusCode();
		string jsonText = (await response.Content.ReadFromJsonAsync<ChatCompletionResponse>(cancellationToken))?.Choices?.FirstOrDefault()?.Message?.Content;
		if (string.IsNullOrWhiteSpace(jsonText))
		{
			return Array.Empty<ShortSegmentCandidate>();
		}
		using JsonDocument doc = JsonDocument.Parse(jsonText);
		return JsonSerializer.Deserialize<List<ShortSegmentCandidate>>(doc.RootElement.GetProperty("clips").GetRawText()) ?? new List<ShortSegmentCandidate>();
	}
}
