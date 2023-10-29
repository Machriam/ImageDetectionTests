using OpenCvSharp;
using System.Runtime.InteropServices;

namespace ImageDetectionTests.Client;

public class MatImageData
{
    public string OriginalImage { get; set; } = "";
    public Guid Guid { get; init; } = Guid.NewGuid();
    public Guid? PreviousImage { get; set; }
    public PipelineStep Step { get; set; } = new();
    public List<object> StepParameter { get; set; } = new();
    public byte[] RGBABytes { get; set; } = System.Array.Empty<byte>();
    public int Width { get; set; }
    public int Height { get; set; }

    public Action<Mat> GetAction(IImageDataHandler dataHandler)
    {
        if (PreviousImage == null) throw new Exception("No Previous Image found");
        var data = dataHandler.GetRenderData(PreviousImage.Value);
        return dest =>
        {
            using var source = data.CreateMatFromRGBA();
            Step.Action(source, dest, StepParameter.ToArray());
        };
    }

    public Mat CreateMatFromRGBA()
    {
        var mat = new Mat();
        mat.Create(new Size(Width, Height), MatType.CV_8UC4);
        Marshal.Copy(RGBABytes, 0, mat.DataStart, RGBABytes.Length);
        return mat;
    }
}