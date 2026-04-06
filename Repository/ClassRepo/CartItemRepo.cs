using Dapper;
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

        public async Task<int> UpdateAsync(int id,int quantity)
        {
            using var con = Getconnection();
            var query = @"update Cart_Item set quantity = @quantity
                        where id = @id";
            var result = await con.ExecuteAsync(query, new { id = id, quantity = quantity });
            return result;
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await DeleteAsync<cart_item>(id);
        }

        public async Task<int> AddnewAsync(cart_item cartItem)
        {
            return await AddnewAsync<cart_item>(cartItem);
        }
        public async Task<int> ClearCartUser(int userId)
        {
            using var con = Getconnection();
            var query = $@"DELETE FROM Cart_Item ci
                        USING Cart c
                        WHERE c.id = ci.cart_id
                        AND c.user_id = {userId}";
            var result = await con.ExecuteAsync(query);
            return result;
        }
    }
}