using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Selling_Clean_Food.Repository;
using Project_Selling_Clean_Food.Model;


namespace Project_Selling_Clean_Food.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Order_Item_Controller : ControllerBase
    {
        private readonly IOrderItemRepo _orderItemRepo;
        public Order_Item_Controller(IOrderItemRepo orderItemRepo)
        {
            _orderItemRepo = orderItemRepo;
        }
        [HttpGet("GetbyID")]
        public async Task<ActionResult<Order_Item>> GetByIDAsync(int id)
        {
            var res = await _orderItemRepo.GetByIDAsync(id);
            if(res == null)
            {
                return NotFound("Khong tim thay");
            }
            return Ok(res);
        }
        [HttpGet("GetAll")]
        public async Task<ActionResult<List<Order_Item>>> GetAllAsync()
        {
            var res = await _orderItemRepo.GetAllAsync();
            return Ok(res);
        }


        [HttpPost("AddNew")]
        public async Task<ActionResult<int>> AddNewAsync([FromBody] Order_Item orderItem)
        {
            var result = await _orderItemRepo.AddnewAsync(orderItem);
            return Ok(result);
        }

        [HttpPut("Update")]
        public async Task<ActionResult<int>> UpdateAsync([FromBody] Order_Item orderItem, int id)
        {
            var result = await _orderItemRepo.UpdateAsync(orderItem, id);
            return Ok(result);
        }

        [HttpDelete("Delete")]
        public async Task<ActionResult<int>> DeleteAsync(int id)
        {
            var result = await _orderItemRepo.DeleteAsync(id);
            return Ok(result);
        }
    }
}
