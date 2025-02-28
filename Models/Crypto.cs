using System;
using System.ComponentModel.DataAnnotations;

namespace FinancialApp.Models
{
    public class Crypto
    {
        [Key]
        public required string Symbol { get; set; }
        public double Price { get; set; }
        public DateTime Date { get; set; }
    }
}
