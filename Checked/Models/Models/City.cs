using System;
using System.Collections.Generic;

namespace Checked.Models.Models
{
    public partial class City
    {
        public int Id { get; set; }
        public int Ibge { get; set; }
        public int Ibge7 { get; set; }
        public string Uf { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Region { get; set; } = null!;
        public int StateId { get; set; }

        public virtual State State { get; set; } = null!;
    }
}
