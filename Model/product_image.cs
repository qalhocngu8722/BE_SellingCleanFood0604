namespace Project_Selling_Clean_Food.Model
{
    public class product_image
    {
        public int? idimg { get; set; }
        public string image_url { get; set; }
        public bool is_primary { get; set; }
        public int product_id { get; set; }
    }
}
