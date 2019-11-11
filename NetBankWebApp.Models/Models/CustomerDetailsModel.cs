using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace NetBankWebApp.Models
{
    public class CustomerDetailsModel
    {
        [Required, Key]
        public string Username { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required, RegularExpression(@"^\d{5}(-\d{4})?$")]
        public string ZipCode { get; set; }
        [Required]
        public string Country { get; set; }
    }
}
