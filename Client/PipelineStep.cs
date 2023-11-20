namespace ImageDetectionTests.Client;

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
    public string JsName { get; init; } = "";
    public Dictionary<int, ParamInfoCV> ParamInfoByIndex { get; init; } = new();
}

public static class ParamInfoCVExtensions
{
    public static object ConvertTextToParam(this ParamInfoCV info, string value)
    {
        bool success;
        switch (info.ParamType)
        {
            case ParamType.Integer:
                success = int.TryParse(value, out var intValue);
                if (success)
                {
                    intValue = Math.Max(Math.Min(intValue, (int)info.MaxValue), (int)info.MinValue);
                    return info.Transform(intValue);
                }
                return (int)info.DefaultValue;

            case ParamType.Double:
                success = double.TryParse(value, out var doubleValue);
                if (success)
                {
                    doubleValue = Math.Max(Math.Min(doubleValue, info.MaxValue), info.MinValue);
                    return info.Transform(doubleValue);
                }
                return (double)info.DefaultValue;
        }
        return value;
    }
}

public struct ParamInfoCV
{
    public ParamInfoCV()
    {
    }

    public ParamType ParamType { get; init; }
    public string Name { get; init; } = "";
    public Func<object, object> Transform { get; set; } = a => a;
    public float MaxValue { get; init; }
    public float MinValue { get; init; }
    public object DefaultValue { get; init; } = new();
    public float Step { get; set; } = 1f;
    public object[] Values { get; init; } = Array.Empty<object>();
}