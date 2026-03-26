using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.Repository
{
    public interface IPaymentTransactionRepo
    {
        Task<payment_transaction> GetByIDAsync(int id);
        Task<List<payment_transaction>> GetAllAsync();
        Task<int> UpdateAsync(payment_transaction paymentTransaction, int id);
        Task<int> DeleteAsync(int id);
        Task<int> AddnewAsync(payment_transaction paymentTransaction);
    }
}