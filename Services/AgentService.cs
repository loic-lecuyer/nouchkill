using DynamicData;
using NouchKill.IO;
using NouchKill.Models;
using NouchKill.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NouchKill.Services {
    public class AgentService {
        private readonly SettingService settingService;
        public ObservableCollection<RuleViewModel> Rules { get; } = new ObservableCollection<RuleViewModel> (); 
        private Settings settings;
        private List<Prediction> previousPredictions = new List<Prediction>();
        public AgentService(SettingService settingService) {
            this.settingService = settingService;
        }
        internal void SetPredictions(List<Prediction> e, IO.WebcamStream stream) {
            foreach (var item in settings.Rules) {
                this.ProcessRule(item, e, stream);
            }
            this.previousPredictions = e;
        }

        private void ProcessRule(Rule item, List<Prediction> e, WebcamStream stream) {
            RuleViewModel? rule = (from i in this.Rules where i.Id.Equals(item.Id) select i).FirstOrDefault();
            if (this.IsTriggered(item.Trigger, e)) {
                if (rule != null) {
                    rule.IsTriggered = true;
                }
                foreach (var action in item.Actions) {
                    this.RunAction(action, e, stream);
                }
            } else {
                if (rule != null) {
                    rule.IsTriggered = false;
                }
            }
           
        }

        private void RunAction(Models.Action action, List<Prediction> e, WebcamStream stream) {
            action.Run(e, stream);
        }

        private bool IsTriggered(Trigger trigger, List<Prediction> e) {
            List<string> lowerClasses = (from c in trigger.Classes select c.ToLower()).ToList();    
            int countPrevious = (from p in this.previousPredictions where lowerClasses.Contains(p.Label.ToLower()) select p).Count();
            int countCurrent = (from p in e where lowerClasses.Contains(p.Label.ToLower()) select p).Count();
            switch (trigger.Mode) {
                case TriggerMode.AllDisappear:
                    if (countPrevious > 0 && countCurrent == 0) return true;
                    break;
                case TriggerMode.OneAppear:
                    if (countCurrent != countPrevious && countCurrent >0) return true;
                    break;
                case TriggerMode.AllAppear:
                    if (countCurrent != countPrevious && countCurrent == trigger.Classes.Count) return true;
                    break;
                case TriggerMode.OneDisappear:
                    if (countPrevious > 0 && countCurrent == 0) return true;
                    break;
            }
            return false;
        }

        internal void Start() {
            this.settings = this.settingService.LoadSetting();         
            this.Rules.AddRange(this.settings.Rules.Select((Rule r) => { return new RuleViewModel(r); }));
        }

        internal void Stop() {
            this.Rules.Clear();
        }
    }
}
