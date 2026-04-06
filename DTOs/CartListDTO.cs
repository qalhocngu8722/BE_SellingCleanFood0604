namespace Project_Selling_Clean_Food.DTOs
{
    public class CartListDTO
    {
        public string name { get; set; }
        public decimal price { get; set; }
        public string unit { get; set; }
        public int id { get; set; }
        public int quantity { get; set; }
        public int product_id { get; set; }
        public string image_url { get; set; }
    }
}