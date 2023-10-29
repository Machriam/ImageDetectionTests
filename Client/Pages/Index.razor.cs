using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using OpenCvSharp;

namespace ImageDetectionTests.Client.Pages
{
    public partial class Index
    {
        public int Zoom { get; set; } = 100;
        private string? _image;
        private readonly List<MatImageData> _images = new();

        public void ZoomChanged(ChangeEventArgs args)
        {
            var newZoom = args.Value?.ToString() ?? "100";
            Zoom = (int)float.Parse(newZoom);
        }

        public async Task OnFileChanged(InputFileChangeEventArgs args)
        {
            using var memoryStream = new MemoryStream();
            await args.File.OpenReadStream(1024 * 1024 * 1024).CopyToAsync(memoryStream);
            _image = "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray());
        }

        public void AddMatImageData(MatImageData data)
        {
            _images.Add(data);
        }

        public void AddPipelineStep(Action<Mat, MatImageData> action)
        {
            _images[^1].PipelineAction = dest => action.Invoke(dest, _images[^1]);
        }
    }
}