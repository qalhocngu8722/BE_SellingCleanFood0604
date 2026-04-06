using Dapper;
using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.Repository
{
    public class ProductImageRepo : BaseRepo, IProductImageRepo
    {
        public ProductImageRepo(IConfiguration configuration) : base(configuration) { }

        public async Task<product_image> GetByIDAsync(int id)
        {
            return await GetByIDAsync<product_image>(id);
        }

        public async Task<List<product_image>> GetAllAsync()
        {
            return await GetAllAsync<product_image>();
        }

        public async Task<int> UpdateAsync(product_image productImage, int id)
        {
            return await UpdateAsync<product_image>(productImage, id);
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await DeleteAsync<product_image>(id);
        }

        public async Task<int> AddnewAsync(product_image productImage)
        {
            return await AddnewAsync<product_image>(productImage);
        }
        public async Task<List<product_image>> GetListImg_product_byID(int id)
        {
            using var con = Getconnection();
            var query = $@"Select id as idimg,product_id,image_url,is_primary from product_image where product_id = @id";
            var res = await con.QueryAsync<product_image>(query, new {id = id });
            return res.ToList();
        }
        public async Task<int> Update_primary_img(int id, string new_url_img)
        {
            using var con = Getconnection();
            // ✅ FIX: Use = true instead of 'is true' for clearer intention
            var query = $@"update product_image 
                           set image_url = @new_url_img 
                           where is_primary = true and product_id = @id";
            var affectedrow = await con.ExecuteAsync(query, new { new_url_img = new_url_img, id = id });
            return affectedrow;
        }

        public async Task<int> Delete_img_not_primary_img(int id)
        {
            using var con = Getconnection();
            // ✅ FIX: Use = false instead of 'is not true' for clear secondary images
            var query = $@"delete from product_image 
                           where is_primary = false and product_id = @id";
            var affectedrow = await con.ExecuteAsync(query, new { id = id });
            return affectedrow;
        }
    }
}