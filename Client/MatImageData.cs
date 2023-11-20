using ImageDetectionTests.Client.Components;

namespace ImageDetectionTests.Client;

public class MatImageData
{
    public string OriginalImage { get; set; } = "";
    public Guid Guid { get; init; } = Guid.NewGuid();
    public Guid? PreviousImage { get; set; }
    public PipelineStep Step { get; set; } = new();
    public List<StepParameter> StepParameter { get; set; } = new();

    public Action<Guid> GetAction(IImageDataHandler dataHandler)
    {
        if (PreviousImage == null) throw new Exception("No Previous Image found");
        var data = dataHandler.GetRenderData(PreviousImage.Value);
        return dest =>
        {
        };
    }
}