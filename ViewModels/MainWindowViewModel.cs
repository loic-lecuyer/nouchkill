using NouchKill.IO;
using ReactiveUI;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NouchKill.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
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
