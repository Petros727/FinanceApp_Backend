using System;
using System.ComponentModel.DataAnnotations;

namespace FinancialApp.Models
{
    public class Stock
    {
        [Key]
        public required string Symbol { get; set; }
        public DateTime Date { get; set; }
        public double Price { get; set; }
    }
}
