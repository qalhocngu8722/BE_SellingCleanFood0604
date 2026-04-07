using System;

namespace Project_Selling_Clean_Food.DTOs
{
    public class RevenueByDayDTO
    {
        public DateOnly day { get; set; }
        public int sodonhang { get; set; }
        public decimal total_amount { get; set; }
    }
}
