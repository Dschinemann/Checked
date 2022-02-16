using System;
using System.Collections.Generic;

namespace Checked.Models.Models
{
    public partial class State
    {
        public State()
        {
            Cities = new HashSet<City>();
        }

        public int Id { get; set; }
        public int Ibge { get; set; }
        public string Name { get; set; } = null!;
        public string Uf { get; set; } = null!;
        public string Region { get; set; } = null!;
        public int? CountryId { get; set; }

        public virtual Country? Country { get; set; }
        public virtual ICollection<City> Cities { get; set; }
    }
}
