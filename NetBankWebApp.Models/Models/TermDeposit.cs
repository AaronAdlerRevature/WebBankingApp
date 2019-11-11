using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NetBankWebApp.Models.Models
{
    public class TermDeposit
    {
        [Required]
        public int id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string? accId { get; set; }
        [Required, Range(0, 1), DisplayFormat(DataFormatString = "{0:P4}"), Column(TypeName = "decimal(20,20)")]
        public decimal InterestRate { get; set; }
        [Required, Column(TypeName = "decimal(20,2)"), Range(double.Epsilon, double.PositiveInfinity)]
        public decimal Balance { get; set; }
    }
}
