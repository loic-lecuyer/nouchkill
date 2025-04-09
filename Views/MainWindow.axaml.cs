using Avalonia.Controls;
using Avalonia.ReactiveUI;
using NouchKill.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;

namespace NouchKill.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel> {
        public MainWindow()
        {
            InitializeComponent();
            this.WhenActivated(action =>
               action(ViewModel!.ShowSettingDialog.RegisterHandler(DoShowDialogAsync)));
        }
        private async Task DoShowDialogAsync(InteractionContext<SettingViewModel,SettingViewModel?> interaction) {
            var dialog = new SettingView();
            dialog.DataContext = interaction.Input;
            var result = await dialog.ShowDialog<SettingViewModel?>(this);
            interaction.SetOutput(result);
        }
    }
}