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

        public ICommand OpenMainWindowCommand => ReactiveCommand.CreateFromTask(OnOpened);
        public ICommand CloseMainWindowCommand => ReactiveCommand.CreateFromTask(OnClosing);

        private async Task OnOpened()
        {
            Console.WriteLine("Fenêtre ouverte !");
            _stream.Start();
        }

        private async Task OnClosing()
        {
            Console.WriteLine("Fermeture en cours !");
            _stream.Stop();
        }

    }
}
