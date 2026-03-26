namespace Project_Selling_Clean_Food.Model
{
    public class cart_item
    {
        public int? id { get; set; }
        public int cart_id { get; set; }
        public int product_id { get; set; }
        public int quantity { get; set; }
        public DateTime added_at { get; set; } = DateTime.Now;
    }
}
