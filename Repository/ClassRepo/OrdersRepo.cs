using Dapper;
using Project_Selling_Clean_Food.Model;
using Project_Selling_Clean_Food.DTOs;

namespace Project_Selling_Clean_Food.Repository
{
    public class OrdersRepo : BaseRepo, IOrdersRepo
    {
        public OrdersRepo(IConfiguration configuration) : base(configuration) { }

        public async Task<orders> GetByIDAsync(int id)
        {
            return await GetByIDAsync<orders>(id);
        }

        public async Task<List<orders>> GetAllAsync()
        {
            return await GetAllAsync<orders>();
        }

        public async Task<int> UpdateAsync(UpdOrders order, int id)
        {
            return await UpdateAsync<UpdOrders>(order, id, "orders");
        }

        public async Task<int> DeleteAsync(int id)
        {
            return await DeleteAsync<orders>(id);
        }

        public async Task<int> AddnewAsync(orders order)
        {
            using var con = Getconnection();
            var query = $@"INSERT INTO Orders
                        (
                            user_id,
                            total_amount,
                            payment_method,
                            payment_status,
                            order_status,
                            shipping_address,
                            note,
                            recipient_name,
                            phone,
                            address
                        )
                        VALUES
                        (
                            @user_id,
                            @total_amount,
                            @payment_method,
                            @payment_status,
                            @order_status::order_status_type,
                            @shipping_address,
                            @Note,
                            @recipient_name,
                            @Phone,
                            @Address
                        ) RETURNING id;";
            var newid = await con.QuerySingleAsync<int>(query, order);
            return newid;
        }
        public async Task<List<orders>> Get_top_10_Current_order()
        {
            using var con = Getconnection();
            var query = $@"select * from orders
                        order by created_at DESC 
                        limit 10 ";
            var list = await con.QueryAsync<orders>(query);
            return list.ToList();
        }
        public async Task<BasicAnalyst> Get_Base_Analyst()
        {
            using var con = Getconnection();
            var query = $@"select 
                        (select sum(total_amount) as doanhthu from orders
                        where payment_status = 'paid' or order_status = 'resolve'),
                        (select count(*) as amount_od from orders),
                        (select count(*) as amount_us from users ),
                        (select count(*) as amount_pd from products)";
            var res = await con.QuerySingleOrDefaultAsync<BasicAnalyst>(query);
            return res;
        }

        public async Task<List<OrderListDTO>> GetOrderListByUserIdAsync(int userId)
        {
            using var con = Getconnection();
            var query = @"select o.id, o.payment_method, o.payment_status, o.order_status, o.order_date, pi.image_url, p.name, oi.quantity, oi.unit_price,o.total_amount
                        from orders as o
                        join order_item as oi on o.id = oi.order_id
                        join products as p on p.id = oi.product_id
                        join product_image as pi on pi.product_id = p.id
                        where o.user_id = @id and pi.is_primary is true";
            var result = await con.QueryAsync<OrderListDTO>(query, new { id = userId });
            return result.ToList();
        }

        public async Task<List<GetListDetailOrderStaff>> GetListDetailOrder_Staff()
        {
            using var con = Getconnection();
            var query = @"SELECT o.id, o.user_id, o.order_date, o.total_amount, o.payment_method,
                                 o.payment_status, o.order_status, o.shipping_address, o.note,
                                 o.recipient_name, o.phone, o.address, o.created_at,
                                 oi.id, oi.order_id, oi.product_id, oi.quantity, oi.unit_price,
                                 p.name AS product_name, pc.image_url AS product_image_url
                          FROM orders AS o
                          JOIN order_item AS oi ON o.id = oi.order_id
                          JOIN products AS p ON oi.product_id = p.id
                          JOIN product_image AS pc ON p.id = pc.product_id
                          WHERE pc.is_primary is true
                          ORDER BY o.created_at DESC";

            var orderDict = new Dictionary<int, GetListDetailOrderStaff>();

            await con.QueryAsync<GetListDetailOrderStaff, DetailOrderItem, GetListDetailOrderStaff>(
                query,
                (order, item) =>
                {
                    if (!orderDict.TryGetValue(order.id!.Value, out var entry))
                    {
                        entry = order;
                        entry.detailOrderItems = new List<DetailOrderItem>();
                        orderDict.Add(entry.id!.Value, entry);
                    }
                    if (item != null)
                    {
                        entry.detailOrderItems.Add(item);
                    }
                    return entry;
                },
                splitOn: "id"
            );

            return orderDict.Values.ToList();
        }

        public async Task<int> UpdateOrderPaymentStatus(int orderId, string paymentStatus, string orderStatus)
        {
            using var con = Getconnection();
            var query = @"UPDATE orders SET payment_status = @paymentStatus, order_status = @orderStatus::order_status_type WHERE id = @orderId";
            return await con.ExecuteAsync(query, new { orderId, paymentStatus, orderStatus });
        }

        public async Task<List<RevenueByDayDTO>> GetRevenueByDayCurrentMonth()
        {
            using var con = Getconnection();
            var query = @"SELECT date_trunc('day', created_at)::date AS day,count(*) as sodonhang, SUM(total_amount) AS total_amount
                          FROM orders
                          WHERE order_status = 'resolve'
                            AND EXTRACT(MONTH FROM created_at) = EXTRACT(MONTH FROM now())
                            AND EXTRACT(YEAR FROM created_at) = EXTRACT(YEAR FROM now())
                          GROUP BY date_trunc('day', created_at)::date
                          ORDER BY day ASC";
            var res = await con.QueryAsync<RevenueByDayDTO>(query);
            return res.ToList();
        }

        public async Task<List<RevenueByUserDTO>> GetRevenueByUser()
        {
            using var con = Getconnection();
            var query = @"SELECT user_id, SUM(total_amount) AS tongtien
                          FROM orders
                          GROUP BY user_id
                          ORDER BY tongtien DESC limit 10";
            var res = await con.QueryAsync<RevenueByUserDTO>(query);
            return res.ToList();
        }

        public async Task<List<RevenueByMonthDTO>> GetRevenueByMonthCurrentYear(int opt)
        {
            using var con = Getconnection();
            var query = @"SELECT CAST(EXTRACT(MONTH FROM created_at) AS int) AS thang, SUM(total_amount) AS total_amount_month
                          FROM orders
                          WHERE order_status = 'resolve'
                            AND EXTRACT(YEAR FROM created_at) = EXTRACT(YEAR FROM now())
                          GROUP BY CAST(EXTRACT(MONTH FROM created_at) AS int)
                          ORDER BY thang ASC";
            var query1 = @"SELECT CAST(EXTRACT(YEAR FROM created_at) AS int) AS thang, SUM(total_amount) AS total_amount_month
                          FROM orders
                          WHERE order_status = 'resolve'
                          GROUP BY CAST(EXTRACT(YEAR FROM created_at) AS int)
                          ORDER BY thang ASC";
            var res = await con.QueryAsync<RevenueByMonthDTO>(opt == 1 ? query1 : query);
            return res.ToList();
        }
    }
}