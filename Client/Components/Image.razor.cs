using ImageDetectionTests.Client.Core;
using Microsoft.AspNetCore.Components;

namespace ImageDetectionTests.Client.Components
{
    public partial class Image
    {
        [Inject] private IImageDataHandler ImageDataHandler { get; set; } = default!;
        [Inject] private IOpenCvInterop CvInterop { get; set; } = default!;
        [Parameter] public Guid Guid { get; set; }
        [Parameter] public int Zoom { get; set; } = 100;
        private bool _selected;
        private bool _eventsAttached;

        public void SelectImage()
        {
            ImageDataHandler.SelectImage(Guid);
        }

        public void AttachEvents()
        {
            ImageDataHandler.ImageSelected += OnImageSelected;
            ImageDataHandler.ReRenderImage += OnReRenderImage;
            ImageDataHandler.ImageRemoved += OnImageRemoved;
            _eventsAttached = true;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_eventsAttached) AttachEvents();
            if (!firstRender) return;
            var data = ImageDataHandler.GetRenderData(Guid);
            if (data.PreviousImage == null) await RenderImage(data.OriginalImage);
            else await RenderPipelineImage(data);
            await base.OnAfterRenderAsync(firstRender);
        }

        public async Task OnReRenderImage(Guid guid)
        {
            if (Guid != guid) return;
            var data = ImageDataHandler.GetRenderData(Guid);
            await RenderPipelineImage(data);
            StateHasChanged();
        }

        public Task OnImageRemoved(Guid guid)
        {
            if (Guid != guid) return Task.CompletedTask;
            Dispose();
            return Task.CompletedTask;
        }

        public Task OnImageSelected(Guid guid)
        {
            if (Guid != guid) _selected = false;
            else _selected = true;
            StateHasChanged();
            return Task.CompletedTask;
        }

        public async Task RenderImage(string image)
        {
            await CvInterop.DrawSourceImage(Guid, image);
            await InvokePictureRendered();
        }

        public async Task RenderPipelineImage(MatImageData pipelineAction)
        {
            var parameter = pipelineAction.StepParameter.Select(p => p.Value).ToArray();
            await CvInterop.ExecutePipelineAction(pipelineAction.Step.JsName, pipelineAction, parameter);
        }

        public async Task InvokePictureRendered()
        {
            await ImageDataHandler.ImageRendered(new MatImageData()
            {
                Guid = Guid,
            });
        }

        public void Dispose()
        {
            _selected = false;
            ImageDataHandler.ImageSelected -= OnImageSelected;
            ImageDataHandler.ReRenderImage -= OnReRenderImage;
            ImageDataHandler.ImageRemoved -= OnImageRemoved;
            _eventsAttached = false;
        }
    }
}