CREATE DATABASE management_selling_vegetables;

CREATE TYPE role_type AS ENUM ('admin','staff','user');
CREATE TYPE status_payment_type AS ENUM ('pending','resolve','reject');
CREATE TYPE  order_status_type AS ENUM ('pending','resolve','reject','approve');

-- 1. Bảng Users
CREATE TABLE Users (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    email VARCHAR(150) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    role role_type NOT NULL, -- 'admin','staff','user'
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 2. Bảng Product
CREATE TABLE Products ( -- lấy từ trong kho ra bán 
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    price NUMERIC(12,2) NOT NULL,
    unit VARCHAR(50), -- lấy từ inventory 
    category_id int REFERENCES Product_Category(id),
    origin VARCHAR(100),
    food_type VARCHAR(50),
    quantity int not null,
    size VARCHAR(50),
    usage_instructions TEXT,
    storage_instructions TEXT,
    hsd DATE, -- hạn sử dụng ( bằng ngày nhập kho + 12days ) 
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 3. Bảng Product_Category
CREATE TABLE Product_Category (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT
);

-- 4. Bảng Product_Image
CREATE TABLE Product_Image (
    id SERIAL PRIMARY KEY,
    product_id int REFERENCES Products(id) ON DELETE CASCADE,
    image_url TEXT NOT NULL,
    is_primary BOOLEAN DEFAULT FALSE -- 'true' / 'false'
);


-- 5. Bảng Cart
CREATE TABLE Cart (
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES Users(id) ON DELETE CASCADE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 6. Bảng Cart_Item
CREATE TABLE Cart_Item (
    id SERIAL PRIMARY KEY,
    cart_id INTEGER REFERENCES Cart(id) ON DELETE CASCADE,
    product_id INTEGER REFERENCES Products(id),
    quantity INTEGER NOT NULL DEFAULT 1,
    added_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 7. Bảng Orders
CREATE TABLE Orders (
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES Users(id),
    order_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    total_amount NUMERIC(12,2) NOT NULL,
    payment_method VARCHAR(50),
    payment_status VARCHAR(20) DEFAULT 'unpaid',
    order_status order_status_type DEFAULT 'pending',
    shipping_address TEXT,
    note TEXT,
    recipient_name VARCHAR(100) NOT NULL,
    phone VARCHAR(20) NOT NULL,
    address TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- 8. Bảng Order_Item
CREATE TABLE Order_Item (
    id SERIAL PRIMARY KEY,
    order_id INTEGER REFERENCES Orders(id) ON DELETE CASCADE,
    product_id INTEGER REFERENCES Products(id),
    quantity INTEGER NOT NULL,
    unit_price NUMERIC(12,2) NOT NULL,
    subtotal NUMERIC(12,2) GENERATED ALWAYS AS (quantity * unit_price) STORED
);

-- 9. Bảng Payment_Transaction
CREATE TABLE Payment_Transaction (
    id SERIAL PRIMARY KEY,
    order_id INTEGER REFERENCES Orders(id) ON DELETE CASCADE,
    amount NUMERIC(12,2) NOT NULL,
    payment_method VARCHAR(50),
    transaction_id VARCHAR(100),
    status VARCHAR(20) DEFAULT 'pending',
    paid_at TIMESTAMP
);

-- Chỉ mục bổ sung cho hiệu năng tìm kiếm
CREATE INDEX idx_product_category ON Products(category_id);
CREATE INDEX idx_order_user ON Orders(user_id);
CREATE INDEX idx_cart_user ON Cart(user_id);