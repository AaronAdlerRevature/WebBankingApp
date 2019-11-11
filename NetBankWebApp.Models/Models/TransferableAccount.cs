using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace NetBankWebApp.Models.Models
{
    public class TransferableAccount
    {
        public int id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string accId { get; set; }
        [Required, Range(0, 1), DisplayFormat(DataFormatString = "{0:P4}"), Column(TypeName = "decimal(20,20)")]
        public decimal InterestRate { get; set; }
        [Required, Column(TypeName = "decimal(20,2)")]
        public decimal Balance { get; set; }
        public bool CanOverdraft { get; set; }
        [Range(double.Epsilon, 1E21 - 1), Column(TypeName = "decimal(20,2)")]
        public decimal? ToTransfer { get; set; }
    }
}
