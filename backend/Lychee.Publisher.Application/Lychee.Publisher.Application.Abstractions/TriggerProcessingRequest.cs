namespace Lychee.Publisher.Application.Abstractions;

public sealed record TriggerProcessingRequest(bool AutoCropFace = false, int Crf = 23, string Codec = "libx264", string Format = "mp4");
