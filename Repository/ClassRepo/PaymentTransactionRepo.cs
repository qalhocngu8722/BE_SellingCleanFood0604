using Project_Selling_Clean_Food.Model;

namespace Project_Selling_Clean_Food.Repository
{
    public class PaymentTransactionRepo : BaseRepo, IPaymentTransactionRepo
    {
        public PaymentTransactionRepo(IConfiguration configuration) : base(configuration) { }

        public async Task<payment_transaction> GetByIDAsync(int id)
        {
            return await GetByIDAsync<payment_transaction>(id);
        }

        public async Task<List<payment_transaction>> GetAllAsync()
        {
            return await GetAllAsync<payment_transaction>();
        }

        public async Task<int> UpdateAsync(payment_transaction paymentTransaction, int id)
        {
            return await UpdateAsync<payment_transaction>(paymentTransaction, id);
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await DeleteAsync<payment_transaction>(id);
        }

        public async Task<int> AddnewAsync(payment_transaction paymentTransaction)
        {
            return await AddnewAsync<payment_transaction>(paymentTransaction);
        }
    }
}