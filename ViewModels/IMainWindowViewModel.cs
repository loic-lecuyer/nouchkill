using NouchKill.IO;
using ReactiveUI;
using System.Windows.Input;

namespace NouchKill.ViewModels
{
    public interface IMainWindowViewModel
    {
        ICommand CloseMainWindowCommand { get; }
        ICommand ExitCommand { get; }
        OnnxStream Onnx { get; set; }
        ICommand OpenMainWindowCommand { get; }
        ICommand OpenSettingCommand { get; }
        Interaction<SettingViewModel, SettingViewModel?> ShowSettingDialog { get; }
        WebcamStream Stream { get; set; }
    }
}