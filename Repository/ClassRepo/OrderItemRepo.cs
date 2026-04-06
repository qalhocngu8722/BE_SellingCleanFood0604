using Dapper;
using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.Repository
{
    public class OrderItemRepo : BaseRepo, IOrderItemRepo
    {
        public OrderItemRepo(IConfiguration configuration) : base(configuration) { }

        public async Task<Order_Item> GetByIDAsync(int id)
        {
            return await GetByIDAsync<Order_Item>(id);
        }

        public async Task<List<Order_Item>> GetAllAsync()
        {
            return await GetAllAsync<Order_Item>();
        }

        public async Task<int> UpdateAsync(Order_Item orderItem, int id)
        {
            return await UpdateAsync<Order_Item>(orderItem, id);
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await DeleteAsync<Order_Item>(id);
        }

        public async Task<int> AddnewAsync(Order_Item orderItem)
        {
            using var con = Getconnection();
            var query = $@"INSERT INTO Order_Item
                        (
                            order_id,
                            product_id,
                            quantity,
                            unit_price
                        )
                        VALUES
                        (
                            @order_id,
                            @product_id,
                            @Quantity,
                            @unit_price
                        )
                        RETURNING id;";
            var newid = await con.QuerySingleAsync<int>(query, orderItem);
            return newid;

        }
    }
}