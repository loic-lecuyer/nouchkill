using System.Collections.ObjectModel;

namespace NouchKill.ViewModels
{
    public class DesignSettingViewModel : ViewModelBase, ISettingViewModel
    {
        public ObservableCollection<RuleViewModel> Rules { get; set; } = new ObservableCollection<RuleViewModel>();
        public DesignSettingViewModel()
        {

        }

    }
}
