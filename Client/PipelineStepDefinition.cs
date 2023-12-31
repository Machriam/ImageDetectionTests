﻿namespace ImageDetectionTests.Client;

using ImageDetectionTests.Client.Extensions;

public static class PipelineStepDefinition
{
    private static int OddTransform(int x)
    {
        return (int)x + (1 - ((int)x % 2));
    }

    public static readonly List<PipelineStep> PossibleSteps = new()
        {
            new PipelineStep()
            {
                Name = "Canny",
                JsName="Canny",
                ParamInfoByIndex = new[]{
                   new ParamInfoCV(){Name="threshold1",ParamType=ParamType.Integer,MaxValue=1000f,MinValue=0f,DefaultValue=100},
                   new ParamInfoCV(){Name="threshold2",ParamType=ParamType.Integer,MaxValue=1000f,MinValue=0f,DefaultValue=300},
                }
                .WithIndex().ToDictionary(x=>x.Index,x=>x.Item)
            },
            new PipelineStep()
            {
                Name = "Median Blur",
                JsName="MedianBlur",
                ParamInfoByIndex = new[]{
                   new ParamInfoCV(){Name="ksize",ParamType=ParamType.Integer,MaxValue=101,MinValue=3,DefaultValue=5,
                       Transform=x=>OddTransform((int)x)},
                }
                .WithIndex().ToDictionary(x=>x.Index,x=>x.Item)
            },
            new PipelineStep()
            {
                Name = "Equalize Grayscale Hist",
                JsName="EqualizeGrayHist"
            },
            new PipelineStep()
            {
                Name = "Equalize Color Hist",
                JsName="EqualizeColorHist"
            },
            new PipelineStep()
            {
                Name="Gaussian Blur",
                JsName="GaussianBlur",
                ParamInfoByIndex = new[]
                {
                    new ParamInfoCV(){Name="Sigma X",MinValue=0.1f,MaxValue=100f,DefaultValue=0.5,Step=0.1f,ParamType=ParamType.Double},
                    new ParamInfoCV(){Name="Sigma Y",MinValue=0f,MaxValue=100f,DefaultValue=0.5,Step=0.1f,ParamType=ParamType.Double},
                }
                .WithIndex().ToDictionary(x=>x.Index,x=>x.Item)
            },
            new PipelineStep()
            {
                Name = "Invert",
                JsName="Invert"
            },
            new PipelineStep()
            {
                Name = "Fourier Transform",
                JsName="FourierTransform",
            },
            new PipelineStep()
            {
                Name = "Power-Law (Gamma)",
                JsName="PowerLaw",
                ParamInfoByIndex = new[]
                {
                    new ParamInfoCV(){MinValue=0,MaxValue=10,DefaultValue=1d,Step=0.1f,Name="Alpha",ParamType=ParamType.Double},
                    new ParamInfoCV(){MinValue=-255,MaxValue=255,DefaultValue=0d,Step=1f,Name="Beta",ParamType=ParamType.Double}
                }.WithIndex().ToDictionary(x=>x.Index,x=>x.Item)
            },
            new PipelineStep()
            {
                Name="Kernel Spatial Filtering",
                JsName="KernelFiltering",
                ParamInfoByIndex = new[]
                {
                    new ParamInfoCV(){ParamType=ParamType.Kernel,DefaultValue=new KernelInfo().Kernel, Kernels=[new KernelInfo()],Name="Kernel Selection" },
                    new ParamInfoCV(){ParamType=ParamType.Boolean,DefaultValue=false,Name="Grayscale" },
                    new ParamInfoCV(){ParamType=ParamType.Boolean,DefaultValue=true,Name="Normalize" },
                }.WithIndex().ToDictionary(x=>x.Index,x=>x.Item)
            },
                        new PipelineStep()
            {
                Name="Erode",
                JsName="Erode",
                ParamInfoByIndex = new[]
                {
                    new ParamInfoCV(){ParamType=ParamType.Kernel,DefaultValue="1,1,1\n1,1,1\n1,1,1", Kernels=[new KernelInfo()],Name="Kernel Selection" },
                    new ParamInfoCV(){ParamType=ParamType.Integer,MinValue=1,MaxValue=10000,DefaultValue=1,Name="Iterations" },
                }.WithIndex().ToDictionary(x=>x.Index,x=>x.Item)
            },
                                                new PipelineStep()
            {
                Name="Dilate",
                JsName="Dilate",
                ParamInfoByIndex = new[]
                {
                    new ParamInfoCV(){ParamType=ParamType.Kernel,DefaultValue="1,1,1\n1,1,1\n1,1,1", Kernels=[new KernelInfo()],Name="Kernel Selection" },
                    new ParamInfoCV(){ParamType=ParamType.Integer,MinValue=1,MaxValue=10000,DefaultValue=1,Name="Iterations" },
                }.WithIndex().ToDictionary(x=>x.Index,x=>x.Item)
            },
            new PipelineStep()
            {
                Name = "Threshold",
                JsName="Threshold",
                ParamInfoByIndex = new[]
                {
                    new ParamInfoCV(){MinValue=0,MaxValue=255,DefaultValue=50d,Name="Threshold",ParamType=ParamType.Double,},
                    new ParamInfoCV(){MinValue=0,MaxValue=255,DefaultValue=50d,Name="MaxValue",ParamType=ParamType.Double,},
                }.WithIndex().ToDictionary(x=>x.Index,x=>x.Item)
            },
                        new PipelineStep()
            {
                Name = "Threshold Gauss",
                JsName="AdaptiveThreshold_Gaussian",
                ParamInfoByIndex = new[]
                {
                    new ParamInfoCV(){MinValue=0,MaxValue=255,DefaultValue=255,Name="Max Value",ParamType=ParamType.Integer,},
                    new ParamInfoCV(){MinValue=0,MaxValue=255,DefaultValue=3,Name="Block Size",ParamType=ParamType.Integer,Transform=x=>OddTransform((int)x)},
                    new ParamInfoCV(){MinValue=-10,MaxValue=10,Step=0.1f,DefaultValue=0,Name="Constant",ParamType=ParamType.Double,},
                    new ParamInfoCV(){DefaultValue=false,Name="Inverted",ParamType=ParamType.Boolean,},
                }.WithIndex().ToDictionary(x=>x.Index,x=>x.Item)
            },
                                                new PipelineStep()
            {
                Name = "Threshold Mean",
                JsName="AdaptiveThreshold_Mean",
                ParamInfoByIndex = new[]
                {
                    new ParamInfoCV(){MinValue=0,MaxValue=255,DefaultValue=255,Name="Max Value",ParamType=ParamType.Integer,},
                    new ParamInfoCV(){MinValue=0,MaxValue=255,DefaultValue=3,Name="Block Size",ParamType=ParamType.Integer,Transform=x=>OddTransform((int)x)},
                    new ParamInfoCV(){MinValue=-10,MaxValue=10,Step=0.1f,DefaultValue=0,Name="Constant",ParamType=ParamType.Double,},
                    new ParamInfoCV(){DefaultValue=false,Name="Inverted",ParamType=ParamType.Boolean,},
                }.WithIndex().ToDictionary(x=>x.Index,x=>x.Item)
            },
};
}