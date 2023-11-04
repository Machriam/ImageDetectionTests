namespace ImageDetectionTests.Client;

using System;

public interface IImageDataHandler
{
    event Func<IList<Guid>, Task>? ImageChanged;

    event Func<Guid, Task>? ImageSelected;

    event Func<Guid, Task>? ReRenderImage;

    Task AddImage(PipelineStep action, List<object> parameters);

    Task AddSourceImage(string image);

    MatImageData GetRenderData(Guid guid);

    Task ImageRendered(MatImageData data);

    Task InvokeImageChanged();

    Task RemoveSelectedImage();

    Task SelectImage(Guid guid);

    Task UpdateImageAt();

    Task Clear();

    Task InvokeSelectedImageChanged();

    void UpdateImageParameter(List<object> parameter);
}

public class ImageDataHandler : IImageDataHandler
{
    private readonly List<MatImageData> _imageData = new();
    private Guid? _selectedImage;

    public event Func<IList<Guid>, Task>? ImageChanged;

    public event Func<Guid, Task>? ImageSelected;

    public event Func<Guid, Task>? ReRenderImage;

    public async Task AddSourceImage(string image)
    {
        _imageData.Clear();
        _imageData.Add(new MatImageData()
        {
            OriginalImage = image,
        });
        await InvokeImageChanged();
    }

    public async Task InvokeSelectedImageChanged()
    {
        if (_selectedImage == null) return;
        var (image, index) = _imageData.WithIndex().First(d => d.Item.Guid == _selectedImage);
        for (var i = index; i < _imageData.Count; i++)
        {
            await (ReRenderImage?.Invoke(_imageData[i].Guid) ?? Task.CompletedTask);
        }
    }

    public async Task InvokeImageChanged()
    {
        await (ImageChanged?.Invoke(_imageData.ConvertAll(d => d.Guid)) ?? Task.CompletedTask);
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

    public void UpdateImageParameter(List<object> parameter)
    {
        if (_selectedImage == null) return;
        _imageData.First(d => d.Guid == _selectedImage).StepParameter = parameter;
    }

    public async Task AddImage(PipelineStep action, List<object> parameters)
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
        await InvokeImageChanged();
        await UpdateFromIndex(index);
    }

    public async Task SelectImage(Guid guid)
    {
        _selectedImage = guid;
        if (_imageData.First(d => !string.IsNullOrEmpty(d.OriginalImage)).Guid == guid) _selectedImage = null;
        await (ImageSelected?.Invoke(_selectedImage ?? Guid.Empty) ?? Task.CompletedTask);
    }

    public async Task RemoveSelectedImage()
    {
        if (_selectedImage == null) return;
        var (image, index) = _imageData.WithIndex().First(d => d.Item.Guid == _selectedImage);
        if (index == 0) { await Clear(); return; }

        _imageData.Remove(image);
        if (index < _imageData.Count) _imageData[index].PreviousImage = _imageData[index - 1].Guid;
        await InvokeImageChanged();
        _selectedImage = null;
        await UpdateFromIndex(index);
    }

    public async Task UpdateFromIndex(int index)
    {
        foreach (var image in _imageData.WithIndex()
            .Where(d => d.Index >= index && d.Index > 0))
        {
            await (ReRenderImage?.Invoke(image.Item.Guid) ?? Task.CompletedTask);
        }
    }

    public async Task Clear()
    {
        _imageData.Clear();
        _selectedImage = null;
        await InvokeImageChanged();
    }

    public Task UpdateImageAt()
    {
        return Task.CompletedTask;
    }
}