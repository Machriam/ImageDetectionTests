namespace ImageDetectionTests.Client;

using OpenCvSharp;
using System;

public class ImageDataHandler
{
    public event Action<Guid> ImageChanged;

    public void AddImage()
    {
    }

    public void RemoveImageAt()
    {
    }

    public void UpdateImageAt()
    {
    }

    protected virtual void OnImageChanged(Guid e)
    {
        ImageChanged?.Invoke(e);
    }
}

public class PipelineStep
{
    public string Name { get; set; } = "";
    public Action<Mat, Mat, object[]> Action { get; set; } = default!;
    public Dictionary<(int Position, string Name), TypeCode> TypeDictionary { get; set; } = new();
}