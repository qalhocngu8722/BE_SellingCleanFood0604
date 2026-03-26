using Microsoft.AspNetCore.Mvc;
using Project_Selling_Clean_Food.DTOs;
using Project_Selling_Clean_Food.Model;
using Project_Selling_Clean_Food.Repository;
using Project_Selling_Clean_Food.Services;

namespace Project_Selling_Clean_Food.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VnPayController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;
        private readonly IOrdersRepo _ordersRepo;
        private readonly IPaymentTransactionRepo _paymentTransactionRepo;

        public VnPayController(
            IVnPayService vnPayService,
            IOrdersRepo ordersRepo,
            IPaymentTransactionRepo paymentTransactionRepo)
        {
            _vnPayService = vnPayService;
            _ordersRepo = ordersRepo;
            _paymentTransactionRepo = paymentTransactionRepo;
        }

        /// <summary>
        /// Bước 1: Frontend gọi để lấy URL thanh toán VNPay.
        /// Sau khi nhận paymentUrl, frontend tự redirect người dùng tới URL đó.
        /// VNPay sẽ redirect browser thẳng về ReturnUrl (trang frontend) sau khi thanh toán.
        /// </summary>
        [HttpPost("CreatePaymentUrl")]
        public async Task<ActionResult> CreatePaymentUrl([FromBody] PaymentInformationModel model)
        {
            if (model.OrderId <= 0)
                return BadRequest("OrderId không hợp lệ");

            var order = await _ordersRepo.GetByIDAsync(model.OrderId);
            if (order == null)
                return NotFound("Không tìm thấy đơn hàng");

            if (order.payment_status == "paid")
                return BadRequest("Đơn hàng đã được thanh toán");

            if (model.Amount <= 0)
                model.Amount = (double)order.total_amount;

            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);
            return Ok(new { paymentUrl = url });
        }

        /// <summary>
        /// Bước 2: Sau khi VNPay redirect về trang FE, FE forward toàn bộ query string lên đây.
        /// BE xác thực chữ ký, cập nhật DB, trả về JSON kết quả cho FE hiển thị.
        /// </summary>
        [HttpPost("VerifyPayment")]
        public async Task<ActionResult<PaymentResponseModel>> VerifyPayment()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);

            if (response == null)
                return BadRequest(new { message = "Không thể xử lý kết quả thanh toán" });

            if (response.Success && response.VnPayResponseCode == "00")
            {
                if (int.TryParse(response.OrderId, out int orderId) && orderId > 0)
                {
                    var order = await _ordersRepo.GetByIDAsync(orderId);
                    if (order != null && order.payment_status != "paid")
                    {
                        await _ordersRepo.UpdateOrderPaymentStatus(orderId, "paid", "confirmed");

                        var transaction = new payment_transaction
                        {
                            order_id = orderId,
                            amount = order.total_amount,
                            payment_method = "VnPay",
                            transaction_id = response.TransactionId,
                            status = "success",
                            paid_at = DateTime.Now
                        };
                        try { await _paymentTransactionRepo.AddnewAsync(transaction); }
                        catch { }
                    }
                }
                return Ok(new { success = true, message = "Thanh toán thành công", data = response });
            }
            else
            {
                if (int.TryParse(response.OrderId, out int orderId) && orderId > 0)
                {
                    var transaction = new payment_transaction
                    {
                        order_id = orderId,
                        amount = 0,
                        payment_method = "VnPay",
                        transaction_id = response.TransactionId ?? "",
                        status = "failed",
                        paid_at = DateTime.Now
                    };
                    try { await _paymentTransactionRepo.AddnewAsync(transaction); }
                    catch { }
                }
                return Ok(new { success = false, message = "Thanh toán thất bại", code = response.VnPayResponseCode, data = response });
            }
        }
    }
}
