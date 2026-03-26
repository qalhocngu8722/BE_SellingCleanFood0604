using Microsoft.AspNetCore.Mvc;
using Project_Selling_Clean_Food.Repository;
using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageRepo _productImageRepo;
        public ProductImageController(IProductImageRepo productImageRepo)
        {
            _productImageRepo = productImageRepo;
        }
        [HttpGet("ProductImage/getbyid/{id}")]
        public async Task<ActionResult<product_image>> GetByIDAsync(int id)
        {
            var img = await _productImageRepo.GetByIDAsync(id);
            if(img == null)
                return NotFound("Không tìm thấy product image");
            return Ok(img);
        }
        [HttpGet("ProductImage/Getall")]
        public async Task<ActionResult<List<product_image>>> GetAllAsync()
        {
            var list = await _productImageRepo.GetAllAsync();
            if (list.Count == 0)
                return NotFound("Danh sách product image rỗng");
            return Ok(list);
        }
        [HttpPost("ProductImage/Addnew")]
        public async Task<ActionResult> AddnewAsync(product_image p)
        {
            var id = await _productImageRepo.AddnewAsync(p);
            if(id > 0)
            {
                p.idimg = (int)id;
                return Ok("Thêm mới thành công");
            }
            return BadRequest("Thêm mới product image không thành công");
        }
        [HttpPut("ProductImage/UpdateProductImage")]
        public async Task<ActionResult<int>> UpdateAsync(product_image p, int id)
        {
            var affected = await _productImageRepo.UpdateAsync(p, id);
            if (affected > 0)
                return Ok("Sửa thành công");
            return BadRequest("Sửa thông tin product image không thành công");
        }
        [HttpDelete("ProductImage/DeleteProductImage")]
        public async Task<ActionResult<int>> DeleteAsync(int id)
        {
            var affected = await _productImageRepo.DeleteAsync(id);
            if (affected > 0)
                return Ok("Xóa thành công");
            return BadRequest("Xóa product image không thành công");
        }
    }
}
