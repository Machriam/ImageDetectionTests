using Microsoft.AspNetCore.Components;
using OpenCvSharp;

namespace ImageDetectionTests.Client.Components;

public class StepParameter
{
    public StepParameter Clone()
    {
        return (StepParameter)MemberwiseClone();
    }

    public string RawInput { get; set; } = "";
    public object Value { get; set; } = new();
}

public partial class PipelineStepSelection : IDisposable
{
    private List<StepParameter> _parameters = new();
    [Inject] private IImageDataHandler ImageDataHandler { get; set; } = default!;

    protected override void OnInitialized()
    {
        ImageDataHandler.ImageParameterChanged += ImageDataHandler_ImageParameterChanged;
        base.OnInitialized();
    }

    private Task ImageDataHandler_ImageParameterChanged(PipelineStep? arg1, List<StepParameter>? arg2)
    {
        _parameters = arg2 ?? new();
        _selectedStep = arg1;
        _selectedStepName = _selectedStep?.Name ?? "";
        StateHasChanged();
        return Task.CompletedTask;
    }

    private PipelineStep? _selectedStep;
    private string _selectedStepName = "";

    public void Clear()
    {
        ImageDataHandler.Clear();
    }

    public void RemoveStep()
    {
        ImageDataHandler.RemoveSelectedImage();
    }

    public Task AddFilter()
    {
        if (_selectedStep == null) return Task.CompletedTask;
        ImageDataHandler.AddImage(_selectedStep.Value, _parameters.ConvertAll(p => p.Clone()) ?? new());
        return Task.CompletedTask;
    }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; } = new();

    [Parameter] public EventCallback<Action<Mat, MatImageData>> FilterAddRequested { get; set; }

    public async Task ParameterChanged(int position, ParamInfoCV info)
    {
        if (info.ParamType == ParamType.Kernel) _parameters[position].RawInput = _parameters[position].RawInput.Replace("\t", ",").Trim();
        var text = _parameters[position].RawInput;
        _parameters[position].Value = info.ConvertTextToParam(text);
        ImageDataHandler.UpdateImageParameter(_parameters.ConvertAll(p => p.Clone()));
        await ImageDataHandler.InvokeSelectedImageChanged();
    }

    public async Task SelectedStepChanged()
    {
        _parameters = new();
        _selectedStep = null;
        if (string.IsNullOrEmpty(_selectedStepName)) return;
        _selectedStep = PipelineStepDefinition.PossibleSteps.First(ps => ps.Name == _selectedStepName);
        foreach (var item in _selectedStep.Value.ParamInfoByIndex.OrderBy(t => t.Key))
            _parameters.Add(new()
            {
                RawInput = item.Value.DefaultValue.ToString() ?? "",
                Value = item.Value.ConvertTextToParam(item.Value.DefaultValue.ToString() ?? "")
            });
        if (_selectedStep == null) return;
        await ImageDataHandler.SelectedStepChanged(_selectedStep.Value, _parameters);
    }

    public void Dispose()
    {
        ImageDataHandler.ImageParameterChanged -= ImageDataHandler_ImageParameterChanged;
    }
}