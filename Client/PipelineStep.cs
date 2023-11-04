namespace ImageDetectionTests.Client;

using OpenCvSharp;
using System;

public enum ParamType
{
    Integer
}

public class PipelineStep
{
    public string Name { get; set; } = "";
    public Action<Mat, Mat, object[]> Action { get; set; } = default!;
    public Dictionary<(int Position, string Name), ParamType> TypeDictionary { get; set; } = new();
}