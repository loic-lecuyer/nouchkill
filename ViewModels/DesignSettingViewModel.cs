using ReactiveUI;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NouchKill.ViewModels
{
    public class DesignSettingViewModel : ViewModelBase, ISettingViewModel
    {
        public ObservableCollection<RuleViewModel> Rules { get; set; } = new ObservableCollection<RuleViewModel>();



        public ICommand AddRuleCommand => ReactiveCommand.CreateFromTask(() => { return null; });

        public DesignSettingViewModel()
        {

        }

    }
}
