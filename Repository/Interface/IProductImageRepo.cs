using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.Repository
{
    public interface IProductImageRepo
    {
        Task<product_image> GetByIDAsync(int id);
        Task<List<product_image>> GetAllAsync();
        Task<int> UpdateAsync(product_image productImage, int id);
        Task<int> DeleteAsync(int id);
        Task<int> AddnewAsync(product_image productImage);
        Task<List<product_image>> GetListImg_product_byID(int id);
    }
}