using Microsoft.AspNetCore.Components.Forms;
using OpenCvSharp;

namespace ImageDetectionTests.Client.Pages
{
    public partial class Index
    {
        private string? _image;
        private MatImageData? _currentImageData;
        private Action<Mat>? _nextAction;

        public async Task OnFileChanged(InputFileChangeEventArgs args)
        {
            using var memoryStream = new MemoryStream();
            await args.File.OpenReadStream(1024 * 1024 * 1024).CopyToAsync(memoryStream);
            _image = "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray());
        }

        public void AddPipelineStep()
        {
            if (_currentImageData == null) return;
            _nextAction = dest =>
            {
                using var source = _currentImageData.CreateMatFromRGBA();
                Cv2.Canny(source, dest, 50, 200);
            };
        }
    }
}