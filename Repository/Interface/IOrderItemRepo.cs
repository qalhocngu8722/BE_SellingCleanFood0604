using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.Repository
{
    public interface IOrderItemRepo
    {
        Task<Order_Item> GetByIDAsync(int id);
        Task<List<Order_Item>> GetAllAsync();
        Task<int> UpdateAsync(Order_Item orderItem, int id);
        Task<int> DeleteAsync(int id);
        Task<int> AddnewAsync(Order_Item orderItem);
    }
}