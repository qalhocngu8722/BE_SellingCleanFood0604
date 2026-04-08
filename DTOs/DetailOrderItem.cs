namespace Project_Selling_Clean_Food.DTOs
{
    public class DetailOrderItem
    {
        public int? id { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }
        public int quantity { get; set; }
        public decimal unit_price { get; set; }
        public string product_name { get; set; }
        public string product_image_url { get; set; }
        public decimal subtotal => quantity * unit_price;
    }
}
