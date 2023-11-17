namespace ImageDetectionTests.Client;

using ImageDetectionTests.Client.Extensions;
using OpenCvSharp;
using System;

public static class PipelineStepDefinition
{
    private static int IntConverter(object value, ParamInfoCV info, Func<int, int>? transform = null)
    {
        var success = int.TryParse(value.ToString(), out var result);
        if (success)
        {
            result = Math.Max(Math.Min(result, info.MaxValue), info.MinValue);
            return transform == null ? result : transform(result);
        }
        return (int)info.DefaultValue;
    }

    public static readonly List<PipelineStep> PossibleSteps = new()
        {
            new PipelineStep()
            {
                Name = "Canny",
                Action = (src, dest, p) =>  Cv2.Canny(src, dest, (int)p[0], (int)p[1]),
                ParamInfoByIndex = new[]{
                   new ParamInfoCV(){Name="threshold1",ParamType=ParamType.Integer,MaxValue=1000,MinValue=0,DefaultValue=100},
                   new ParamInfoCV(){Name="threshold2",ParamType=ParamType.Integer,MaxValue=1000,MinValue=0,DefaultValue=300},
                }
                .Select(x=>{x.ConvertTextToParam=a=>IntConverter(a,x); return x; })
                .WithIndex().ToDictionary(x=>x.Index,x=>x.Item)
            },
            new PipelineStep()
            {
                Name = "Median Blur",
                Action = (src, dest, p) => Cv2.MedianBlur(src, dest, (int)p[0]),
                ParamInfoByIndex = new[]{
                   new ParamInfoCV(){Name="ksize",ParamType=ParamType.Integer,MaxValue=101,MinValue=3,DefaultValue=5},
                }
                .Select(x=>{x.ConvertTextToParam=a=>IntConverter(a,x)+(1-(IntConverter(a,x)%2)); return x; })
                .WithIndex().ToDictionary(x=>x.Index,x=>x.Item)
            },
            new PipelineStep()
            {
                Name = "Equalize Grayscale Hist",
                Action = (src, dest, p) => {
                    Cv2.CvtColor(src,src,ColorConversionCodes.BGRA2GRAY);
                    Cv2.EqualizeHist(src, dest); },
            },
            new PipelineStep()
            {
                Name="Gaussian Blur",
                Action = (src, dest, p) =>
                {
                    Cv2.GaussianBlur(src,dest,Size.Zero,(double)p[0],(double)p[1]);
                },
                ParamInfoByIndex = new[]
                {
                    new ParamInfoCV(){Name="Sigma X",MinValue=0,MaxValue=9999,DefaultValue=0.5,ParamType=ParamType.Double},
                    new ParamInfoCV(){Name="Sigma Y",MinValue=0,MaxValue=9999,DefaultValue=0.5,ParamType=ParamType.Double},
                }.WithIndex().ToDictionary(x=>x.Index,x=>x.Item)
            },
            new PipelineStep()
            {
                Name = "Invert",
                Action = (src, dest, p) => {
                    Cv2.Invert(src,dest);
                }
            },

            new PipelineStep()
            {
                Name = "Log",
                Action = (src, dest, p) => {
                    Cv2.Log(src,dest);
                }
            },
            new PipelineStep()
            {
                Name = "Fourier Transform",
                Action = (src, dest, p) => {
                    Cv2.Dft(src,dest);
                }
            },
            new PipelineStep()
            {
                Name = "Power-Law",
                Action = (src, dest, p) => {
                    Cv2.Pow(src,(double)p[0],dest);
                },
                ParamInfoByIndex = new[]
                {
                    new ParamInfoCV(){MinValue=0,MaxValue=10,DefaultValue=2.5d,Name="Power",ParamType=ParamType.Double}
                }.WithIndex().ToDictionary(x=>x.Index,x=>x.Item)
            },
            new PipelineStep()
            {
                Name = "Threshold",
                Action = (src, dest, p) => {
                    Cv2.Threshold(src,dest,(double)p[0],(double)p[1],(ThresholdTypes)p[2]);
                },
                ParamInfoByIndex = new[]
                {
                    new ParamInfoCV(){MinValue=0,MaxValue=10,DefaultValue=2.5d,Name="Threshold",ParamType=ParamType.Double,},
                    new ParamInfoCV(){MinValue=0,MaxValue=10,DefaultValue=2.5d,Name="MaxValue",ParamType=ParamType.Double,},
                    new ParamInfoCV(){MinValue=0,MaxValue=10,DefaultValue=2.5d,Name="Type",ParamType=ParamType.Double,},
                }.WithIndex().ToDictionary(x=>x.Index,x=>x.Item)
            },
};
}