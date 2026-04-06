namespace Project_Selling_Clean_Food.DTOs
{
    public class OrderListDTO
    {
        public int id { get; set; }
        public string payment_method { get; set; }
        public string payment_status { get; set; }
        public string order_status { get; set; }
        public DateTime order_date { get; set; }
        public string image_url { get; set; }
        public string name { get; set; }
        public int quantity { get; set; }
        public decimal unit_price { get; set; }
        public decimal total_amount { get; set;}
    }
}