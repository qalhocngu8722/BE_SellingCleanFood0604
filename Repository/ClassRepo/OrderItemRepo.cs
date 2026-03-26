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
            return await AddnewAsync<Order_Item>(orderItem);
        }
    }
}