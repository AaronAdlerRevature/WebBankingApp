using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NetBankWebApp.Models.Models
{
    public class TransactionModel
    {
        public int id { get; set; }
        public string UserName { get; set; }
        [Required]
        public int transferTypeId { get; set; }
        [Required]
        public string accId { get; set; }
        public string toAccId { get; set; }
        [Required, Range(0, 1E21 - 1), Column(TypeName = "decimal(20,2)")]
        public decimal Amount { get; set; }
        [Required]
        public DateTime CreateTime { get; set; }
        
    }
}
