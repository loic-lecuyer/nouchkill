namespace NouchKill.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // Modèle downlad : https://github.com/onnx/models/blob/main/validated/vision/object_detection_segmentation/faster-rcnn/model/FasterRCNN-10.onnx
#pragma warning disable CA1822 // Mark members as static
        public string Greeting => "Welcome to Avalonia!";
#pragma warning restore CA1822 // Mark members as static
    }
}
