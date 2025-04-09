using NouchKill.IO;
using ReactiveUI;
using System.Windows.Input;

namespace NouchKill.ViewModels
{
    public class DesignMainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        public ICommand CloseMainWindowCommand => ReactiveCommand.CreateFromTask(() => { return null; });

        public ICommand ExitCommand => ReactiveCommand.CreateFromTask(() => { return null; });

        public OnnxStream Onnx { get; set; }

        public ICommand OpenMainWindowCommand => ReactiveCommand.CreateFromTask(() => { return null; });

        public ICommand OpenSettingCommand => ReactiveCommand.CreateFromTask(() => { return null; });

        public Interaction<SettingViewModel, SettingViewModel?> ShowSettingDialog => new Interaction<SettingViewModel, SettingViewModel?>();

        public WebcamStream Stream { get; set; }
    }
}
