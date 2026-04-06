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
        public async Task<int> UpdateDetailProduct(int id, BaseTypeFieldUpdProduct updSanPham, string img_primary, List<product_image> listimg, bool hasNewPrimaryImage = false)
        {
            using var con = Getconnection();
            var field = typeof(BaseTypeFieldUpdProduct).GetProperties().ToList();
            var code = string.Join(", ", field.Select(x => $"{x.Name} = @{x.Name}").ToList());
            var query = $@"update Products set {code} where id = {id}";
            var res = await con.ExecuteAsync(query, updSanPham);
            Console.WriteLine($"id = {id}");
            Console.WriteLine($"res = {res}");

            // ========== ✅ Xử lý ảnh chính nếu có ảnh chính MỚI ==========
            bool primaryImageSuccess = true;  // Assume success by default (no image update needed)

            if (hasNewPrimaryImage)
            {
                // ✅ FIX: Delete old secondary images (return 0 = no secondary images = still OK)
                var res1 = await _productImageRepo.Delete_img_not_primary_img(id);
                Console.WriteLine($"Deleted secondary images: {res1}");

                // ✅ FIX: Update primary image (must return > 0, else log error)
                var res2 = await _productImageRepo.Update_primary_img(id, img_primary);
                Console.WriteLine($"Updated primary image: {res2}");

                if (res2 <= 0)
                {
                    // ❌ No primary image record found to update - critical error
                    Console.WriteLine($"ERROR: Failed to update primary image for product {id}");
                    return 0;
                }

                primaryImageSuccess = (res1 >= 0 && res2 > 0);
            }

            if (!primaryImageSuccess)
                return 0;

            // ========== ✅ Thêm ảnh phụ MỚI nếu có ==========
            foreach (var img in listimg)
            {
                var imageEntity = new product_image
                {
                    image_url = img.image_url,
                    is_primary = img.is_primary,
                    product_id = id
                };
                var queryImg = $@"insert into product_image(image_url, is_primary, product_id) values(@image_url, @is_primary, @product_id);";
                var newid = await con.ExecuteAsync(queryImg, imageEntity);
                if (newid == 0)
                {
                    Console.WriteLine($"ERROR: Failed to insert secondary image for product {id}");
                    return 0;
                }
            }

            return 1;  // ✅ All successful
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
                    // Khi insert product_image, chỉ truyền image_url, is_primary, product_id
                    var imageEntity = new product_image
                    {
                        image_url = img.image_url,
                        is_primary = img.is_primary,
                        product_id = productId
                    };
                    var queryImg = $@"insert into product_image(image_url, is_primary, product_id) values(@image_url, @is_primary, @product_id);";
                    await con.ExecuteAsync(queryImg, imageEntity);
                }
            return productId;
        }

        public async Task<List<Render_product_dashbroad>> Render_Product_sellwell_Dashbroads()
        {
            using var con = Getconnection();
            var query = @$"select distinct p.id,p.""name"",pi.image_url,sum(p.quantity) over(PARTITION by p.id) as quantity_selling from orders as o 
                        join order_item as oi 
                        on o.id = oi.order_id 
                        join products as p 
                        on oi.product_id = p.id 
                        join product_image as pi 
                        on p.id = pi.product_id
                        where extract(month from o.order_date) = EXTRACT(month from now()) 
                        and (o.order_status = 'resolve' or o.payment_status = 'resolve')
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
            var queryRelated = @"select p.id,pc.name as category_name, p.name as product_name, pi.image_url, p.origin, p.food_type, p.price, p.unit
                                from products as p
                                join product_category as pc on p.category_id = pc.id
                                join product_image as pi on pi.product_id = p.id
                                where pc.id = @catId and p.id != @id and pi.is_primary is true";
            var related = (await con.QueryAsync<RelatedProductDTO>(queryRelated, new { catId = detail.category_id, id = productId })).ToList();
            foreach ( var relatedProduct in related)
            {
                var query_list_cate = $@"select pc.""name"" from products as p join product_category as pc 
						    on p.category_id = pc.id 
						    where p.id = @id";
                var list_cate = await con.QueryAsync<string>(query_list_cate, new { id = relatedProduct.id });
                relatedProduct.list_category_name = list_cate.ToList();
            }
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

                        where pi.is_primary = true";
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
        public async Task<List<getListProduct_Staff>> Get_list_product_staff_interface()
        {
            using var con = Getconnection();

            var query = @"
                        SELECT 
                            p.id,
                            p.""name"",
                            p.usage_instructions,
                            p.unit,
                            p.storage_instructions,
                            p.size,
                            p.quantity,
                            p.price,
                            p.origin,
                            p.hsd,
                            p.food_type,
                            p.description,
                            p.created_at,

                            pc.id AS id,
                            pc.""name"" AS name,

                            pi.id AS id,
                            pi.image_url,
                            pi.is_primary

                        FROM products p
                        JOIN product_category pc ON p.category_id = pc.id
                        JOIN product_image pi ON p.id = pi.product_id
                    ";

            var dict = new Dictionary<int, getListProduct_Staff>();

            await con.QueryAsync<getListProduct_Staff, Product_Category, product_image, getListProduct_Staff>(
                query,
                (product, cate, image) =>
                {
                    if (!dict.TryGetValue(product.id, out var dto))
                    {
                        dto = new getListProduct_Staff
                        {
                            id = product.id,
                            name = product.name,
                            origin = product.origin,
                            food_type = product.food_type,
                            price = product.price,
                            unit = product.unit,
                            quantity = product.quantity,
                            description = product.description,
                            storage_instructions = product.storage_instructions,
                            usage_instructions = product.usage_instructions,
                            size = product.size,
                            hsd = product.hsd,
                            category_id = cate.id ?? 0,
                            category_name = cate.name,
                            list_image_url = new List<product_image>()
                        };

                        dict.Add(dto.id, dto);
                    }

                    if (image != null && !dto.list_image_url.Any(x => x.idimg == image.idimg))
                    {
                        dto.list_image_url.Add(image);
                    }

                    return dto;
                },
                splitOn: "id,id"
            );

            return dict.Values.ToList();
        }
    }
}