using DynamicData;
using NouchKill.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;

namespace NouchKill.ViewModels
{
    public class RuleViewModel : ViewModelBase
    {
        public string Id { get; set; }

        private bool _isTriggered = false;
        public bool IsTriggered {
            get { return _isTriggered; }
            set {
                this.RaiseAndSetIfChanged(ref _isTriggered, value);
            }
        }



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
            Id = rule.Id;   
            Trigger = new TriggerViewModel(rule.Trigger);
            Actions.AddRange((from a in rule.Actions select ActionViewModel.Create(a)).ToList());

        }

        internal Rule ToRule()
        {
            Rule rule = new Rule();
            rule.Name = Name;
            rule.Id = Id;   
            rule.Trigger = Trigger.ToTrigger();
            rule.Actions = Actions.Select(a => a.ToAction()).ToList();
            return rule;
        }
    }
}
