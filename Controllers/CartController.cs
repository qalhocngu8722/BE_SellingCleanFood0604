using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Selling_Clean_Food.DTOs;
using Project_Selling_Clean_Food.Model;
using Project_Selling_Clean_Food.Repository;

namespace Project_Selling_Clean_Food.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartRepo _cartItemRepo;
        public CartController(ICartRepo cartItemRepo)
        {
            _cartItemRepo = cartItemRepo;
        }

        [HttpGet("GetAllCart")]
        public async Task<ActionResult<List<cart>>> GetAllCartAsync()
        {
            var res = await _cartItemRepo.GetAllAsync();
            if (res.Count > 0)
            {
                return Ok(res);
            }
            return BadRequest("Danh sach cart dang rong vui long them moi cart");
        }
        [HttpGet("GetCartByID")]
        public async Task<ActionResult<cart>> GetCartbyID(int id)
        {
            var user = await _cartItemRepo.GetByIDAsync(id);
            if (user == null)
            {
                return BadRequest("cart khong ton tai");
            }
            return Ok(user);
        }
        [HttpDelete("DeleteCart")]
        public async Task<ActionResult<int>> DeleteCart(int id)
        {
            var affected_row = await _cartItemRepo.DeleteAsync(id);
            if (affected_row > 0)
            {
                return Ok("Xoa cart thanh cong");
            }
            return BadRequest("Xoa cart khong thanh cong");
        }
        [HttpGet("Get_List_Product_inCart")]
        public async Task<ActionResult<List<CartListDTO>>> GetCartListByUserIdAsync(int id)
        {
            var res = await _cartItemRepo.GetCartListByUserIdAsync(id);
            if (res.Count > 0)
            {
                return Ok(res);
            }
            return Ok(new List<CartListDTO>(){ });
        }
    }
}
