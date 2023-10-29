namespace ImageDetectionTests.Client;

using OpenCvSharp;
using System;

public interface IImageDataHandler
{
    event Action<IList<Guid>>? ImageChanged;

    event Action<Guid>? ImageSelected;

    void AddImage(Action<Mat, MatImageData> action);

    void AddSourceImage(string image);

    MatImageData GetRenderData(Guid guid);

    Task ImageRendered(MatImageData data);

    void InvokeImageChanged();

    void RemoveSelectedImage();

    void SelectImage(Guid guid);

    void UpdateImageAt();

    void Clear();
}

public class ImageDataHandler : IImageDataHandler
{
    private readonly List<MatImageData> _imageData = new();
    private Guid? _selectedImage;

    public event Action<IList<Guid>>? ImageChanged;

    public event Action<Guid>? ImageSelected;

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
            BaseAction = action,
            PipelineAction = dest => action.Invoke(dest, previousData),
        });
        InvokeImageChanged();
    }

    public void SelectImage(Guid guid)
    {
        _selectedImage = guid;
        ImageSelected?.Invoke(guid);
    }

    public void RemoveSelectedImage()
    {
        if (_selectedImage == null) return;
        var (image, index) = _imageData.WithIndex().First(d => d.Item.Guid == _selectedImage);
        if (!string.IsNullOrEmpty(image.OriginalImage))
        {
            Clear();
            return;
        }
        _imageData.Remove(image);
        if (_imageData.Count > index && _imageData[index].BaseAction != null)
            _imageData[index].PipelineAction = dest => _imageData[index].BaseAction!(dest, _imageData[index - 1]);
        InvokeImageChanged();
        _selectedImage = null;
    }

    public void Clear()
    {
        _imageData.Clear();
        _selectedImage = null;
        InvokeImageChanged();
    }

    public void UpdateImageAt()
    {
    }
}