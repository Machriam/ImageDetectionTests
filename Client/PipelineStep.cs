namespace ImageDetectionTests.Client;

using ImageDetectionTests.Client.Extensions;
using System;

public enum ParamType
{
    Integer,
    Double,
    Kernel,
    Boolean
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
                success = int.TryParse(value.Replace(",", "."), out var intValue);
                if (success)
                {
                    intValue = Math.Max(Math.Min(intValue, (int)info.MaxValue), (int)info.MinValue);
                    return info.Transform(intValue);
                }
                return (int)info.DefaultValue;

            case ParamType.Double:
                success = double.TryParse(value.Replace(",", "."), out var doubleValue);
                if (success)
                {
                    doubleValue = Math.Max(Math.Min(doubleValue, info.MaxValue), info.MinValue);
                    return info.Transform(doubleValue);
                }
                return (double)info.DefaultValue;

            case ParamType.Kernel:
                return value.ConvertToMatrix();

            case ParamType.Boolean:
                return bool.Parse(value);
        }
        return value;
    }
}

public class KernelInfo
{
    public string Name { get; set; } = "Identity";
    public string Kernel { get; set; } = "0,0,0\n0,1,0\n0,0,0";
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
    public List<KernelInfo> Kernels { get; set; } = new();
}