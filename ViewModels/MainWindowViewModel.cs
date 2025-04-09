using NouchKill.IO;
using NouchKill.Services;
using ReactiveUI;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NouchKill.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        public Interaction<SettingViewModel, SettingViewModel?> ShowSettingDialog { get; }

        private WebcamStream _stream = new WebcamStream();
        public WebcamStream Stream
        {
            get { return _stream; }
            set
            {
                this.RaiseAndSetIfChanged(ref _stream, value);
            }
        }


        private OnnxStream _onnx = new FasterRcnnOnnxStream();
        public OnnxStream Onnx
        {
            get { return _onnx; }
            set
            {
                this.RaiseAndSetIfChanged(ref _onnx, value);
            }
        }

        public ICommand OpenMainWindowCommand => ReactiveCommand.CreateFromTask(OnOpened);
        public ICommand CloseMainWindowCommand => ReactiveCommand.CreateFromTask(OnClosing);

        public ICommand ExitCommand => ReactiveCommand.CreateFromTask(AppExit);
        public ICommand OpenSettingCommand => ReactiveCommand.CreateFromTask(OpenSetting);

        private readonly SettingService settingService;

        public MainWindowViewModel(SettingService settingService)
        {
            ShowSettingDialog = new Interaction<SettingViewModel, SettingViewModel?>();
            this.settingService = settingService;
        }
        private async Task OpenSetting()
        {
            var store = new SettingViewModel(settingService);
            var result = await ShowSettingDialog.Handle(store);
        }

        private async Task AppExit()
        {
            Environment.Exit(0);
        }

        private async Task OnOpened()
        {
            Console.WriteLine("Fenêtre ouverte !");
            _onnx.Start();
            _stream.Start();
        }

        private async Task OnClosing()
        {
            Console.WriteLine("Fermeture en cours !");
            _onnx.Stop();
            _stream.Stop();
        }

    }
}
