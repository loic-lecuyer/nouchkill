using System.Collections.Generic;

namespace NouchKill.Models
{
    public class Rule
    {
        public string Name { get; set; } = "Rule";
        public Trigger Trigger { get; set; }

        public List<Action> Actions { get; set; } = new List<Action>();
    }
}
