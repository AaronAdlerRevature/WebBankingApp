using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.DataAnnotations;

namespace NetBankWebApp.Models
{
    public class CustomerModel
    {    
        [Required, Key]
        public string Username { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required, DataType(DataType.Date), Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }
        [Required][RegularExpression(@"^\d{3}-\d{2}-\d{4}$")]
        public string SSN { get; set; }

    }
}
