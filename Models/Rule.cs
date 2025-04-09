using System.Collections.Generic;

namespace NouchKill.Models
{
    public class Rule
    {
        public string Name { get; set; }
        public Trigger Trigger { get; set; }

        public List<Action> Actions { get; set; }
    }
}
