using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace NetBankWebApp.Models.Models
{
    public class CheckingAccount
    {
        public int id { get; set; }
        [Required]
        public string accId { get; set; }
        public IdentityUser UserName { get; set; }
        public decimal Balance { get; set; }
    }
}
