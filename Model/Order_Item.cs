namespace Project_Selling_Clean_Food.Model
{
    public class Order_Item
    {
        public int? id { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }
        public int quantity { get; set; }
        public decimal unit_price { get; set; }
        public decimal subtotal => quantity * unit_price;
    }
}
