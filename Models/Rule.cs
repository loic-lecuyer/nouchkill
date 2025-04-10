using System;
using System.Collections.Generic;

namespace NouchKill.Models
{
    public class Rule
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "Rule";
        public Trigger Trigger { get; set; } = new Trigger();

        public List<Action> Actions { get; set; } = new List<Action>();
    }
}
