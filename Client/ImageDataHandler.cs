namespace ImageDetectionTests.Client;

using OpenCvSharp;
using System;

public interface IImageDataHandler
{
    event Action<IList<Guid>>? ImageChanged;

    void AddImage(Action<Mat, MatImageData> action);

    void AddSourceImage(string image);

    void RemoveImageAt();

    void UpdateImageAt();

    MatImageData GetRenderData(Guid guid);

    Task ImageRendered(MatImageData data);
}

public class ImageDataHandler : IImageDataHandler
{
    private readonly List<MatImageData> _imageData = new();
    private Guid? _selectedImage;

    public event Action<IList<Guid>>? ImageChanged;

    public void AddSourceImage(string image)
    {
        _imageData.Clear();
        _imageData.Add(new MatImageData()
        {
            OriginalImage = image,
        });
        InvokeImageChanged();
    }

    public void InvokeImageChanged()
    {
        ImageChanged?.Invoke(_imageData.ConvertAll(d => d.Guid));
    }

    public MatImageData GetRenderData(Guid guid)
    {
        return _imageData.First(d => d.Guid == guid);
    }

    public Task ImageRendered(MatImageData data)
    {
        var entry = _imageData.First(d => d.Guid == data.Guid);
        entry.RGBABytes = data.RGBABytes;
        entry.Height = data.Height;
        entry.Width = data.Width;
        return Task.CompletedTask;
    }

    public void AddImage(Action<Mat, MatImageData> action)
    {
        var (previousData, index) = _selectedImage == null ?
            (_imageData.Last(), _imageData.Count - 1) :
            _imageData.WithIndex().First(d => d.Item.Guid == _selectedImage);
        _imageData.Insert(index + 1, new MatImageData()
        {
            PipelineAction = dest => action.Invoke(dest, previousData),
        });
        InvokeImageChanged();
    }

    public void RemoveImageAt()
    {
    }

    public void UpdateImageAt()
    {
    }
}