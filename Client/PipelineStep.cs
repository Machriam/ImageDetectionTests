namespace ImageDetectionTests.Client;

using ImageDetectionTests.Client.Core;
using System;

public enum ParamType
{
    Integer,
    Double
}

public struct PipelineStep
{
    public PipelineStep()
    {
    }

    public string Name { get; init; } = "";
    public Func<MatImageData, IOpenCvInterop, object[], Task> Action { get; init; } = default!;
    public Dictionary<int, ParamInfoCV> ParamInfoByIndex { get; init; } = new();
}

public struct ParamInfoCV
{
    public ParamInfoCV()
    {
    }

    public ParamType ParamType { get; init; }
    public string Name { get; init; } = "";
    public Func<string, object> ConvertTextToParam { get; set; } = a => a;
    public int MaxValue { get; init; }
    public int MinValue { get; init; }
    public object DefaultValue { get; init; } = new();
    public object[] Values { get; init; } = Array.Empty<object>();
}