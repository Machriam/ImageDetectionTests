using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using SpawnDev.BlazorJS;

namespace ImageDetectionTests.Client.Pages
{
    public partial class Index : IDisposable
    {
        [Inject] private IImageDataHandler ImageDataHandler { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        public int Zoom { get; set; } = 100;
        private string? _image;
        private readonly List<Guid> _images = new();

        protected override void OnInitialized()
        {
            ImageDataHandler.ImageChanged += ImageDataHandler_ImageChanged;
            base.OnInitialized();
        }

        private async Task ImageDataHandler_ImageChanged(IList<Guid> obj)
        {
            _images.Clear();
            _images.AddRange(obj);
            await Task.Delay(1);
            StateHasChanged();
        }

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
            await JSRuntime.InvokeVoidAsync("openCvTest", _image);
        }

        public void Dispose()
        {
            ImageDataHandler.ImageChanged -= ImageDataHandler_ImageChanged;
        }
    }
}