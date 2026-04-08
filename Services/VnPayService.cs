using Project_Selling_Clean_Food.DTOs;
using Project_Selling_Clean_Food.Helpers;

namespace Project_Selling_Clean_Food.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        public VnPayService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            // 🔥 Fix timezone chạy cả Windows + Linux
            TimeZoneInfo timeZone;

            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // Windows
            }
            catch
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh"); // Linux
            }

            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

            // 🔥 Tick theo timezone chuẩn
            var tick = timeNow.Ticks.ToString();

            // 🔥 Lấy config + check null
            var tmnCode = _configuration["Vnpay:TmnCode"];
            var hashSecret = _configuration["Vnpay:HashSecret"];
            var baseUrl = _configuration["Vnpay:BaseUrl"];
            var returnUrl = _configuration["Vnpay:ReturnUrl"];
            var version = _configuration["Vnpay:Version"];
            var command = _configuration["Vnpay:Command"];
            var currCode = _configuration["Vnpay:CurrCode"];
            var locale = _configuration["Vnpay:Locale"];

            if (string.IsNullOrEmpty(tmnCode) ||
                string.IsNullOrEmpty(hashSecret) ||
                string.IsNullOrEmpty(baseUrl) ||
                string.IsNullOrEmpty(returnUrl))
            {
                throw new Exception("Missing VNPay configuration!");
            }

            var pay = new VnPayLibrary();

            pay.AddRequestData("vnp_Version", version ?? "2.1.0");
            pay.AddRequestData("vnp_Command", command ?? "pay");
            pay.AddRequestData("vnp_TmnCode", tmnCode);
            pay.AddRequestData("vnp_Amount", ((int)(model.Amount * 100)).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", currCode ?? "VND");
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", locale ?? "vn");
            pay.AddRequestData("vnp_OrderInfo", $"Thanh toán đơn hàng {model.OrderId}");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", returnUrl);
            pay.AddRequestData("vnp_TxnRef", $"{model.OrderId}_{tick}");

            var paymentUrl = pay.CreateRequestUrl(baseUrl, hashSecret);

            return paymentUrl;
        }

        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var hashSecret = _configuration["Vnpay:HashSecret"];

            if (string.IsNullOrEmpty(hashSecret))
            {
                throw new Exception("Missing Vnpay:HashSecret config");
            }

            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(collections, hashSecret);

            return response;
        }
    }
}