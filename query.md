-- thống kê doanh thu theo ngày tại tháng hiện tại 
-- class,repo,controller sử dụng là liên quan đến : orders 
select * from orders 
where order_status = 'resolve' and EXTRACT(month from created_at) = extract(month from now())
order by order_date asc  
-- thống kê doanh thu theo người dùng
-- class,repo,controller sử dụng là liên quan đến : orders 
select user_id,sum(total_amount) as tongtien from orders
group by user_id

-- thống kê doanh thu theo tháng trong năm hiện tại 
-- class,repo,controller sử dụng là liên quan đến : orders 
select extract(month from created_at) as thang,sum(total_amount) as total_amount_month from orders 
where order_status = 'resolve' and EXTRACT(year from created_at) = extract(year from now())
group by extract(month from created_at)

Yêu cầu : đọc toàn bộ nội dùng và các file code có liên quan (có thể thêm,  mới tương ứng ) và giúp tôi tạo thêm code ở 3 nơi là : 
class,repo,controller thât chuẩn và có thể hoạt động được không lỗi 