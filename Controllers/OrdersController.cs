using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Selling_Clean_Food.DTOs;
using Project_Selling_Clean_Food.Model;
using Project_Selling_Clean_Food.Repository;

namespace Project_Selling_Clean_Food.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersRepo _ordersRepo;
        public OrdersController(IOrdersRepo ordersRepo)
        {
            _ordersRepo = ordersRepo;
        }

        [HttpGet("Orders/GetTop10CurrentOrder")]
        public async Task<ActionResult<List<orders>>> GetTop10CurrentOrder()
        {
            var list = await _ordersRepo.Get_top_10_Current_order();
            if (list == null || list.Count == 0)
                return NotFound("Không có đơn hàng nào");
            return Ok(list);
        }
        [HttpGet("Orders/getbyid/{id}")]
        public async Task<ActionResult<orders>> GetByIDAsync(int id)
        {
            var order = await _ordersRepo.GetByIDAsync(id);
            if (order == null)
                return NotFound("Không tìm thấy đơn hàng");
            return Ok(order);
        }
        [HttpGet("Orders/Getall")]
        public async Task<ActionResult<List<orders>>> GetAllAsync()
        {
            var list = await _ordersRepo.GetAllAsync();
            if (list.Count == 0)
                return NotFound("Danh sách đơn hàng rỗng");
            return Ok(list);
        }
        [HttpPost("Orders/Addnew")]
        public async Task<ActionResult> AddAsync(orders o)
        {
            var id = await _ordersRepo.AddnewAsync(o);
            if (id > 0)
            {
                o.id = id;
                return Ok(id);
            }
            return BadRequest("Thêm mới đơn hàng không thành công");
        }
        [HttpPut("Orders/UpdateOrder")]
        public async Task<ActionResult<int>> UpdateAsync(UpdOrders o, int id)
        {
            var affected = await _ordersRepo.UpdateAsync(o, id);
            if (affected > 0)
                return Ok("Sửa thành công");
            return BadRequest("Sửa thông tin đơn hàng không thành công");
        }
        [HttpDelete("Orders/DeleteOrder")]
        public async Task<ActionResult<int>> DeleteAsync(int id)
        {
            var affected = await _ordersRepo.DeleteAsync(id);
            if (affected > 0)
                return Ok("Xóa thành công");
            return BadRequest("Xóa đơn hàng không thành công");
        }
        [HttpGet("Base_analysth")]
        public async Task<ActionResult<BasicAnalyst>> Get_Base_Analyst()
        {
            var res = await _ordersRepo.Get_Base_Analyst();
            if(res == null)
            {
                return BadRequest("Không thể tải lên dữ liệu");
            }
            return Ok(res);
        }
        [HttpGet("List_Order_ofUsers")]
        public async Task<ActionResult<List<OrderListDTO>>> GetOrderListByUserIdAsync(int id)
        {
            var res = await _ordersRepo.GetOrderListByUserIdAsync(id);
            if (res == null)
            {
                return BadRequest("Không thể tải lên dữ liệu");
            }
            return Ok(res);
        }

        [HttpGet("ListDetailOrder_Staff")]
        public async Task<ActionResult<List<GetListDetailOrderStaff>>> GetListDetailOrder_Staff()
        {
            var res = await _ordersRepo.GetListDetailOrder_Staff();
            if (res == null || res.Count == 0)
            {
                return NotFound("Không có đơn hàng chi tiết nào");
            }
            return Ok(res);
        }
        [HttpPut("Update_status_order")]
        public async Task<ActionResult<int>> UpdateOrderPaymentStatus(int orderId, string? payment_status, string? order_status)
        {
            var res = await _ordersRepo.UpdateOrderPaymentStatus(orderId, payment_status, order_status);
            if(res > 0)
            {
                return Ok("Cập Nhật Thành Công");
            }
            return BadRequest("Cập Nhật Fail");
        }
    }
}
