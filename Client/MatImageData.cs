using OpenCvSharp;
using System.Runtime.InteropServices;

namespace ImageDetectionTests.Client;

public class MatImageData
{
    public byte[] RGBABytes { get; set; } = System.Array.Empty<byte>();
    public int Width { get; set; }
    public int Height { get; set; }

    public Mat CreateMatFromRGBA()
    {
        var mat = new Mat();
        mat.Create(new Size(Width, Height), MatType.CV_8UC4);
        Marshal.Copy(RGBABytes, 0, mat.DataStart, RGBABytes.Length);
        return mat;
    }
}