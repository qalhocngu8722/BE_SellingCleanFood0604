using Project_Selling_Clean_Food.Model;
using Project_Selling_Clean_Food.DTOs;
using Dapper;

namespace Project_Selling_Clean_Food.Repository
{
    public class ProductsRepo : BaseRepo, IProductsRepo
    {
        private readonly IProductImageRepo _productImageRepo;
        public ProductsRepo(IConfiguration configuration) : base(configuration) {
            _productImageRepo = new ProductImageRepo(configuration);
        }

        public async Task<products> GetByIDAsync(int id)
        {
            return await GetByIDAsync<products>(id);
        }

        public async Task<List<products>> GetAllAsync()
        {
            return await GetAllAsync<products>();
        }

        public async Task<int> UpdateAsync(products product, int id)
        {
            return await UpdateAsync<products>(product, id);
        }
        // kiểm tra đường dẫn của image product mà đường dẫn bị khác 
        public async Task<UpdSanPham> GetDetailProduct(int id)
        {
            using var con = Getconnection();
            var query = $@"select p.*,pi.* from products as p 
                        join product_image as pi 
                        on p.id = pi.product_id where p.id = {id}";
            var listdtpd = await con.QuerySingleOrDefaultAsync<UpdSanPham>(query);
            return listdtpd;
        } 
        public async Task<int> UpdateDetailProduct(int id,UpdSanPham updSanPham)
        {
            using var con = Getconnection();
            var field = typeof(UpdSanPham).GetProperties().Skip(typeof(UpdSanPham).GetProperties().Length - 1).ToList();
            List<product_image> listimg = updSanPham.img_sp; 
            var code = string.Join(", ",field.Select(x => $"{x.Name} = @{x.Name}").ToList());
            var query = $@"update Products set {code} where id = {id}";
            var res = await con.ExecuteAsync(query, updSanPham);
            var res1 = await _productImageRepo.DeleteAsync(id);
            foreach(var i in updSanPham.img_sp)
            {
                var newid = await _productImageRepo.AddnewAsync(i);
                if(newid == 0)
                {
                    return 0;
                }
            }
            return (res > 0 && res1 > 0)? 1 : 0;

        }
        public async Task<int> DeleteAsync(int id)
        {
            return await DeleteAsync<products>(id);
        }
        public async Task<int> AddnewAsync(products product)
        {
            return await AddnewAsync<products>(product);
        }
        public async Task<DetailProducts> Get_Detail_Products(int id)
        {
            using var con = Getconnection();
            var query = $@"select p.id,p.name,p.description,p.price,p.unit,p.category_id,p.origin,p.food_type,p.quantity,p.size,p.usage_instructions,
                        p.storage_instructions,p.hsd,p.created_at,pi.id as idimg,pi.image_url,pi.is_primary from products as p 
                        join product_image as pi
                        on p.id = pi.product_id where p.id = {id}";
            Dictionary<int, DetailProducts> keyValuePairs = new Dictionary<int, DetailProducts>();
            await con.QueryAsync<DetailProducts, product_image, DetailProducts>(query,
                (dtuser, img) =>
                {
                    if (!keyValuePairs.TryGetValue(dtuser.id, out var detailProducts))
                    {
                        detailProducts = dtuser;
                        keyValuePairs.Add(dtuser.id, detailProducts);
                    }
                    if(img != null)
                    {
                        detailProducts.images.Add(img);
                    }
                    return detailProducts;
                },
                splitOn: "idimg"
                );
            var res = keyValuePairs.Values.SingleOrDefault();
            return res;
        }
        public async Task<int> Add_new_Detail_product(DetailProducts detailProducts)
        {
            using var con = Getconnection();

            // Map DetailProducts to products entity
            var productEntity = new products
            {
                name = detailProducts.name,
                description = detailProducts.description,
                price = detailProducts.price,
                unit = detailProducts.unit,
                category_id = detailProducts.category_id,
                origin = detailProducts.origin,
                food_type = detailProducts.food_type,
                quantity = detailProducts.quantity,
                size = detailProducts.size,
                usage_instructions = detailProducts.usage_instructions,
                storage_instructions = detailProducts.storage_instructions,
                hsd = detailProducts.hsd
            };

            // Insert product and get new id
            var properties = typeof(products).GetProperties().Skip(1).SkipLast(1).ToList();
            var id_namefield = typeof(products).GetProperties().First().Name;
            string list_field_insert = string.Join(", ", properties.Select(p => p.Name));
            string list_value_insert = string.Join(", ", properties.Select(p => "@" + p.Name));
            Console.WriteLine(list_field_insert);
            Console.WriteLine(list_value_insert);

            var query1 = $@"insert into products({list_field_insert}) values({list_value_insert}) returning id;";
            Console.Out.WriteLineAsync(query1);
            var productId = await con.QuerySingleAsync<int>(query1, productEntity);

            // Insert images
            foreach (var img in detailProducts.images)
            {
                img.product_id = productId;
                await _productImageRepo.AddnewAsync(img);
            }

            return productId;
        }

        public async Task<List<Render_product_dashbroad>> Render_Product_sellwell_Dashbroads()
        {
            using var con = Getconnection();
            var query = @$"select p.id,p.""name"",pi.image_url,sum(p.quantity) over(PARTITION by p.id) as quantity_selling from orders as o 
                        join order_item as oi 
                        on o.id = oi.order_id 
                        join products as p 
                        on oi.product_id = p.id 
                        join product_image as pi 
                        on p.id = pi.product_id
                        where extract(month from o.order_date) = EXTRACT(month from now()) 
                        and (o.order_status = 'resolve' or o.payment_status = 'paid')
                        and pi.is_primary = 'true'
                        limit 10 ";
            var list = await con.QueryAsync<Render_product_dashbroad>(query);
            return list.ToList();
        }
        // Lấy detail product kèm danh sách sản phẩm liên quan
        public async Task<DetailProductWithRelatedDTO> GetDetailProductWithRelatedAsync(int productId)
        {
            using var con = Getconnection();
            // Lấy detail product
            var queryDetail = @"select p.id, p.name, p.description, p.price, p.unit, p.category_id, pc.name as category_name, p.origin, p.food_type, p.quantity, p.size, p.usage_instructions, p.storage_instructions, p.hsd, p.created_at
                                from products as p
                                join product_category as pc on p.category_id = pc.id
                                join product_image as pi on pi.product_id = p.id
                                where p.id = @id";
            var detail = await con.QueryFirstOrDefaultAsync<DetailProductWithRelatedDTO>(queryDetail, new { id = productId });
            if (detail == null) return null;

            var queryGetListImg = await _productImageRepo.GetListImg_product_byID(productId);
            detail.image_url = queryGetListImg;
            // Lấy danh sách sản phẩm liên quan (cùng category)
            var queryRelated = @"select pc.name as category_name, p.name as product_name, pi.image_url, p.origin, p.food_type, p.price, p.unit
                                from products as p
                                join product_category as pc on p.category_id = pc.id
                                join product_image as pi on pi.product_id = p.id
                                where pc.id = @catId and p.id != @id";
            var related = (await con.QueryAsync<RelatedProductDTO>(queryRelated, new { catId = detail.category_id, id = productId })).ToList();
            detail.related_products = related;
            return detail;
        }
        public async Task<List<RelatedProductDTO>> Get_list_product_homepage()
        {
            using var con = Getconnection();
            var query = @"
                        select 
                            p.id,
                            p.name as product_name,
                            p.origin,
                            p.food_type,
                            p.price,
                            p.unit,

                            pi.id as idimg,
                            pi.image_url,

                            pc.id as id,
                            pc.name

                        from products p
                        join product_category pc on p.category_id = pc.id
                        join product_image pi on pi.product_id = p.id

                        where pi.is_primary = 1";
            var productDict = new Dictionary<int, RelatedProductDTO>();

            var list = await con.QueryAsync<RelatedProductDTO, product_image, Product_Category, RelatedProductDTO>(
                query,
                (product, image, category) =>
                {
                    if (!productDict.TryGetValue(product.id, out var dto))
                    {
                        dto = new RelatedProductDTO
                        {
                            id = product.id,
                            product_name = product.product_name,
                            origin = product.origin,
                            food_type = product.food_type,
                            price = product.price,
                            unit = product.unit,
                            image_url = image.image_url,
                            list_category_name = new List<string>()
                        };

                        productDict.Add(dto.id, dto);
                    }

                    if (!dto.list_category_name.Contains(category.name))
                    {
                        dto.list_category_name.Add(category.name);
                    }

                    return dto;
                },
                splitOn: "idimg,id"
            );

            return productDict.Values.ToList();
        }
     }
}