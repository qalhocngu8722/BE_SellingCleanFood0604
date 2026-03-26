namespace Project_Selling_Clean_Food.Model
{
    public class payment_transaction
    {
        public int? id { get; set; }
        public int order_id { get; set; }
        public decimal amount { get; set; }
        public string payment_method { get; set; }
        public string transaction_id { get; set; }
        public string status { get; set; } = "pending";
        public DateTime? paid_at { get; set; }
    }
}
