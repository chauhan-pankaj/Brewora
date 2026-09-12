using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Brewora.Application.Common;
using Brewora.Application.Interfaces;
using Brewora.Application.Services;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Brewora.Infrastructure.Security;

public class JwtOptions
{
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = "Brewora";
    public string Audience { get; set; } = "BreworaApp";
    public int ExpiresMinutes { get; set; } = 1440;
}

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _opt;
    public JwtTokenService(IOptions<JwtOptions> opt) => _opt = opt.Value;

    public string CreateToken(UserRecord user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.RoleName)
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
        var token = new JwtSecurityToken(
            _opt.Issuer,
            _opt.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(_opt.ExpiresMinutes),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class RazorpayGateway : IRazorpayGateway
{
    private readonly RazorpayOptions _opt;
    private readonly HttpClient _http;
    public RazorpayGateway(IOptions<RazorpayOptions> opt, HttpClient http)
    {
        _opt = opt.Value;
        _http = http;
    }

    public async Task<(string OrderId, int AmountPaise)> CreateOrderAsync(decimal amountInr, string receipt, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_opt.KeyId) || _opt.KeyId.Contains("replace", StringComparison.OrdinalIgnoreCase))
            throw new AppException("Razorpay test keys are not configured on the server.");

        var paise = (int)Math.Round(amountInr * 100m, MidpointRounding.AwayFromZero);
        var payload = JsonSerializer.Serialize(new { amount = paise, currency = "INR", receipt });
        using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.razorpay.com/v1/orders");
        var basic = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_opt.KeyId}:{_opt.KeySecret}"));
        req.Headers.Authorization = new AuthenticationHeaderValue("Basic", basic);
        req.Content = new StringContent(payload, Encoding.UTF8, "application/json");
        using var res = await _http.SendAsync(req, cancellationToken);
        var body = await res.Content.ReadAsStringAsync(cancellationToken);
        if (!res.IsSuccessStatusCode)
            throw new AppException("Unable to create Razorpay order");
        using var doc = JsonDocument.Parse(body);
        var id = doc.RootElement.GetProperty("id").GetString() ?? throw new AppException("Razorpay did not return an order id");
        return (id, paise);
    }
}
