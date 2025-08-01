using Domain;
using Infrastructure.Interfaces;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services)
    {
        // Services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderItemService, OrderItemService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRestaurantService, RestaurantService>();
        services.AddScoped<ITableService, TableService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IProfileService, ProfileService>();
        services.AddScoped<IActionLogService, ActionLogService>();
        services.AddScoped<IAdminUserService, AdminUserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IRestaurantRequestService, RestaurantRequestService>();
        services.AddScoped<IJobApplicationService, JobApplicationService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IStatisticService, StatisticService>();
    }
}
