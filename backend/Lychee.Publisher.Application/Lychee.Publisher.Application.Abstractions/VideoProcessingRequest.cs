using System;

namespace Lychee.Publisher.Application.Abstractions;

public sealed record VideoProcessingRequest(Guid VideoId, string InputUri, bool BurnSubtitles, bool AddWatermark, bool AutoCropFace = false, int Crf = 23, string Codec = "libx264", string Format = "mp4");
