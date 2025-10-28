# 🐾 PetFoodShop API

ASP.NET Core Web API cho hệ thống quản lý cửa hàng thức ăn cho thú cưng.

## 🏗️ Kiến trúc

```
PetFoodShop.Api/
├── Controllers/         # API Endpoints (7 controllers)
├── Services/
│   ├── Interfaces/     # Service contracts
│   └── Implements/     # Business logic implementation
├── Repositories/
│   ├── Interfaces/     # Repository contracts (Generic + Specific)
│   └── Implements/     # Data access layer
├── Models/             # Database entities (12 models)
├── Dtos/               # Data Transfer Objects
└── Data/               # DbContext
```

### Luồng xử lý
```
Controller → Service (Business Logic) → Repository (Data Access) → DbContext → Database
```

## 📦 Công nghệ sử dụng

- **.NET 8.0** - Framework
- **Entity Framework Core 8.0.8** - ORM
- **PostgreSQL** - Database (Npgsql 8.0.8)
- **BCrypt.Net 4.0.3** - Password hashing
- **Swagger/OpenAPI** - API Documentation

## 🗄️ Database Schema

### Core Tables
- **users** - Quản lý người dùng (customer/admin roles)
- **categories** - Danh mục sản phẩm
- **products** - Sản phẩm thức ăn
- **carts** + **cartitems** - Giỏ hàng
- **orders** + **orderitems** - Đơn hàng
- **payments** - Thanh toán
- **storelocations** - Vị trí cửa hàng

### Additional Tables
- **messages** - Hệ thống nhắn tin
- **notifications** - Thông báo
- **fcmtokens** - Firebase Cloud Messaging tokens

## 🚀 API Endpoints

### Products (`/api/products`)
- `GET /` - Lấy danh sách sản phẩm (query: activeOnly, categoryId)
- `GET /{id}` - Lấy chi tiết sản phẩm
- `POST /` - Tạo sản phẩm mới
- `PUT /{id}` - Cập nhật sản phẩm
- `DELETE /{id}` - Xóa sản phẩm
- `PATCH /{id}/soft-delete` - Soft delete sản phẩm

### Categories (`/api/categories`)
- `GET /` - Lấy danh sách danh mục
- `GET /{id}` - Lấy chi tiết danh mục
- `POST /` - Tạo danh mục mới
- `PUT /{id}` - Cập nhật danh mục
- `DELETE /{id}` - Xóa danh mục

### Users (`/api/users`)
- `GET /` - Lấy danh sách người dùng (query: activeOnly)
- `GET /{id}` - Lấy chi tiết người dùng
- `GET /email/{email}` - Tìm người dùng theo email
- `POST /` - Đăng ký người dùng mới
- `PUT /{id}` - Cập nhật thông tin
- `DELETE /{id}` - Xóa người dùng
- `PATCH /{id}/soft-delete` - Soft delete người dùng

### Carts (`/api/carts`)
- `GET /user/{userId}` - Lấy giỏ hàng của user
- `POST /items` - Thêm sản phẩm vào giỏ
- `PUT /items/{cartItemId}` - Cập nhật số lượng
- `DELETE /items/{cartItemId}` - Xóa sản phẩm khỏi giỏ
- `DELETE /user/{userId}/clear` - Xóa toàn bộ giỏ hàng

### Orders (`/api/orders`)
- `GET /` - Lấy danh sách đơn hàng (query: status)
- `GET /{id}` - Lấy chi tiết đơn hàng
- `GET /user/{userId}` - Lấy đơn hàng của user
- `POST /` - Tạo đơn hàng mới
- `PATCH /{id}/status` - Cập nhật trạng thái đơn hàng
- `DELETE /{id}` - Xóa đơn hàng

### Payments (`/api/payments`)
- `GET /` - Lấy danh sách thanh toán (query: status)
- `GET /{id}` - Lấy chi tiết thanh toán
- `GET /order/{orderId}` - Lấy thanh toán theo đơn hàng
- `POST /` - Tạo thanh toán mới
- `PATCH /{id}` - Cập nhật thanh toán
- `DELETE /{id}` - Xóa thanh toán

### Store Locations (`/api/storelocations`)
- `GET /` - Lấy danh sách cửa hàng
- `GET /{id}` - Lấy chi tiết cửa hàng
- `GET /nearby` - Tìm cửa hàng gần (query: latitude, longitude, radiusKm)
- `POST /` - Tạo cửa hàng mới
- `PUT /{id}` - Cập nhật thông tin cửa hàng
- `DELETE /{id}` - Xóa cửa hàng

## ⚙️ Configuration

### Connection String
Cấu hình trong `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "PetFoodShopDb": "Host=localhost;Port=5432;Database=petfoodshop;Username=postgres;Password=12345"
  }
}
```

## 🏃 Chạy ứng dụng

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL server
- Database: `petfoodshop`

### Build & Run
```bash
# Restore dependencies
dotnet restore

# Build project
dotnet build

# Run application
dotnet run --project PetFoodShop.Api

# Or use watch mode for development
dotnet watch run --project PetFoodShop.Api
```


