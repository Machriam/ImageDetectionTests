using ImageDetectionTests.Client.Components;
using ImageDetectionTests.Client.Extensions;
using Microsoft.JSInterop;

namespace ImageDetectionTests.Client.Core
{
    public interface IOpenCvInterop
    {
        Task DrawSourceImage(Guid guid, string source);

        Task ExecutePipelineAction(string name, MatImageData data, params object[] parameter);
    }

    public class OpenCvInterop(IJSRuntime js) : IOpenCvInterop
    {
        private readonly IJSRuntime _jsRuntime = js;
        private const string OpenCvModulePath = $"./{nameof(Components)}/{nameof(Image)}.razor.js";

        public async Task DrawSourceImage(Guid guid, string source)
        {
            await _jsRuntime.ExecuteModuleFunction("DrawSourceImage", new List<object> { guid, source }, OpenCvModulePath);
        }

        public async Task ExecutePipelineAction(string name, MatImageData data, params object[] parameter)
        {
            await _jsRuntime.ExecuteModuleFunction(name, new List<object>() { data.PreviousImage!, data.Guid, parameter }, OpenCvModulePath);
        }
    }
}