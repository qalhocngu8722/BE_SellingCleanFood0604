using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Selling_Clean_Food.DTOs;
using Project_Selling_Clean_Food.Model;
using Project_Selling_Clean_Food.Repository;

namespace Project_Selling_Clean_Food.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DanhMucSPController : ControllerBase
    {
        private readonly IProductCategoryRepo _productCategoryRepo;
        public DanhMucSPController(IProductCategoryRepo productCategoryRepo)
        {
            _productCategoryRepo = productCategoryRepo;
        }
        [HttpGet("ProductCategory/getbyid/{id}")]
        public async Task<ActionResult<Product_Category>> GetByIDAsync(int id)
        {
            var category = await _productCategoryRepo.GetByIDAsync(id);
            if (category == null)
                return NotFound("Không tìm thấy danh mục sản phẩm");
            return Ok(category);
        }
        [HttpGet("ProductCategory/Getall")]
        public async Task<ActionResult<List<Product_Category>>> GetAllAsync()
        {
            var list = await _productCategoryRepo.GetAllAsync();
            if (list.Count == 0)
                return NotFound("Danh sách danh mục sản phẩm rỗng");
            return Ok(list);
        }
        [HttpPost("ProductCategory/Addnew")]
        public async Task<ActionResult> AddAsync(Product_Category p)
        {
            var id = await _productCategoryRepo.AddnewAsync(p);
            if (id > 0)
            {
                p.id = (int)id;
                return Ok("Thêm mới thành công");
            }
            return BadRequest("Thêm mới danh mục sản phẩm không thành công");
        }
        [HttpPut("ProductCategory/UpdateProductCategory")]
        public async Task<ActionResult<int>> UpdateAsync(Product_Category p, int id)
        {
            var affected = await _productCategoryRepo.UpdateAsync(p, id);
            if (affected > 0)
                return Ok("Sửa thành công");
            return BadRequest("Sửa thông tin danh mục sản phẩm không thành công");
        }
        [HttpDelete("ProductCategory/DeleteProductCategory")]
        public async Task<ActionResult<int>> DeleteAsync(int id)
        {
            var affected = await _productCategoryRepo.DeleteAsync(id);
            if (affected > 0)
                return Ok("Xóa thành công");
            return BadRequest("Xóa danh mục sản phẩm không thành công");
        }
    }
}
