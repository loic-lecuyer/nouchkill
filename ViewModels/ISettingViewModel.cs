using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NouchKill.ViewModels
{
    public interface ISettingViewModel
    {
        ObservableCollection<RuleViewModel> Rules { get; set; }
        ICommand AddRuleCommand { get; }
    }
}