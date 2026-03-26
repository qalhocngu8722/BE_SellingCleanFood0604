using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.Repository
{
    public interface IUsersRepo
    {
        Task<users> GetByIDAsync(int id);
        Task<List<users>> GetAllAsync();
        Task<int> UpdateAsync(users user, int id);
        Task<int> DeleteAsync(int id);
        Task<int> AddnewAsync(users user);
        Task<users> Login(string email, string password);
        Task<bool> SignUp(string name, string email, string password, string repeatPassword);
    }
}
