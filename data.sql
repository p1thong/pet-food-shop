-- 1️⃣ CREATE DATABASE
CREATE DATABASE petfoodshop;

-- 2️⃣ USERS (Admin + Customer)
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    email VARCHAR(255) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    fullname VARCHAR(150),
    phone VARCHAR(30),
    address VARCHAR(255),
    role VARCHAR(20) NOT NULL DEFAULT 'customer',   -- 'admin' or 'customer'
    isdeleted BOOLEAN NOT NULL DEFAULT FALSE,       -- soft delete flag
    createdat TIMESTAMP DEFAULT NOW(),
    updatedat TIMESTAMP DEFAULT NOW()
);

-- 3️⃣ CATEGORIES
CREATE TABLE categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(120) NOT NULL,
    description VARCHAR(255)
);

-- 4️⃣ PRODUCTS
CREATE TABLE products (
    id SERIAL PRIMARY KEY,
    sku VARCHAR(100) UNIQUE,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    price NUMERIC(12,2) NOT NULL,
    stock INT DEFAULT 0,
    categoryid INT REFERENCES categories(id),
    imageurl VARCHAR(500),
    isdeleted BOOLEAN NOT NULL DEFAULT FALSE,
    createdat TIMESTAMP DEFAULT NOW(),
    updatedat TIMESTAMP DEFAULT NOW()
);

-- 5️⃣ CARTS
CREATE TABLE carts (
    id SERIAL PRIMARY KEY,
    userid INT REFERENCES users(id) ON DELETE CASCADE,
    createdat TIMESTAMP DEFAULT NOW(),
    updatedat TIMESTAMP DEFAULT NOW()
);

-- 6️⃣ CART ITEMS
CREATE TABLE cartitems (
    id SERIAL PRIMARY KEY,
    cartid INT REFERENCES carts(id) ON DELETE CASCADE,
    productid INT REFERENCES products(id),
    quantity INT NOT NULL DEFAULT 1,
    pricesnapshot NUMERIC(12,2) NOT NULL,
    addedat TIMESTAMP DEFAULT NOW()
);

-- 7️⃣ ORDERS
CREATE TABLE orders (
    id SERIAL PRIMARY KEY,
    userid INT REFERENCES users(id),
    totalamount NUMERIC(12,2) NOT NULL,
    status VARCHAR(50) DEFAULT 'pending',  -- pending | paid | shipped | completed | cancelled
    shippingaddress VARCHAR(255),
    placedat TIMESTAMP DEFAULT NOW(),
    createdat TIMESTAMP DEFAULT NOW(),
    updatedat TIMESTAMP DEFAULT NOW()
);

-- 8️⃣ ORDER ITEMS
CREATE TABLE orderitems (
    id SERIAL PRIMARY KEY,
    orderid INT REFERENCES orders(id) ON DELETE CASCADE,
    productid INT REFERENCES products(id),
    quantity INT NOT NULL,
    price NUMERIC(12,2) NOT NULL,
    productname VARCHAR(255),
    productsku VARCHAR(100)
);

-- 9️⃣ PAYMENTS
CREATE TABLE payments (
    id SERIAL PRIMARY KEY,
    orderid INT UNIQUE REFERENCES orders(id),
    method VARCHAR(50),
    amount NUMERIC(12,2),
    status VARCHAR(50) DEFAULT 'unpaid',   -- unpaid | paid | failed | refunded
    transactionid VARCHAR(255),
    paidat TIMESTAMP NULL,
    createdat TIMESTAMP DEFAULT NOW()
);

-- 🔟 STORE LOCATIONS
CREATE TABLE storelocations (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255),
    latitude DOUBLE PRECISION,
    longitude DOUBLE PRECISION,
    address VARCHAR(255)
);

-- 1️⃣1️⃣ MESSAGES
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE messages (
    id SERIAL PRIMARY KEY,
    conversationid UUID NOT NULL DEFAULT uuid_generate_v4(),
    orderid INT REFERENCES orders(id),
    senderid INT NOT NULL REFERENCES users(id),
    receiverid INT NOT NULL REFERENCES users(id),
    message TEXT,
    createdat TIMESTAMP DEFAULT NOW(),
    isread BOOLEAN DEFAULT FALSE
);

-- 1️⃣2️⃣ FCM TOKENS
CREATE TABLE fcmtokens (
    id SERIAL PRIMARY KEY,
    userid INT REFERENCES users(id),
    token VARCHAR(500) NOT NULL,
    platform VARCHAR(50),
    createdat TIMESTAMP DEFAULT NOW()
);

-- 1️⃣3️⃣ NOTIFICATIONS
CREATE TABLE notifications (
    id SERIAL PRIMARY KEY,
    userid INT REFERENCES users(id),
    title VARCHAR(255),
    body TEXT,
    payload TEXT,
    sentat TIMESTAMP DEFAULT NOW()
);

-- ============================================================
-- SAMPLE DATA
-- ============================================================

INSERT INTO users (email, password, fullname, role)
VALUES 
('admin@gmail.com', '123456', 'Admin PetShop', 'admin'),
('user1@gmail.com', '123456', 'Khách Hàng Thú Cưng', 'customer');

INSERT INTO categories (name, description) VALUES
('Thức ăn cho chó', 'Đồ ăn dinh dưỡng, hạt khô, pate cho chó'),
('Thức ăn cho mèo', 'Đồ ăn dinh dưỡng, hạt khô, pate cho mèo'),
('Thức ăn cho thú nhỏ', 'Đồ ăn cho hamster, thỏ, chim, cá cảnh');

INSERT INTO products (sku, name, description, price, stock, categoryid, imageurl)
VALUES 
('DOGFOOD01', 'Royal Canin Medium Adult', 'Cho chó giống vừa, tăng cường sức đề kháng', 320000, 50, 1, 'https://paddy.vn/cdn/shop/files/hat-royal-canin.png?v=1723473110'),
('DOGFOOD02', 'Pedigree Adult Beef & Veg', 'Hương vị bò và rau củ, đầy đủ vitamin', 250000, 60, 1, 'https://bizweb.dktcdn.net/100/362/345/products/60916c76-8701-4e06-aeac-489d99334153-jpeg.jpg?v=1625861149540'),
('DOGFOOD03', 'Ganador Puppy Milk', 'Dành cho chó con, giúp xương chắc khỏe', 210000, 40, 1, 'https://paddy.vn/cdn/shop/files/ganadordha.jpg?v=1711013482'),
('DOGFOOD04', 'Reflex Plus Lamb & Rice', 'Thức ăn cao cấp cho chó lông mượt', 340000, 30, 1, 'https://cdn.akakce.com/reflex/reflex-plus-orta-ve-buyuk-irk-kuzu-etli-15-kg-yavru-z.jpg'),
('CATFOOD01', 'Whiskas Ocean Fish', 'Cho mèo vị cá biển, bổ sung omega 3-6', 180000, 30, 2, 'https://bizweb.dktcdn.net/thumb/1024x1024/100/383/944/products/thuc-an-hat-cho-cho-fcb58f5a-87a6-44ee-9ca9-2f08db097109.png?v=1589031904057'),
('CATFOOD02', 'Me-O Tuna Flavor', 'Giúp mèo khỏe mạnh, lông óng mượt', 165000, 40, 2, 'https://product.hstatic.net/200000352097/product/8850477001657_4f311614c08f44f5b64510b34dfef676.jpg'),
('CATFOOD03', 'CatEye Salmon Adult', 'Dành cho mèo trưởng thành, dễ tiêu hóa', 195000, 25, 2, 'https://cdn1.npcdn.net/images/17472013602032f0b376bc35794337b8531c3fdd97.webp'),
('CATFOOD04', 'Royal Canin Kitten', 'Dành cho mèo con dưới 12 tháng', 280000, 20, 2, 'https://bizweb.dktcdn.net/100/448/728/products/06879c7d-178a-4ad4-92c6-65ce7392dc14.jpg'),
('SMALL01', 'Thức ăn cho thỏ MENU Premium Rabbit Food', 'Bổ sung chất xơ, giúp tiêu hóa tốt', 180000, 25, 3, 'https://i5.walmartimages.com/seo/MENU-Premium-Rabbit-Food-Timothy-Hay-Pellets-Blend-Vitamin-Mineral-Fortified-4-lb-Bag.jpeg'),
('SMALL02', 'Thức ăn cho hamster VitaKraft', 'Nhiều hạt dinh dưỡng và vitamin', 150000, 30, 3, 'https://www.vietpet.net/wp-content/uploads/2019/12/thuc-an-cho-chuot-hamster-vitakraft-menu-hamster.jpg'),
('SMALL03', 'Thức ăn cho cá cảnh TetraBits', 'Hỗ trợ màu sắc cá sáng đẹp', 115000, 40, 3, 'https://aquatics.sg/cdn/shop/products/Tetra-Bits-complete-Discus-Granules-Tropical-Fish-Food.jpg'),
('SMALL04', 'Thức ăn cho chim cảnh Mazuri Small Bird Diet', 'Giàu hạt ngũ cốc, tăng sức đề kháng', 78000, 50, 3, 'https://mazuri.com/cdn/shop/files/AN_727613010935_2894512_9901_EXOTICMazuriSmallBirdMaintenance2_5lb_front_3D.jpg');

INSERT INTO storelocations (name, latitude, longitude, address)
VALUES ('Cửa hàng thú cưng PetYumYum', 10.7769, 106.7009, '123 Nguyễn Nhật Trường, Q1, TP.HCM');

-- ✅ DONE
DO $$ BEGIN
    RAISE NOTICE '✅ PetFoodShop Database created successfully (PostgreSQL version)';
END $$;
