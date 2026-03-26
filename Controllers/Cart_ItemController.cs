using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Selling_Clean_Food.Model;
using Project_Selling_Clean_Food.Repository;

namespace Project_Selling_Clean_Food.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Cart_ItemController : ControllerBase
    {
        private readonly ICartItemRepo _cartItemRepo;
        public Cart_ItemController(ICartItemRepo cartItemRepo)
        {
            _cartItemRepo = cartItemRepo;
        }
        [HttpGet("CartItem/getbyid/{id}")]
        public async Task<ActionResult<cart_item>> GetByIDAsync(int id)
        {
            var item = await _cartItemRepo.GetByIDAsync(id);
            if(item == null)
                return NotFound("Không tìm thấy cart item");
            return Ok(item);
        }
        [HttpGet("CartItem/Getall")]
        public async Task<ActionResult<List<cart_item>>> GetAllAsync()
        {
            var list = await _cartItemRepo.GetAllAsync();
            if (list.Count == 0)
                return NotFound("Danh sách cart item rỗng");
            return Ok(list);
        }
        [HttpPost("CartItem/Addnew")]
        public async Task<ActionResult> AddnewAsync(cart_item c)
        {
            var id = await _cartItemRepo.AddnewAsync(c);
            if(id > 0)
            {
                c.id = (int)id;
                return Ok("Thêm mới thành công");
            }
            return BadRequest("Thêm mới cart item không thành công");
        }
        [HttpPut("CartItem/UpdateCartItem")]
        public async Task<ActionResult<int>> UpdateAsync(cart_item c, int id)
        {
            var affected = await _cartItemRepo.UpdateAsync(c, id);
            if (affected > 0)
                return Ok("Sửa thành công");
            return BadRequest("Sửa thông tin cart item không thành công");
        }
        [HttpDelete("CartItem/DeleteCartItem")]
        public async Task<ActionResult<int>> DeleteAsync(int id)
        {
            var affected = await _cartItemRepo.DeleteAsync(id);
            if (affected > 0)
                return Ok("Xóa thành công");
            return BadRequest("Xóa cart item không thành công");
        }
    }
}
