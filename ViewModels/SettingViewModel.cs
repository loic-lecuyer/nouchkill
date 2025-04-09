using NouchKill.Models;
using NouchKill.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NouchKill.ViewModels
{
    public class SettingViewModel : ViewModelBase, ISettingViewModel
    {
        public ObservableCollection<RuleViewModel> Rules { get; set; } = new ObservableCollection<RuleViewModel>();
        public ICommand AddRuleCommand => ReactiveCommand.CreateFromTask(AddRule);

        private async Task AddRule() {
            RuleViewModel ruleViewModel = new RuleViewModel(new Rule());
            this.Rules.Add(ruleViewModel);  
        }

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
