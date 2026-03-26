using Project_Selling_Clean_Food.Model;
using Project_Selling_Clean_Food.DTOs;
using Dapper;

namespace Project_Selling_Clean_Food.Repository
{
    public class CartRepo : BaseRepo, ICartRepo
    {
        public CartRepo(IConfiguration configuration) : base(configuration) { }

        public async Task<cart> GetByIDAsync(int id)
        {
            return await GetByIDAsync<cart>(id);
        }

        public async Task<List<cart>> GetAllAsync()
        {
            return await GetAllAsync<cart>();
        }

        public async Task<int> UpdateAsync(cart cart, int id)
        {
            return await UpdateAsync<cart>(cart, id);
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await DeleteAsync<cart>(id);
        }
        // Lấy list giỏ hàng hiện tại của user
        public async Task<List<CartListDTO>> GetCartListByUserIdAsync(int userId)
        {
            using var con = Getconnection();
            var query = @"select p.name, p.price, p.unit, ci.quantity from cart as c
                        join cart_item as ci on c.id = ci.cart_id
                        join products as p on ci.product_id = p.id
                        where c.user_id = @id";
            var result = await con.QueryAsync<CartListDTO>(query, new { id = userId });
            return result.ToList();
        }
    }
}