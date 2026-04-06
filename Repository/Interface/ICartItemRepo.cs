using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.Repository
{
    public interface ICartItemRepo
    {
        Task<cart_item> GetByIDAsync(int id);
        Task<List<cart_item>> GetAllAsync();
        Task<int> UpdateAsync(int id, int quantity);
        Task<int> DeleteAsync(int id);
        Task<int> AddnewAsync(cart_item cartItem);
        Task<int> ClearCartUser(int userId);
    }
}