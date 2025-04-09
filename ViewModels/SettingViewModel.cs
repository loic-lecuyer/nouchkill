using NouchKill.Models;
using NouchKill.Services;
using System.Collections.ObjectModel;

namespace NouchKill.ViewModels
{
    public class SettingViewModel : ViewModelBase, ISettingViewModel
    {
        public ObservableCollection<RuleViewModel> Rules { get; set; } = new ObservableCollection<RuleViewModel>();

        public SettingViewModel(SettingService settingService)
        {
            Settings settings = settingService.LoadSetting();
            settings.Rules.ForEach(rule =>
            {
                Rules.Add(new RuleViewModel(rule));
            });
        }
    }
}
