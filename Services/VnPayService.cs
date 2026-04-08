using System.Net;
using System.Security.Cryptography;
using System.Text;

public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
{
    var vnp_Url = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
    var vnp_HashSecret = "SECRETKEY";

    var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
    var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);

    var vnp_Params = new SortedList<string, string>();

    vnp_Params.Add("vnp_Version", "2.1.0");
    vnp_Params.Add("vnp_Command", "pay");
    vnp_Params.Add("vnp_TmnCode", "DEMOV210");
    vnp_Params.Add("vnp_Amount", ((int)(model.Amount * 100)).ToString());
    vnp_Params.Add("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
    vnp_Params.Add("vnp_CurrCode", "VND");
    vnp_Params.Add("vnp_IpAddr", "192.168.1.1");
    vnp_Params.Add("vnp_Locale", "vn");
    vnp_Params.Add("vnp_OrderInfo", $"Thanh toan don hang {model.OrderId}");
    vnp_Params.Add("vnp_OrderType", "other");
    vnp_Params.Add("vnp_ReturnUrl", "https://fe-project-selling-clean-food-h42m.vercel.app/payment-return.html");
    vnp_Params.Add("vnp_TxnRef", DateTime.Now.Ticks.ToString());

    // Build query string
    var query = new StringBuilder();
    foreach (var kv in vnp_Params)
    {
        query.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
    }

    var queryString = query.ToString().TrimEnd('&');

    // Create hash
    var hash = HmacSHA512(vnp_HashSecret, queryString);

    var paymentUrl = $"{vnp_Url}?{queryString}&vnp_SecureHash={hash}";

    return paymentUrl;
}

private string HmacSHA512(string key, string inputData)
{
    var keyBytes = Encoding.UTF8.GetBytes(key);
    var inputBytes = Encoding.UTF8.GetBytes(inputData);

    using (var hmac = new HMACSHA512(keyBytes))
    {
        var hashValue = hmac.ComputeHash(inputBytes);
        return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
    }
}
