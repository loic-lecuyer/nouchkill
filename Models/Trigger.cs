using System.Collections.Generic;

namespace NouchKill.Models
{
    public enum TriggerMode
    {
        OneAppear,
        AllAppear,
        OneDisappear,
        AllDisappear,
    }

    public class Trigger
    {
        public List<string> Classes { get; set; } = new List<string>();

        public TriggerMode Mode { get; set; } = TriggerMode.OneAppear;
    }
}
