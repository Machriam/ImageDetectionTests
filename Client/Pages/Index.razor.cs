using Microsoft.AspNetCore.Components.Forms;
using OpenCvSharp;

namespace ImageDetectionTests.Client.Pages
{
    public partial class Index
    {
        private string? _image;
        private readonly List<MatImageData> _images = new();

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

        public void AddPipelineStep()
        {
            _images[^1].PipelineAction = dest =>
            {
                using var source = _images[^1].CreateMatFromRGBA();
                Cv2.Canny(source, dest, 50, 200);
            };
        }
    }
}