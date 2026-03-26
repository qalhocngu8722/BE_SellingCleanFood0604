namespace Project_Selling_Clean_Food.DTOs
{
    public class RelatedProductDTO
    {
        public int id {  get; set; }
        public string product_name { get; set; }
        public string origin { get; set; }
        public string food_type { get; set; }
        public decimal price { get; set; }
        public string unit { get; set; }
        public string image_url { get; set; }
        public List<string> list_category_name { get; set; }
    }
}