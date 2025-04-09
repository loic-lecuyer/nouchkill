using NouchKill.Models;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;

namespace NouchKill.ViewModels
{
    public class TriggerViewModel : ViewModelBase
    {
        public ObservableCollection<LabelViewModel> Classes { get; set; } = new ObservableCollection<LabelViewModel>();

        private TriggerMode _mode = TriggerMode.OneAppear;


        public TriggerMode Mode
        {
            get { return _mode; }
            set
            {
                this.RaiseAndSetIfChanged(ref _mode, value);
            }
        }

        public TriggerViewModel()
        {
            // Fixed the error by replacing Array.ForEach with LINQ's foreach loop  
            foreach (var x in LabelMap.Labels.OrderBy(label => label))
            {
                Classes.Add(new LabelViewModel
                {
                    Name = x,
                    IsChecked = false
                });

            }
        }

        public TriggerViewModel(Trigger trigger)
        {
            foreach (var x in LabelMap.Labels.OrderBy(label => label))
            {
                Classes.Add(new LabelViewModel
                {
                    Name = x,
                    IsChecked = trigger.Classes.Contains(x)
                });

            }
            Mode = trigger.Mode;


        }

        internal Trigger ToTrigger()
        {
            Trigger trigger = new Trigger();
            trigger.Mode = Mode;
            trigger.Classes = (from c in Classes.ToList() where c.IsChecked select c.Name).ToList();
            return trigger;
        }
    }
}
