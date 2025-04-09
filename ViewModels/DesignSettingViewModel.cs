using NouchKill.Models;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NouchKill.ViewModels
{
    public class DesignSettingViewModel : ViewModelBase, ISettingViewModel
    {
        public ObservableCollection<RuleViewModel> Rules { get; set; } = new ObservableCollection<RuleViewModel>();


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
        public ICommand CancelCommand => ReactiveCommand.CreateFromTask(() => { return null; });

        public ICommand ApplyCommand => ReactiveCommand.CreateFromTask(() => { return null; });
        public ICommand AddRuleCommand => ReactiveCommand.CreateFromTask(() => { return null; });

        public ICommand AddActionCommand => ReactiveCommand.CreateFromTask(() => { return null; });

        public List<TriggerMode> TriggerModes => new List<TriggerMode>(new TriggerMode[] {
            TriggerMode.OneAppear,TriggerMode.AllAppear, TriggerMode .OneDisappear, TriggerMode.AllDisappear
        });
        public Interaction<SettingViewModel, SettingViewModel?> CloseSettingDialog { get; }
        public List<ActionType> ActionTypes => new List<ActionType>(new ActionType[] {
            ActionType.TakeScreenshot, ActionType.PlaySound
        });

        public DesignSettingViewModel()
        {
            RuleViewModel demoRule = new RuleViewModel(new Models.Rule());
            demoRule.Name = "Demo Rule";
            Rules.Add(demoRule);
            SelectedRule = demoRule;
        }

    }
}
