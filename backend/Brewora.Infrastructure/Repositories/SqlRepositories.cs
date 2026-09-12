using System.Data;
using Brewora.Application.DTOs;
using Brewora.Application.Interfaces;
using Brewora.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Brewora.Infrastructure.Repositories;

public class SqlMenuRepository : IMenuRepository
{
    private readonly SqlConnectionFactory _factory;
    public SqlMenuRepository(SqlConnectionFactory factory) => _factory = factory;

    public Task<IEnumerable<MenuItemDto>> GetAllAsync(CancellationToken ct) => Query("sp_Menu_GetAll", null, ct);
    public Task<IEnumerable<MenuItemDto>> GetFeaturedAsync(CancellationToken ct) => Query("sp_Menu_GetFeatured", null, ct);
    public Task<IEnumerable<MenuItemDto>> GetByCategoryAsync(int categoryId, CancellationToken ct) =>
        Query("sp_Menu_GetByCategory", cmd => cmd.Parameters.AddWithValue("@CategoryId", categoryId), ct);

    public async Task<MenuItemDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var rows = await Query("sp_Menu_GetById", cmd => cmd.Parameters.AddWithValue("@MenuItemId", id), ct);
        return rows.FirstOrDefault();
    }

    public async Task<int> CreateAsync(CreateMenuItemDto request, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Menu_Create";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@CategoryId", request.CategoryId);
        cmd.Parameters.AddWithValue("@Name", request.Name);
        cmd.Parameters.AddWithValue("@Description", request.Description);
        cmd.Parameters.AddWithValue("@Price", request.Price);
        cmd.Parameters.AddWithValue("@ImageUrl", request.ImageUrl);
        cmd.Parameters.AddWithValue("@IsFeatured", request.IsFeatured);
        cmd.Parameters.AddWithValue("@IsAvailable", request.IsAvailable);
        var result = await cmd.ExecuteScalarAsync(ct);
        return Convert.ToInt32(result);
    }

    public async Task UpdateAsync(int id, CreateMenuItemDto request, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Menu_Update";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@MenuItemId", id);
        cmd.Parameters.AddWithValue("@CategoryId", request.CategoryId);
        cmd.Parameters.AddWithValue("@Name", request.Name);
        cmd.Parameters.AddWithValue("@Description", request.Description);
        cmd.Parameters.AddWithValue("@Price", request.Price);
        cmd.Parameters.AddWithValue("@ImageUrl", request.ImageUrl);
        cmd.Parameters.AddWithValue("@IsFeatured", request.IsFeatured);
        cmd.Parameters.AddWithValue("@IsAvailable", request.IsAvailable);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private async Task<IEnumerable<MenuItemDto>> Query(string sp, Action<SqlCommand>? bind, CancellationToken ct)
    {
        var list = new List<MenuItemDto>();
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sp;
        cmd.CommandType = CommandType.StoredProcedure;
        bind?.Invoke(cmd);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
            list.Add(MapMenu(reader));
        return list;
    }

    internal static MenuItemDto MapMenu(SqlDataReader r) => new(
        r.GetInt32(r.GetOrdinal("MenuItemId")),
        r.GetInt32(r.GetOrdinal("CategoryId")),
        r["CategoryName"] as string,
        r.GetString(r.GetOrdinal("Name")),
        r.GetString(r.GetOrdinal("Description")),
        r.GetDecimal(r.GetOrdinal("Price")),
        r.GetString(r.GetOrdinal("ImageUrl")),
        r.GetBoolean(r.GetOrdinal("IsFeatured")),
        r.GetBoolean(r.GetOrdinal("IsAvailable")));
}

public class SqlCategoryRepository : ICategoryRepository
{
    private readonly SqlConnectionFactory _factory;
    public SqlCategoryRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken ct)
    {
        var list = new List<CategoryDto>();
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Category_GetAll";
        cmd.CommandType = CommandType.StoredProcedure;
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            list.Add(new CategoryDto(
                reader.GetInt32(reader.GetOrdinal("CategoryId")),
                reader.GetString(reader.GetOrdinal("Name")),
                reader.GetString(reader.GetOrdinal("Slug")),
                reader["Icon"] as string));
        }
        return list;
    }

    public async Task<int> CreateAsync(string name, string slug, string icon, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Category_Create";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Name", name);
        cmd.Parameters.AddWithValue("@Slug", slug);
        cmd.Parameters.AddWithValue("@Icon", icon);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }
}

public class SqlOrderRepository : IOrderRepository
{
    private readonly SqlConnectionFactory _factory;
    public SqlOrderRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<int> CreateAsync(OrderDto order, int? userId, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Order_Create";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@OrderNumber", order.OrderNumber);
        cmd.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CustomerName", order.CustomerName);
        cmd.Parameters.AddWithValue("@CustomerEmail", order.CustomerEmail);
        cmd.Parameters.AddWithValue("@CustomerPhone", order.CustomerPhone);
        cmd.Parameters.AddWithValue("@DeliveryAddress", order.DeliveryAddress);
        cmd.Parameters.AddWithValue("@Subtotal", order.Subtotal);
        cmd.Parameters.AddWithValue("@DeliveryCharges", order.DeliveryCharges);
        cmd.Parameters.AddWithValue("@Discount", order.Discount);
        cmd.Parameters.AddWithValue("@Tax", order.Tax);
        cmd.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);
        cmd.Parameters.AddWithValue("@OrderStatus", order.OrderStatus);
        cmd.Parameters.AddWithValue("@PaymentStatus", order.PaymentStatus);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }

    public async Task<OrderDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Order_GetById";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@OrderId", id);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;
        var order = MapOrder(reader);
        var items = new List<OrderItemDto>();
        if (await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                items.Add(new OrderItemDto(
                    reader.GetInt32(reader.GetOrdinal("OrderItemId")),
                    reader.GetInt32(reader.GetOrdinal("MenuItemId")),
                    reader["Name"] as string,
                    reader["ImageUrl"] as string,
                    reader.GetInt32(reader.GetOrdinal("Quantity")),
                    reader.GetDecimal(reader.GetOrdinal("UnitPrice")),
                    reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                    reader["Size"] as string));
            }
        }
        return order with { Items = items };
    }

    public Task<IEnumerable<OrderDto>> GetByUserAsync(int userId, CancellationToken ct) =>
        List("sp_Order_GetByUser", cmd => cmd.Parameters.AddWithValue("@UserId", userId), ct);
    public Task<IEnumerable<OrderDto>> GetByEmailAsync(string email, CancellationToken ct) =>
        List("sp_Order_GetByEmail", cmd => cmd.Parameters.AddWithValue("@Email", email), ct);
    public Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken ct) => List("sp_Order_GetAll", null, ct);

    public async Task UpdatePaymentAsync(int orderId, string paymentStatus, string orderStatus, string? razorpayOrderId, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Order_UpdatePayment";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@OrderId", orderId);
        cmd.Parameters.AddWithValue("@PaymentStatus", paymentStatus);
        cmd.Parameters.AddWithValue("@OrderStatus", orderStatus);
        cmd.Parameters.AddWithValue("@RazorpayOrderId", (object?)razorpayOrderId ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task UpdateStatusAsync(int orderId, string orderStatus, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Order_UpdateStatus";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@OrderId", orderId);
        cmd.Parameters.AddWithValue("@OrderStatus", orderStatus);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private async Task<IEnumerable<OrderDto>> List(string sp, Action<SqlCommand>? bind, CancellationToken ct)
    {
        var list = new List<OrderDto>();
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sp;
        cmd.CommandType = CommandType.StoredProcedure;
        bind?.Invoke(cmd);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct)) list.Add(MapOrder(reader));
        return list;
    }

    private static OrderDto MapOrder(SqlDataReader r) => new(
        r.GetInt32(r.GetOrdinal("OrderId")),
        r.GetString(r.GetOrdinal("OrderNumber")),
        r.GetString(r.GetOrdinal("CustomerName")),
        r.GetString(r.GetOrdinal("CustomerEmail")),
        r.GetString(r.GetOrdinal("CustomerPhone")),
        r.GetString(r.GetOrdinal("DeliveryAddress")),
        r.GetDecimal(r.GetOrdinal("Subtotal")),
        r.GetDecimal(r.GetOrdinal("DeliveryCharges")),
        r.GetDecimal(r.GetOrdinal("Discount")),
        r.GetDecimal(r.GetOrdinal("Tax")),
        r.GetDecimal(r.GetOrdinal("TotalAmount")),
        r.GetString(r.GetOrdinal("OrderStatus")),
        r.GetString(r.GetOrdinal("PaymentStatus")),
        r["RazorpayOrderId"] as string,
        r.GetDateTime(r.GetOrdinal("CreatedDate")),
        null);
}

public class SqlOrderItemRepository : IOrderItemRepository
{
    private readonly SqlConnectionFactory _factory;
    public SqlOrderItemRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task CreateAsync(int orderId, int menuItemId, int quantity, decimal unitPrice, decimal totalPrice, string size, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_OrderItems_Create";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@OrderId", orderId);
        cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
        cmd.Parameters.AddWithValue("@Quantity", quantity);
        cmd.Parameters.AddWithValue("@UnitPrice", unitPrice);
        cmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
        cmd.Parameters.AddWithValue("@Size", (object?)size ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}

public class SqlPaymentRepository : IPaymentRepository
{
    private readonly SqlConnectionFactory _factory;
    public SqlPaymentRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<int> CreateAsync(PaymentWrite write, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Payment_Create";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@OrderId", write.OrderId);
        cmd.Parameters.AddWithValue("@RazorpayOrderId", (object?)write.RazorpayOrderId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@RazorpayPaymentId", (object?)write.RazorpayPaymentId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@RazorpaySignature", (object?)write.RazorpaySignature ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Amount", write.Amount);
        cmd.Parameters.AddWithValue("@Currency", write.Currency);
        cmd.Parameters.AddWithValue("@PaymentStatus", write.PaymentStatus);
        cmd.Parameters.AddWithValue("@PaymentMethod", (object?)write.PaymentMethod ?? DBNull.Value);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }

    public async Task<IEnumerable<PaymentWrite>> GetByOrderIdAsync(int orderId, CancellationToken ct)
    {
        var list = new List<PaymentWrite>();
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Payment_GetByOrderId";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@OrderId", orderId);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            list.Add(new PaymentWrite(
                reader.GetInt32(reader.GetOrdinal("PaymentId")),
                reader.GetInt32(reader.GetOrdinal("OrderId")),
                reader["RazorpayOrderId"] as string,
                reader["RazorpayPaymentId"] as string,
                reader["RazorpaySignature"] as string,
                reader.GetDecimal(reader.GetOrdinal("Amount")),
                reader.GetString(reader.GetOrdinal("Currency")),
                reader.GetString(reader.GetOrdinal("PaymentStatus")),
                reader["PaymentMethod"] as string));
        }
        return list;
    }

    public async Task UpdateStatusAsync(int paymentId, string status, string? paymentIdRzp, string? signature, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Payment_UpdateStatus";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@PaymentId", paymentId);
        cmd.Parameters.AddWithValue("@PaymentStatus", status);
        cmd.Parameters.AddWithValue("@RazorpayPaymentId", (object?)paymentIdRzp ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@RazorpaySignature", (object?)signature ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
