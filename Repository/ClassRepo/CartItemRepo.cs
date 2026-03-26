using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.Repository
{
    public class CartItemRepo : BaseRepo, ICartItemRepo
    {
        public CartItemRepo(IConfiguration configuration) : base(configuration) { }

        public async Task<cart_item> GetByIDAsync(int id)
        {
            return await GetByIDAsync<cart_item>(id);
        }

        public async Task<List<cart_item>> GetAllAsync()
        {
            return await GetAllAsync<cart_item>();
        }

        public async Task<int> UpdateAsync(cart_item cartItem, int id)
        {
            return await UpdateAsync<cart_item>(cartItem, id);
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await DeleteAsync<cart_item>(id);
        }

        public async Task<int> AddnewAsync(cart_item cartItem)
        {
            return await AddnewAsync<cart_item>(cartItem);
        }
    }
}