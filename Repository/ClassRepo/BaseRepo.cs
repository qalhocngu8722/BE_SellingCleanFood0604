using System.Data.SqlClient;
using System.Data;
using Npgsql;
using Dapper;

namespace Project_Selling_Clean_Food.Repository
{
    public abstract class BaseRepo
    {
        public  readonly IConfiguration _configuration;
        public BaseRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        //public IDbConnection Getconnection()
        //{
        //    return new NpgsqlConnection(_configuration.GetConnectionString("MyConnection"));
        //}
        public IDbConnection Getconnection()
        {
            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("DB_CONNECTION not found ❌");
            }

            return new NpgsqlConnection(connectionString);
        }
        public async Task<T?> GetByIDAsync<T>(int id) where T : class
        {
            using var con = Getconnection();
            string idField = typeof(T).GetProperties().First().Name;
            string nametable = typeof(T).Name;
            string query = $@"SELECT * FROM {nametable} WHERE {idField} = @id";
            var result = await con.QueryFirstOrDefaultAsync<T>(query, new { id });
            return result;
        }
        public async Task<List<T>> GetAllAsync<T>() where T : class
        {
            using var con = Getconnection();
            string nametable = typeof(T).Name;
            string query = $@"SELECT * FROM {nametable} order by id ASC";
            var result = await con.QueryAsync<T>(query);
            return result.ToList();
        }
        public async Task<int> AddnewAsync<T>(T entity) where T : class
        {
            using var con = Getconnection();
            string name_table = typeof(T).Name;
            var properties = typeof(T).GetProperties().Skip(1).ToList();
            var id_namefield = typeof(T).GetProperties().First().Name;
            string list_field_insert = string.Join(", ", properties.Select(p => p.Name));
            string list_value_insert = string.Join(", ", properties.Select(p => "@" + p.Name));
            var query = $@"INSERT INTO {name_table}({list_field_insert})
                        VALUES ({list_value_insert}) returning {id_namefield}";
            var newid = await con.QuerySingleAsync<int>(query, entity);
            return newid;
        }

        public async Task<int> UpdateAsync<T>(T entity, int id,string? table = null) where T : class
        {
            using var con = Getconnection();
            string name_table = table ?? typeof(T).Name;
            string idField = typeof(T).GetProperties().First().Name;
            var check = entity.GetType().GetProperties().Where(p =>
            {
                if (p.GetValue(entity) == null)
                {
                    return false;
                }
                return true;
            }
                );
            var properties = check.Where(p => p.Name != idField).ToList();
            string list_field_update = string.Join(", ", properties.Select(p => p.Name + " = @" + p.Name));
            string query = $@"UPDATE {name_table} SET {list_field_update} WHERE {idField} = {id}";
            //await Console.Out.WriteLineAsync(query);
            var result = await con.ExecuteAsync(query, entity);
            return result;
        }
        public async Task<int> DeleteAsync<T>(int id) where T : class
        {
            using var con = Getconnection();
            string name_table = typeof(T).Name;
            string idField = typeof(T).GetProperties().First().Name;
            string query = $@"DELETE FROM {name_table} WHERE {idField} = @id";
            var result = await con.ExecuteAsync(query, new { id });
            return result;
        }
    }
}
