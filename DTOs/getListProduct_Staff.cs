using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.DTOs
{
    public class getListProduct_Staff
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public string unit { get; set; }
        public string origin { get; set; }
        public string food_type { get; set; }
        public int quantity { get; set; }
        public string size { get; set; }
        public string usage_instructions { get; set; }
        public string storage_instructions { get; set; }
        public DateTime? hsd { get; set; }
        public DateTime? created_at { get; set; } = DateTime.Now;
        public int category_id { get; set; }
        public string category_name { get; set; }
        public List<product_image> list_image_url { get; set; }
    }
}
