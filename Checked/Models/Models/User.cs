using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Checked.Models.Models
{
    public class User: IdentityUser
    {
        
        [PersonalData]
        public DateTime register { get; set; }
        [PersonalData]
        public string City { get; set; }
        [PersonalData]
        public string Region { get; set; }
        [PersonalData]
        [DataType(DataType.PostalCode)]
        public string PostalCode { get; set; }
        [PersonalData]
        public string Country { get; set; } 
    }
}
