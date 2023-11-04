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

public partial class PipelineStepSelection
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
        StateHasChanged();
        return Task.CompletedTask;
    }

    private PipelineStep? _selectedStep;

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
        var text = _parameters[position].RawInput;
        _parameters[position].Value = info.ConvertTextToParam(text);
        ImageDataHandler.UpdateImageParameter(_parameters.ConvertAll(p => p.Clone()));
        await ImageDataHandler.InvokeSelectedImageChanged();
    }

    public void SelectedStepChanged(ChangeEventArgs args)
    {
        var stepName = (string?)args.Value;
        _parameters = new();
        _selectedStep = null;
        if (string.IsNullOrEmpty(stepName)) return;
        _selectedStep = PipelineStepDefinition.PossibleSteps.First(ps => ps.Name == stepName);
        foreach (var item in _selectedStep.Value.ParamInfoByIndex.OrderBy(t => t.Key))
            _parameters.Add(new() { RawInput = item.Value.DefaultValue.ToString() ?? "", Value = item.Value.DefaultValue });
    }
}