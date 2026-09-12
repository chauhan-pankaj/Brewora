using System.Data;
using Brewora.Application.DTOs;
using Brewora.Application.Interfaces;
using Brewora.Infrastructure.Data;
using Microsoft.Data.SqlClient;

namespace Brewora.Infrastructure.Repositories;

public class SqlReservationRepository : IReservationRepository
{
    private readonly SqlConnectionFactory _factory;
    public SqlReservationRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<int> CreateAsync(CreateReservationDto request, int? userId, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Reservation_Create";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@UserId", (object?)userId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CustomerName", request.CustomerName);
        cmd.Parameters.AddWithValue("@Email", request.Email);
        cmd.Parameters.AddWithValue("@Phone", request.Phone);
        cmd.Parameters.AddWithValue("@ReservationDate", request.ReservationDate.Date);
        cmd.Parameters.AddWithValue("@ReservationTime", request.ReservationTime);
        cmd.Parameters.AddWithValue("@GuestCount", request.GuestCount);
        cmd.Parameters.AddWithValue("@SpecialRequest", (object?)request.SpecialRequest ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Status", "Confirmed");
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }

    public async Task<IEnumerable<SlotDto>> GetAvailabilityAsync(DateTime date, CancellationToken ct)
    {
        var list = new List<SlotDto>();
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Reservation_GetAvailability";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ReservationDate", date.Date);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var remaining = Convert.ToInt32(reader["Remaining"]);
            list.Add(new SlotDto(reader["Time"].ToString() ?? "", remaining > 0, remaining));
        }
        return list;
    }

    public async Task<IEnumerable<ReservationDto>> GetByUserAsync(int userId, CancellationToken ct)
    {
        var list = new List<ReservationDto>();
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Reservation_GetByUser";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@UserId", userId);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct)) list.Add(Map(reader));
        return list;
    }

    public async Task<IEnumerable<ReservationDto>> GetAllAsync(CancellationToken ct)
    {
        var list = new List<ReservationDto>();
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Reservation_GetAll";
        cmd.CommandType = CommandType.StoredProcedure;
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct)) list.Add(Map(reader));
        return list;
    }

    public async Task UpdateStatusAsync(int id, string status, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Reservation_UpdateStatus";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ReservationId", id);
        cmd.Parameters.AddWithValue("@Status", status);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<ReservationDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        var all = await GetAllAsync(ct);
        return all.FirstOrDefault(r => r.ReservationId == id);
    }

    private static ReservationDto Map(SqlDataReader r) => new(
        r.GetInt32(r.GetOrdinal("ReservationId")),
        r.GetString(r.GetOrdinal("CustomerName")),
        r.GetString(r.GetOrdinal("Email")),
        r.GetString(r.GetOrdinal("Phone")),
        r.GetDateTime(r.GetOrdinal("ReservationDate")),
        r.GetString(r.GetOrdinal("ReservationTime")),
        r.GetInt32(r.GetOrdinal("GuestCount")),
        r["SpecialRequest"] as string,
        r.GetString(r.GetOrdinal("Status")));
}

public class SqlUserRepository : IUserRepository
{
    private readonly SqlConnectionFactory _factory;
    public SqlUserRepository(SqlConnectionFactory factory) => _factory = factory;

    public Task<UserRecord?> GetByEmailAsync(string email, CancellationToken ct) =>
        Get("sp_User_GetByEmail", cmd => cmd.Parameters.AddWithValue("@Email", email), ct);
    public Task<UserRecord?> GetByIdAsync(int id, CancellationToken ct) =>
        Get("sp_User_GetById", cmd => cmd.Parameters.AddWithValue("@UserId", id), ct);

    public async Task<int> CreateAsync(int roleId, string fullName, string email, string phone, string passwordHash, string? avatar, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_User_Create";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@RoleId", roleId);
        cmd.Parameters.AddWithValue("@FullName", fullName);
        cmd.Parameters.AddWithValue("@Email", email);
        cmd.Parameters.AddWithValue("@Phone", (object?)phone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
        cmd.Parameters.AddWithValue("@AvatarUrl", (object?)avatar ?? DBNull.Value);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }

    public async Task UpdateProfileAsync(int userId, UpdateProfileDto request, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_User_UpdateProfile";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@FullName", request.FullName);
        cmd.Parameters.AddWithValue("@Phone", request.Phone);
        cmd.Parameters.AddWithValue("@Address", (object?)request.Address ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@AvatarUrl", (object?)request.AvatarUrl ?? DBNull.Value);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private async Task<UserRecord?> Get(string sp, Action<SqlCommand> bind, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = sp;
        cmd.CommandType = CommandType.StoredProcedure;
        bind(cmd);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) return null;
        return new UserRecord(
            reader.GetInt32(reader.GetOrdinal("UserId")),
            reader.GetInt32(reader.GetOrdinal("RoleId")),
            reader.GetString(reader.GetOrdinal("RoleName")),
            reader.GetString(reader.GetOrdinal("FullName")),
            reader.GetString(reader.GetOrdinal("Email")),
            reader["Phone"] as string,
            reader.GetString(reader.GetOrdinal("PasswordHash")),
            reader["AvatarUrl"] as string,
            reader["Address"] as string);
    }
}

public class SqlFavouriteRepository : IFavouriteRepository
{
    private readonly SqlConnectionFactory _factory;
    public SqlFavouriteRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task AddAsync(int userId, int menuItemId, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Favourite_Add";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task RemoveAsync(int userId, int menuItemId, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Favourite_Remove";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@UserId", userId);
        cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<IEnumerable<MenuItemDto>> GetByUserAsync(int userId, CancellationToken ct)
    {
        var list = new List<MenuItemDto>();
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Favourite_GetByUser";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@UserId", userId);
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct)) list.Add(SqlMenuRepository.MapMenu(reader));
        return list;
    }
}

public class SqlGalleryRepository : IGalleryRepository
{
    private readonly SqlConnectionFactory _factory;
    public SqlGalleryRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<GalleryDto>> GetAllAsync(CancellationToken ct)
    {
        var list = new List<GalleryDto>();
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Gallery_GetAll";
        cmd.CommandType = CommandType.StoredProcedure;
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            list.Add(new GalleryDto(
                reader.GetInt32(reader.GetOrdinal("GalleryId")),
                reader.GetString(reader.GetOrdinal("Title")),
                reader.GetString(reader.GetOrdinal("ImageUrl")),
                reader["Caption"] as string));
        }
        return list;
    }

    public async Task<int> CreateAsync(string title, string imageUrl, string? caption, int sort, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Gallery_Create";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Title", title);
        cmd.Parameters.AddWithValue("@ImageUrl", imageUrl);
        cmd.Parameters.AddWithValue("@Caption", (object?)caption ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@SortOrder", sort);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }
}

public class SqlEventRepository : IEventRepository
{
    private readonly SqlConnectionFactory _factory;
    public SqlEventRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task<IEnumerable<EventDto>> GetAllAsync(CancellationToken ct)
    {
        var list = new List<EventDto>();
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Event_GetAll";
        cmd.CommandType = CommandType.StoredProcedure;
        await using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            list.Add(new EventDto(
                reader.GetInt32(reader.GetOrdinal("EventId")),
                reader.GetString(reader.GetOrdinal("Title")),
                reader.GetString(reader.GetOrdinal("Description")),
                reader.GetDateTime(reader.GetOrdinal("EventDate")),
                reader.GetString(reader.GetOrdinal("ImageUrl"))));
        }
        return list;
    }

    public async Task<int> CreateAsync(string title, string description, DateTime eventDate, string imageUrl, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Event_Create";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Title", title);
        cmd.Parameters.AddWithValue("@Description", description);
        cmd.Parameters.AddWithValue("@EventDate", eventDate);
        cmd.Parameters.AddWithValue("@ImageUrl", imageUrl);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
    }
}

public class SqlContactRepository : IContactRepository
{
    private readonly SqlConnectionFactory _factory;
    public SqlContactRepository(SqlConnectionFactory factory) => _factory = factory;

    public async Task CreateAsync(ContactDto request, CancellationToken ct)
    {
        await using var conn = _factory.Create();
        await conn.OpenAsync(ct);
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "sp_Contact_Create";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Name", request.Name);
        cmd.Parameters.AddWithValue("@Email", request.Email);
        cmd.Parameters.AddWithValue("@Phone", (object?)request.Phone ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Message", request.Message);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
