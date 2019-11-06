using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace NetBankWebApp.Models.Models
{
    public class AccountTypeDetails
    {
        [Required, Key]
        public int typeId { get; set; }
        [Required]
        public string typeName { get; set; }
        [Required]
        public bool canTransfer { get; set; }
        [Required]
        public bool canOverdraft { get; set; }
    }


}
