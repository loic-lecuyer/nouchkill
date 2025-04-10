using DynamicData;
using NouchKill.IO;
using NouchKill.Models;
using NouchKill.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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
        public AgentService AgentService { get; private set; }

        public MainWindowViewModel(SettingService settingService, AgentService agentService)
        {
            this.settingService = settingService;   
            ShowSettingDialog = new Interaction<SettingViewModel, SettingViewModel?>();
            this.AgentService = agentService;
        }

        public ObservableCollection<RuleViewModel> Rules { get; set; } = new ObservableCollection<RuleViewModel>();
        private async Task OpenSetting()
        {
            var store = new SettingViewModel(settingService,this.AgentService);
            var result = await ShowSettingDialog.Handle(store);

        }

        private async Task AppExit()
        {
            Environment.Exit(0);
        }

        private async Task OnOpened()
        {
            Console.WriteLine("Fenêtre ouverte !");
            this.Rules.Clear();
            this.Rules.AddRange(settingService.LoadSetting().Rules.Select((Rule m) => { return new RuleViewModel(m); }));
       
            _onnx.OnPredictionsReady += this._onnx_OnPredictionsReady;
            _onnx.Start();
            _stream.Start();
            this.AgentService.Start();
        }

        private void _onnx_OnPredictionsReady(object? sender, System.Collections.Generic.List<Models.Prediction> e) {
            this.AgentService.SetPredictions(e, Stream);
        }

        private async Task OnClosing()
        {
            Console.WriteLine("Fermeture en cours !");
            _onnx.OnPredictionsReady -= this._onnx_OnPredictionsReady;
            this.AgentService.Stop();
            _onnx.Stop();
            _stream.Stop();
        
        }

    }
}
