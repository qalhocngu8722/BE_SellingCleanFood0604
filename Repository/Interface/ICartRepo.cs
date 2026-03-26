using Project_Selling_Clean_Food.Model;
using Project_Selling_Clean_Food.DTOs;

namespace Project_Selling_Clean_Food.Repository
{
    public interface ICartRepo
    {
        Task<cart> GetByIDAsync(int id);
        Task<List<cart>> GetAllAsync();
        Task<int> UpdateAsync(cart cart, int id);
        Task<int> DeleteAsync(int id);
        Task<List<CartListDTO>> GetCartListByUserIdAsync(int userId);

    }
}