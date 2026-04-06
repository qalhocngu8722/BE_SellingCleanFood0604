using Dapper;
using Npgsql;
using System.Data;
using System.Reflection;
using Project_Selling_Clean_Food.DTOs;
using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.Repository
{
    public class UserRepo : BaseRepo,IUsersRepo
    {
        public UserRepo(IConfiguration configuration) : base(configuration)
        {
        }
        public async Task<users> GetByIDAsync(int id)
        {
            return await GetByIDAsync<users>(id);
        }
        public async Task<List<users>> GetAllAsync()
        {
            return await GetAllAsync<users>();
        }
        public async Task<int> UpdateAsync(users user, int id)
        {
            using var con = Getconnection();
            var query = $@"update users set password = @password , email = @email, role = @role::role_type, name = @name where id = {id}";
            var affactedrow = await con.ExecuteAsync(query, user);
            return affactedrow;
        }
        public async Task<int> DeleteAsync(int id)
        {
            using var con = Getconnection();
            return await DeleteAsync<users>( id);
        }
        public async Task<int> AddnewAsync(users user)
        {
            using var con = Getconnection();
            var query = $@"insert into users(name,email,password,role)
                        values(@name,@email,@password,@role::role_type) returning id";
            var newID = await con.QuerySingleAsync<int>(query, user);
            return newID;
        }
        public async Task<DetailIn4User> Login(string email,string password)
        {
            using var con = Getconnection();
            var query = $@"Select u.*,c.id as cart_id from users as u join cart as c on u.id = c.user_id where u.email = @email and u.password = @password";
            var res = await con.QuerySingleOrDefaultAsync<DetailIn4User>(query, new { email = email, password = password });
            return res;
        }
        public async Task<bool> SignUp(string name, string email, string password, string repeatPassword)
        {
            if (password != repeatPassword)
            {
                throw new Exception("Mật khẩu không khớp");
            }
            using var con = Getconnection();
            var existingUser = await con.QuerySingleOrDefaultAsync<users>(
                "SELECT * FROM users WHERE email = @email", new { email });
            if (existingUser != null)
            {
                throw new Exception("Email đã tồn tại");
            }
            var query = @"INSERT INTO users(name, email, password, role) VALUES(@name, @email, @password, 'user')";
            await con.ExecuteAsync(query, new { name, email, password });
            return true;
        }
    }
}
