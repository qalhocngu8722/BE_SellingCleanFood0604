using Project_Selling_Clean_Food.DTOs;
using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.Repository
{
    public interface IProductsRepo
    {
        Task<products> GetByIDAsync(int id);
        Task<List<products>> GetAllAsync();
        Task<int> UpdateAsync(products product, int id);
        Task<int> DeleteAsync(int id);
        Task<int> AddnewAsync(products product);
        Task<UpdSanPham> GetDetailProduct(int id);
        Task<int> UpdateDetailProduct(int id, BaseTypeFieldUpdProduct updSanPham, string img_primary, List<product_image> listimg, bool hasNewPrimaryImage = false);
        Task<int> Add_new_Detail_product(DetailProducts detailProducts);
        Task<DetailProducts> Get_Detail_Products(int id);
        Task<DetailProductWithRelatedDTO> GetDetailProductWithRelatedAsync(int productId);
        Task<List<RelatedProductDTO>> Get_list_product_homepage();
        Task<List<Render_product_dashbroad>> Render_Product_sellwell_Dashbroads();
        Task<List<getListProduct_Staff>> Get_list_product_staff_interface();
    }
}