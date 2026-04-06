using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Selling_Clean_Food.DTOs;
using Project_Selling_Clean_Food.Model;
using Project_Selling_Clean_Food.Repository;
using System.Linq;

namespace Project_Selling_Clean_Food.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SanPhamController : ControllerBase
    {
        private readonly IProductsRepo _sellingcleanfood;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SanPhamController(IProductsRepo sellingcleanfood, IWebHostEnvironment webHostEnvironment)
        {
            _sellingcleanfood = sellingcleanfood;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("GetAllSanPham")]
        public async Task<ActionResult<List<products>>> GetAllSanPhamAsync()
        {
            var res = await _sellingcleanfood.GetAllAsync();
            if (res.Count > 0)
            {
                return Ok(res);
            }
            return BadRequest("Danh sach SanPham dang rong vui long them moi SanPham");
        }
        [HttpGet("GetSanPhamByID")]
        public async Task<ActionResult<products>> GetSanPhambyID(int id)
        {
            var user = await _sellingcleanfood.GetByIDAsync(id);
            if (user == null)
            {
                return BadRequest("SanPham khong ton tai");
            }
            return Ok(user);
        }
        [HttpPost("AddnewSanPham")]
        public async Task<ActionResult> AddnewSanPham(products u)
        {
            try
            {
                var res = await _sellingcleanfood.AddnewAsync(u);
                if (res > 0)
                {
                    u.id = res;
                    return Ok("Them thanh cong");
                }
                return BadRequest("Them That Bai");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("UpdateDetailProduct")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> UpdateDetailProduct(int id,[FromForm] UpdSanPham request)
        {
            try
            {
                // ✅ Validate: Product must exist
                var existingProduct = await _sellingcleanfood.GetByIDAsync(id);
                if (existingProduct == null)
                {
                    return BadRequest($"Sản phẩm với ID {id} không tồn tại");
                }

                var updSanPham = new BaseTypeFieldUpdProduct
                {
                    name = request.name,
                    description = request.description,
                    price = request.price,
                    unit = request.unit,
                    category_id = request.category_id,
                    origin = request.origin,
                    food_type = request.food_type,
                    quantity = request.quantity,
                    size = request.size,
                    usage_instructions = request.usage_instructions,
                    storage_instructions = request.storage_instructions,
                    hsd = request.hsd
                };

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "imgs");

                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var validImgTypes = new List<string> { "image/jpeg", "image/png" };

                var listProductImages = new List<product_image>();

                var new_img_primary = "";

                // ✅ Process primary image
                if (request.primary_img_file != null && request.primary_img_file.Length > 0)
                {
                    if (!validImgTypes.Contains(request.primary_img_file.ContentType))
                        return BadRequest("Ảnh chính không hợp lệ - chỉ chấp nhận JPEG, PNG");

                    if (request.primary_img_file.Length > 5 * 1024 * 1024)  // 5MB max
                        return BadRequest("Ảnh chính quá lớn - tối đa 5MB");

                    try
                    {
                        var uniqueFileName = Guid.NewGuid() + Path.GetExtension(request.primary_img_file.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using var stream = new FileStream(filePath, FileMode.Create);
                        await request.primary_img_file.CopyToAsync(stream);

                        new_img_primary = $"/uploads/imgs/{uniqueFileName}";
                        Console.WriteLine($"✅ Primary image saved: {new_img_primary}");
                    }
                    catch (Exception fileEx)
                    {
                        return BadRequest($"Lỗi lưu ảnh chính: {fileEx.Message}");
                    }
                }

                // ✅ Process secondary images
                if (request.images != null && request.images.Count > 0)
                {
                    foreach (var img in request.images)
                    {
                        if (!validImgTypes.Contains(img.ContentType))
                            return BadRequest("Một hoặc nhiều ảnh phụ không hợp lệ - chỉ chấp nhận JPEG, PNG");

                        if (img.Length > 5 * 1024 * 1024)
                            return BadRequest($"Ảnh phụ quá lớn - tối đa 5MB");

                        try
                        {
                            var uniqueFileName = Guid.NewGuid() + Path.GetExtension(img.FileName);
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using var stream = new FileStream(filePath, FileMode.Create);
                            await img.CopyToAsync(stream);

                            listProductImages.Add(new product_image
                            {
                                image_url = $"/uploads/imgs/{uniqueFileName}",
                                is_primary = false
                            });
                            Console.WriteLine($"✅ Secondary image saved: /uploads/imgs/{uniqueFileName}");
                        }
                        catch (Exception fileEx)
                        {
                            return BadRequest($"Lỗi lưu ảnh phụ: {fileEx.Message}");
                        }
                    }
                }

                // ✅ FIX: Repo now handles the logic correctly
                var affected = await _sellingcleanfood.UpdateDetailProduct(
                    id,
                    updSanPham,
                    new_img_primary,
                    listProductImages,
                    !string.IsNullOrEmpty(new_img_primary)
                );

                if (affected > 0)
                {
                    Console.WriteLine($"✅ Product {id} updated successfully");
                    return Ok(new { message = "Cập nhật sản phẩm thành công", productId = id });
                }

                return BadRequest(new { message = "Cập nhật sản phẩm thất bại", productId = -1 });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Exception in UpdateDetailProduct: {ex.Message}\n{ex.StackTrace}");
                return BadRequest($"Lỗi cập nhật sản phẩm: {ex.Message}");
            }
        }
        [HttpDelete("DeleteProductbyID")]
        public async Task<ActionResult<int>> DeleteUser(int id)
        {
            var affected_row = await _sellingcleanfood.DeleteAsync(id);
            if (affected_row > 0)
            {
                return Ok("Xoa product thanh cong");
            }
            return BadRequest("Xoa product Khong Thanh Cong");
        }
        [HttpGet("Get_Detail_Product")]
        public async Task<ActionResult<DetailProducts>> Get_Detail_Products(int id)
        {
            try
            {
                var dtsp = await _sellingcleanfood.Get_Detail_Products(id);
                if (dtsp is null)
                {
                    return NotFound("Không thể tải sản phẩm");
                }
                return Ok(dtsp);
            }
            catch (Exception ex) { return BadRequest(); }
        }

        [HttpGet("Render_Product_sellwell_Dashbroads")]
        public async Task<ActionResult<List<Render_product_dashbroad>>> Render_Product_sellwell_Dashbroads()
        {
            try
            {
                var list = await _sellingcleanfood.Render_Product_sellwell_Dashbroads();
                if (list == null || list.Count == 0)
                    return NotFound("Không có sản phẩm bán chạy");
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi: {ex.Message}");
            }
        }
        [HttpPost("Add_new_Detail_Product")]
        public async Task<ActionResult> Add_new_Detail_product(
            [FromForm] UpdSanPham addnewpd)
        {
            // Validate product name
            if (string.IsNullOrWhiteSpace(addnewpd.name))
                return BadRequest("Vui lòng điền vào tên sản phẩm");

            var validImgTypes = new List<string> { "image/jpeg", "image/png" };
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "imgs");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Xử lý ảnh chính
            string? mainImgUrl = null;
            if (addnewpd.primary_img_file != null)
            {
                if (!validImgTypes.Contains(addnewpd.primary_img_file.ContentType))
                    return BadRequest("Ảnh chính tải lên không hợp lệ");
                if (addnewpd.primary_img_file.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(addnewpd.primary_img_file.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await addnewpd.primary_img_file.CopyToAsync(stream);
                    }
                    mainImgUrl = $"/uploads/imgs/{uniqueFileName}";
                }
            }

            // Xử lý các ảnh phụ
            var listProductImages = new List<product_image>();
            if (addnewpd.images != null && addnewpd.images.Count > 0)
            {
                foreach (var img in addnewpd.images)
                {
                    if (!validImgTypes.Contains(img.ContentType))
                        return BadRequest("Một hoặc nhiều ảnh phụ tải lên không hợp lệ");
                    if (img.Length > 0)
                    {
                        var uniqueFileName = Guid.NewGuid() + Path.GetExtension(img.FileName);
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await img.CopyToAsync(stream);
                        }
                        listProductImages.Add(new product_image
                        {
                            image_url = $"/uploads/imgs/{uniqueFileName}",
                            is_primary = false // Ảnh phụ
                        });
                    }
                }
            }

            // Thêm ảnh chính vào đầu danh sách (nếu có)
            if (!string.IsNullOrEmpty(mainImgUrl))
            {
                listProductImages.Insert(0, new product_image
                {
                    image_url = mainImgUrl,
                    is_primary = true
                });
            }

            // Gom dữ liệu DetailProducts
            var detailProduct = new DetailProducts
            {
                name = addnewpd.name,
                description = addnewpd.description,
                price = addnewpd.price.Value,
                unit = addnewpd.unit,
                category_id = addnewpd.category_id.Value,
                origin = addnewpd.origin,
                food_type = addnewpd.food_type,
                quantity = addnewpd.quantity.Value,
                size = addnewpd.size,
                usage_instructions = addnewpd.usage_instructions,
                storage_instructions = addnewpd.storage_instructions,
                hsd = addnewpd.hsd.Value,
                created_at = DateTime.Now,
                images = listProductImages
            };

            try
            {
                var productId = await _sellingcleanfood.Add_new_Detail_product(detailProduct);
                if (productId == 0)
                {
                    return BadRequest("Thêm mới sản phẩm thất bại");
                }
                return Ok("Thêm mới sản phẩm thành công");
            }
            catch (Exception ex)
            {
                return BadRequest($"Thêm mới sản phẩm thất bại: {ex.Message}");
            }
        }
        [HttpGet("Get_List_Product_homepage")]
        public async Task<ActionResult<List<RelatedProductDTO>>> Get_list_product_homepage()
        {
            var list = await _sellingcleanfood.Get_list_product_homepage();
            if (list.Count == 0)
            {
                return NotFound("Danh sách sản phẩm trống");
            }
            return Ok(list);
        }
        [HttpGet("Get_Detail_Product_With_Related")]
        public async Task<ActionResult<DetailProductWithRelatedDTO>> GetDetailProductWithRelatedAsync(int id)
        {
            var res = await _sellingcleanfood.GetDetailProductWithRelatedAsync(id);
            if (res == null)
            {
                return BadRequest("Không tìm thấy sản phẩm");
            }
            return Ok(res);
        }
        [HttpGet("Get_list_product_staff_interface")]
        public async Task<ActionResult<List<getListProduct_Staff>>> Get_list_product_staff_interface(){
            var listres = await _sellingcleanfood.Get_list_product_staff_interface();
            if (listres == null)
            {
                return BadRequest("Không tìm thấy sản phẩm");
            }
            return Ok(listres);

        }
    }
}
