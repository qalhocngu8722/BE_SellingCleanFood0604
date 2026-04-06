using Project_Selling_Clean_Food.DTOs;
using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.Repository
{
    public interface IOrdersRepo
    {
        Task<orders> GetByIDAsync(int id);
        Task<List<orders>> GetAllAsync();
        Task<int> UpdateAsync(UpdOrders order, int id);
        Task<int> DeleteAsync(int id);
        Task<int> AddnewAsync(orders order);
        Task<List<orders>> Get_top_10_Current_order();
        Task<BasicAnalyst> Get_Base_Analyst();
        Task<List<OrderListDTO>> GetOrderListByUserIdAsync(int userId);
        Task<List<GetListDetailOrderStaff>> GetListDetailOrder_Staff();
        Task<int> UpdateOrderPaymentStatus(int orderId, string paymentStatus, string orderStatus);

    }
}