using global::Microsoft.AspNetCore.Components;
using OpenCvSharp;

namespace ImageDetectionTests.Client.Components;

public partial class PipelineStepSelection
{
    private readonly List<PipelineStep> PossibleSteps = new()
        {
            new PipelineStep()
            {
                Name = "Canny",
                Action = (src, dest, p) =>  Cv2.Canny(src, dest, (int)p[0], (int)p[1]),
                TypeDictionary = new()
                {
                    {
                        (0, "threshold1"),
                        TypeCode.Int32
                    },
                    {
                        (1, "threshold2"),
                        TypeCode.Int32
                    }
                }
            },
            new PipelineStep()
            {
                Name = "Median Blur",
                Action = (src, dest, p) => Cv2.MedianBlur(src, dest, (int)p[0]),
                TypeDictionary = new()
                {
                    {
                        (0, "ksize"),
                        TypeCode.Int32
                    }
                }
            }
        };

    private List<object> _parameters = new();
    [Inject] private IImageDataHandler ImageDataHandler { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    private PipelineStep? _selectedStep;

    public Task AddFilter()
    {
        if (_selectedStep == null) return Task.CompletedTask;
        ImageDataHandler.AddImage((dest, data) =>
        {
            using var source = data.CreateMatFromRGBA();
            _selectedStep.Action(source, dest, _parameters.ToArray());
        });
        return Task.CompletedTask;
    }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; } = new();

    [Parameter] public EventCallback<Action<Mat, MatImageData>> FilterAddRequested { get; set; }

    public void SelectedStepChanged(ChangeEventArgs args)
    {
        var stepName = (string?)args.Value;
        _parameters = new();
        _selectedStep = null;
        if (string.IsNullOrEmpty(stepName)) return;
        _selectedStep = PossibleSteps.First(ps => ps.Name == stepName);
        foreach (var item in _selectedStep.TypeDictionary.OrderBy(t => t.Key.Position)) _parameters.Add(TranslateType(item.Value));
    }

    private object TranslateType(TypeCode typeCode)
    {
        switch (typeCode)
        {
            case TypeCode.Boolean:
                return false;

            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Int32:
                return 0;

            case TypeCode.Single:
                return 0f;

            case TypeCode.Double:
                return 0d;

            case TypeCode.Decimal:
                return 0M;

            case TypeCode.String:
                return "";

            default:
                throw new Exception("Unknown Type found");
        }
    }
}