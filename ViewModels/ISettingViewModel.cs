using NouchKill.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace NouchKill.ViewModels
{
    public interface ISettingViewModel
    {
        ActionViewModel? SelectedAction { get; set; }
        RuleViewModel? SelectedRule { get; set; }
        ObservableCollection<RuleViewModel> Rules { get; set; }
        ICommand AddRuleCommand { get; }
        ICommand AddActionCommand { get; }


        ICommand ApplyCommand { get; }
        List<ActionType> ActionTypes { get; }
        List<TriggerMode> TriggerModes { get; }


    }
}