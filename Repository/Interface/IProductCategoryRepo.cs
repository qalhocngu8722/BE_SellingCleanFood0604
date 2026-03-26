using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.Repository
{
    public interface IProductCategoryRepo
    {
        Task<Product_Category> GetByIDAsync(int id);
        Task<List<Product_Category>> GetAllAsync();
        Task<int> UpdateAsync(Product_Category productCategory, int id);
        Task<int> DeleteAsync(int id);
        Task<int> AddnewAsync(Product_Category productCategory);
    }
}