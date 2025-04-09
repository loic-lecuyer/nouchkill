using System.Collections.ObjectModel;

namespace NouchKill.ViewModels
{
    public interface ISettingViewModel
    {
        ObservableCollection<RuleViewModel> Rules { get; set; }
    }
}