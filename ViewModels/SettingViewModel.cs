using NouchKill.Models;
using NouchKill.Services;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using NouchKill.Models;
namespace NouchKill.ViewModels
{
    public class SettingViewModel : ViewModelBase, ISettingViewModel
    {
        public ICommand ApplyCommand => ReactiveCommand.CreateFromTask(ApplySettings);


        public List<ActionType> ActionTypes => new List<ActionType>(new ActionType[] {
            ActionType.TakeScreenshot, ActionType.PlaySound
        });
        public List<TriggerMode> TriggerModes => new List<TriggerMode>(new TriggerMode[] {
            TriggerMode.OneAppear,TriggerMode.AllAppear, TriggerMode .OneDisappear, TriggerMode.AllDisappear
        });
        public ObservableCollection<RuleViewModel> Rules { get; set; } = new ObservableCollection<RuleViewModel>();
        public ICommand AddRuleCommand => ReactiveCommand.CreateFromTask(AddRule);

        private RuleViewModel? _selectedRule = null;
        public RuleViewModel? SelectedRule
        {
            get { return _selectedRule; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedRule, value);
            }
        }
        private ActionViewModel? _selectedAction = null;
        public ActionViewModel? SelectedAction
        {
            get { return _selectedAction; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedAction, value);
            }
        }
        private ActionType? _selectedActionType = null;
        private readonly SettingService settingService;
        private readonly AgentService agentService;

        public ActionType? SelectedActionType
        {
            get { return _selectedActionType; }
            set
            {
                this.RaiseAndSetIfChanged(ref _selectedActionType, value);
            }
        }
        public ICommand AddActionCommand => ReactiveCommand.CreateFromTask(AddAction);

        private async Task AddAction()
        {
            if (_selectedActionType == null) return;
            if (SelectedRule == null) return;

            Action? action = Action.Create(_selectedActionType);
            if (action == null) return;

            ActionViewModel? actionViewModel = ActionViewModel.Create(action);
            if (actionViewModel == null) return;

            SelectedRule.Actions.Add(actionViewModel);
            SelectedAction = actionViewModel;

        }

        private async Task AddRule()
        {
            RuleViewModel ruleViewModel = new RuleViewModel(new Rule());
            Rules.Add(ruleViewModel);
            SelectedRule = ruleViewModel;
            SelectedAction = null;
        }

        public SettingViewModel(SettingService settingService,AgentService agentService)
        {
            Settings settings = settingService.LoadSetting();
            settings.Rules.ForEach(rule =>
            {
                Rules.Add(new RuleViewModel(rule));
            });
            this.settingService = settingService;
            this.agentService = agentService;
        }

        private async Task ApplySettings()
        {
            Settings settings = new Settings();
            settings.Rules.AddRange(Rules.Select(rule => rule.ToRule()));
            settingService.SaveSetting(settings);
            this.agentService.Stop();
            this.agentService.Start();  
        }

    }
}
