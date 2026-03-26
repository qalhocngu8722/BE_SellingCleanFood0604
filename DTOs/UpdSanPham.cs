using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.DTOs
{
    public class UpdSanPham
    {
        public string? name_sp { get; set; }
        public int? iddm { get; set; }
        public int? soluong { get; set; }
        public decimal? price { get; set; }
        public string? decription { get; set; }
        public string? donvi { get; set; }
        public List<product_image> img_sp { get; set; }
    }
}
