-- 1. hiển thị detail product kèm 1 dây products liên quan bên dưới nữa

-- detail products ( tạo 1 DTO mới và xây dựng hàm để query và render ra chuẩn với  câu query trong file ProductRepo sau đó tiếp tục triển khai ở Interface của repo đó và cuối cùng là controller )

select p.*,pi.image_url,pc."name" from products as p
join product_category as pc
on p.category_id = pc.id
join product_image as pi
on pi.product_id = p.id
where p.id = @id 

-- list product liên quan bên dưới (có thể làm 1 file với value là array)
select pc."name",p."name",pi.image_url,p.origin,p.food_type,p.price,p.unit from products as p
join product_category as pc
on p.category_id = pc.id
join product_image as pi
on pi.product_id = p.id
where pc.id = @id 


--2. lấy ra list đơn hàng và list giỏ hàng hiện tại của users

--2.1 carts ( tạo 1 DTO mới và xây dựng hàm để query và render ra chuẩn với câu query trong file CartRepo sau đó tiếp tục triển khai ở Interface của repo đó và cuối cùng là controller )

select name,price,unit,quantity from cart as c
join cart_item as ci 
on c.id = ci.cart_id
join products as p 
on ci.product_id = p.id
where c.user_id = @id 
  
--2.2 orders  ( tạo 1 DTO mới và xây dựng hàm để query và render ra chuẩn với câu query trong file OrdersRepo sau đó tiếp tục triển khai ở Interface của repo đó và cuối cùng là controller )

select o.id,o.payment_method,o.payment_status,o.order_status,o.order_date,pi.image_url,p."name",oi.quantity,oi.unit_price,o.shipping_fee_id from orders as o
join order_item as oi 
on o.id = oi.order_id
join products as p
on p.id = oi.product_id
join product_image as pi
on pi.product_id = p.id