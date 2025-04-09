using DynamicData;
using NouchKill.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;

namespace NouchKill.ViewModels
{
    public class RuleViewModel : ViewModelBase
    {
        private string _name = "Rule";
        public string Name
        {
            get { return _name; }
            set
            {
                this.RaiseAndSetIfChanged(ref _name, value);
            }
        }

        public TriggerViewModel? Trigger { get; set; }

        public ObservableCollection<ActionViewModel> Actions { get; set; } = new ObservableCollection<ActionViewModel>();

        public RuleViewModel(Rule rule)
        {
            Name = rule.Name;
            Trigger = new TriggerViewModel(rule.Trigger);
            Actions.AddRange((from a in rule.Actions select ActionViewModel.Create(a)).ToList());

        }

        internal Rule ToRule()
        {
            Rule rule = new Rule();
            rule.Name = Name;
            rule.Trigger = Trigger.ToTrigger();
            rule.Actions = Actions.Select(a => a.ToAction()).ToList();
            return rule;
        }
    }
}
