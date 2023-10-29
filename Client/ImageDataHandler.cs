namespace ImageDetectionTests.Client;

using System;

public interface IImageDataHandler
{
    event Action<IList<Guid>>? ImageChanged;

    event Action<Guid>? ImageSelected;

    void AddImage(PipelineStep action, List<object> parameters);

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
        var (entry, index) = _imageData.WithIndex().First(d => d.Item.Guid == data.Guid);
        entry.RGBABytes = data.RGBABytes;
        entry.Height = data.Height;
        entry.Width = data.Width;
        if (index != 0) entry.PreviousImage = _imageData[index - 1].Guid;
        return Task.CompletedTask;
    }

    public void AddImage(PipelineStep action, List<object> parameters)
    {
        var (previousData, index) = _selectedImage == null ?
            (_imageData.Last(), _imageData.Count - 1) :
            _imageData.WithIndex().First(d => d.Item.Guid == _selectedImage);
        _imageData.Insert(index + 1, new MatImageData()
        {
            Step = action,
            StepParameter = parameters,
            PreviousImage = previousData.Guid
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
        if (index == 0) { Clear(); return; }

        _imageData.Remove(image);
        if (index < _imageData.Count) _imageData[index].PreviousImage = _imageData[index - 1].Guid;
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