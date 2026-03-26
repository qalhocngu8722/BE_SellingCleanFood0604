using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.Repository
{
    public class ProductCategoryRepo : BaseRepo, IProductCategoryRepo
    {
        public ProductCategoryRepo(IConfiguration configuration) : base(configuration) { }

        public async Task<Product_Category> GetByIDAsync(int id)
        {
            return await GetByIDAsync<Product_Category>(id);
        }

        public async Task<List<Product_Category>> GetAllAsync()
        {
            return await GetAllAsync<Product_Category>();
        }

        public async Task<int> UpdateAsync(Product_Category productCategory, int id)
        {
            return await UpdateAsync<Product_Category>(productCategory, id);
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await DeleteAsync<Product_Category>(id);
        }

        public async Task<int> AddnewAsync(Product_Category productCategory)
        {
            return await AddnewAsync<Product_Category>(productCategory);
        }
    }
}