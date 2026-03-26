namespace Project_Selling_Clean_Food.Model
{
    public class orders
    {
        public int? id { get; set; }
        public int user_id { get; set; }
        public DateTime order_date { get; set; } = DateTime.Now;
        public decimal total_amount { get; set; }
        public string payment_method { get; set; }
        public string payment_status { get; set; } = "unpaid";
        public string order_status { get; set; } = "pending";
        public string shipping_address { get; set; }
        public string note { get; set; }
        public string recipient_name { get; set; }
        public string phone { get; set; }
        public string address { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
    }
}
