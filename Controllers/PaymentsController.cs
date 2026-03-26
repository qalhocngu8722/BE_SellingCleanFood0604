using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project_Selling_Clean_Food.Model;
using Project_Selling_Clean_Food.Repository;

namespace project_selling_clean_food.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class paymentscontroller : ControllerBase
    {
        private readonly IPaymentTransactionRepo _paymentTransactionRepo;
        public paymentscontroller(IPaymentTransactionRepo paymenttransactionrepo)
        {
            _paymentTransactionRepo = paymenttransactionrepo;
        }
        [HttpGet("paymenttransaction/getbyid/{id}")]
        public async Task<ActionResult<IPaymentTransactionRepo>> getbyidasync(int id)
        {
            var payment = await _paymentTransactionRepo.GetByIDAsync(id);
            if (payment == null)
                return NotFound("không tìm thấy payment transaction");
            return Ok(payment);
        }
        [HttpGet("paymenttransaction/getall")]
        public async Task<ActionResult<List<payment_transaction>>> getallasync()
        {
            var list = await _paymentTransactionRepo.GetAllAsync();
            if (list.Count == 0)
                return NotFound("danh sách payment transaction rỗng");
            return Ok(list);
        }
        [HttpPost("paymenttransaction/addnew")]
        public async Task<ActionResult> addasync(payment_transaction p)
        {
            var id = await _paymentTransactionRepo.AddnewAsync(p);
            if (id > 0)
            {
                p.id = (int)id;
                return Ok("thêm mới thành công");
            }
            return BadRequest("thêm mới payment transaction không thành công");
        }
        [HttpDelete("paymenttransaction/delete")]
        public async Task<ActionResult<int>> deleteasync(int id)
        {
            var affected = await _paymentTransactionRepo.DeleteAsync(id);
            if (affected > 0)
                return Ok("xóa thành công");
            return BadRequest("xóa payment transaction không thành công");
        }
        // làm 1 hàm update detail đễ 
    }
}
