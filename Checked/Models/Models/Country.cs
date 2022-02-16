using System;
using System.Collections.Generic;

namespace Checked.Models.Models
{
    public partial class Country
    {
        public Country()
        {
            States = new HashSet<State>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual ICollection<State> States { get; set; }
    }
}
