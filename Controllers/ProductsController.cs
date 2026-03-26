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
        [HttpPut("UpdateSanPham")]
        public async Task<ActionResult<int>> UpdateDetailProduct(int idsp, [FromBody] UpdSanPham udp_sp)
        {
            var dtpd = await _sellingcleanfood.GetDetailProduct(idsp);
            if (dtpd == null)
            {
                return NotFound("Khong tim thay user nay");
            }

            dtpd.name_sp = (udp_sp.name_sp == null) ? dtpd.name_sp : udp_sp.name_sp;


            dtpd.img_sp = (udp_sp.img_sp.Count == 0) ? dtpd.img_sp : udp_sp.img_sp;
            dtpd.price = udp_sp.price ?? dtpd.price;
            dtpd.iddm = udp_sp.iddm ?? dtpd.iddm;
            dtpd.donvi = (udp_sp.donvi == null) ? dtpd.donvi : udp_sp.donvi;
            dtpd.decription = (udp_sp.decription == null) ? dtpd.decription : udp_sp.decription;
            dtpd.soluong = udp_sp.soluong ?? dtpd.soluong;
            try
            {
                var affected_row = await _sellingcleanfood.UpdateDetailProduct(idsp,dtpd);
                if (affected_row > 0)
                {
                    return Ok("update thanh cong");
                }
                return BadRequest("update that bai");
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }
        [HttpDelete("DeleteUser")]
        public async Task<ActionResult<int>> DeleteUser(int id)
        {
            var affected_row = await _sellingcleanfood.DeleteAsync(id);
            if (affected_row > 0)
            {
                return Ok("Xoa nguoi dung thanh cong");
            }
            return BadRequest("Xoa Khong Thanh Cong");
        }
        [HttpGet("Get_Detail_Product")]
        public async Task<ActionResult<DetailProducts>> Get_Detail_Products(int id)
        {
            try
            {
                var dtsp = await _sellingcleanfood.Get_Detail_Products(id);
                if(dtsp is null)
                {
                    return NotFound("Không thể tải sản phẩm");
                }
                return Ok(dtsp);
            }
            catch(Exception ex) { return BadRequest(); }
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
            [FromForm] products p,
            IFormFile? primary_img,
            List<IFormFile>? pi)
        {
            // Validate product name
            if (string.IsNullOrWhiteSpace(p.name))
                return BadRequest("Vui lòng điền vào tên sản phẩm");

            var validImgTypes = new List<string> { "image/jpeg", "image/png" };
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "imgs");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // Xử lý ảnh chính
            string? mainImgUrl = null;
            if (primary_img != null)
            {
                if (!validImgTypes.Contains(primary_img.ContentType))
                    return BadRequest("Ảnh chính tải lên không hợp lệ");
                if (primary_img.Length > 0)
                {
                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(primary_img.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await primary_img.CopyToAsync(stream);
                    }
                    mainImgUrl = $"/uploads/imgs/{uniqueFileName}";
                }
            }

            // Xử lý các ảnh phụ
            var listProductImages = new List<product_image>();
            if (pi != null && pi.Count > 0)
            {
                foreach (var img in pi)
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
                name = p.name,
                description = p.description,
                price = p.price,
                unit = p.unit,
                category_id = p.category_id,
                origin = p.origin,
                food_type = p.food_type,
                quantity = p.quantity,
                size = p.size,
                usage_instructions = p.usage_instructions,
                storage_instructions = p.storage_instructions,
                hsd = p.hsd.Value,
                created_at = DateTime.Now,
                images = listProductImages
            };

            try
            {
                var productId = await _sellingcleanfood.Add_new_Detail_product(detailProduct);
                if(productId == 0)
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
            if(list.Count == 0)
            {
                return NotFound("Danh sách sản phẩm trống");
            }
            return Ok(list);
        }
        [HttpGet("Get_Detail_Product_With_Related")]
        public async Task<ActionResult<DetailProductWithRelatedDTO>> GetDetailProductWithRelatedAsync(int id)
        {
            var res = await _sellingcleanfood.GetDetailProductWithRelatedAsync(id);
            if(res == null)
            {
                return BadRequest("Không tìm thấy sản phẩm");
            }
            return Ok(res);
        }
    }
}
