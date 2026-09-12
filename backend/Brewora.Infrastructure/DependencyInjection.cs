using Brewora.Application.Interfaces;
using Brewora.Application.Services;
using Brewora.Infrastructure.Data;
using Brewora.Infrastructure.Persistence;
using Brewora.Infrastructure.Repositories;
using Brewora.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Brewora.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddBreworaInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtOptions>(config.GetSection("Jwt"));
        services.Configure<RazorpayOptions>(config.GetSection("Razorpay"));
        services.AddHttpClient<IRazorpayGateway, RazorpayGateway>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        var useSql = config.GetValue<bool>("Data:UseSqlServer");
        if (useSql)
        {
            services.AddSingleton<SqlConnectionFactory>();
            services.AddScoped<IMenuRepository, SqlMenuRepository>();
            services.AddScoped<ICategoryRepository, SqlCategoryRepository>();
            services.AddScoped<IOrderRepository, SqlOrderRepository>();
            services.AddScoped<IOrderItemRepository, SqlOrderItemRepository>();
            services.AddScoped<IPaymentRepository, SqlPaymentRepository>();
            services.AddScoped<IReservationRepository, SqlReservationRepository>();
            services.AddScoped<IUserRepository, SqlUserRepository>();
            services.AddScoped<IFavouriteRepository, SqlFavouriteRepository>();
            services.AddScoped<IGalleryRepository, SqlGalleryRepository>();
            services.AddScoped<IEventRepository, SqlEventRepository>();
            services.AddScoped<IContactRepository, SqlContactRepository>();
        }
        else
        {
            services.AddSingleton<InMemoryStore>();
            services.AddSingleton<IMenuRepository, InMemoryMenuRepository>();
            services.AddSingleton<ICategoryRepository, InMemoryCategoryRepository>();
            services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
            services.AddSingleton<IOrderItemRepository, InMemoryOrderItemRepository>();
            services.AddSingleton<IPaymentRepository, InMemoryPaymentRepository>();
            services.AddSingleton<IReservationRepository, InMemoryReservationRepository>();
            services.AddSingleton<IUserRepository, InMemoryUserRepository>();
            services.AddSingleton<IFavouriteRepository, InMemoryFavouriteRepository>();
            services.AddSingleton<IGalleryRepository, InMemoryGalleryRepository>();
            services.AddSingleton<IEventRepository, InMemoryEventRepository>();
            services.AddSingleton<IContactRepository, InMemoryContactRepository>();
        }

        services.AddScoped<MenuService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<OrderService>();
        services.AddScoped<PaymentService>();
        services.AddScoped<ReservationService>();
        services.AddScoped<AuthService>();
        return services;
    }
}
