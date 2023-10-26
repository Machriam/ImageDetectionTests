using global::Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using OpenCvSharp;
using SpawnDev.BlazorJS.JSObjects;

namespace ImageDetectionTests.Client.Pages
{
    public partial class Index
    {
        private ElementReference canvasSrcRef;
        private ElementReference canvasDestRef;

        public async Task OnFileChanged(InputFileChangeEventArgs args)
        {
            using var memoryStream = new MemoryStream();
            await args.File.OpenReadStream(1024 * 1024 * 1024).CopyToAsync(memoryStream);
            var image = "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray());
            await UpdateImage(image);
        }

        protected async Task UpdateImage(string image)
        {
            using var canvasSrcEl = new HTMLCanvasElement(canvasSrcRef);
            using var canvasSrcCtx = canvasSrcEl.Get2DContext();
            using var canvasDestEl = new HTMLCanvasElement(canvasDestRef);
            using var canvasDestCtx = canvasDestEl.Get2DContext();
            using var src = new Mat();
            await src.LoadImageURL(image);
            src.DrawOnCanvas(canvasSrcCtx, true);
            using var dst = new Mat();
            Cv2.Canny(src, dst, 50, 200);
            dst.DrawOnCanvas(canvasDestCtx, true);
        }
    }
}