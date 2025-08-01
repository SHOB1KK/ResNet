using AutoMapper;
using ResNet.Domain.Dtos;
using ResNet.Domain.Entities;

namespace Infrastructure.AutoMapper;

public class InfrastructureProfile : Profile
{
    public InfrastructureProfile()
    {
        // User
        CreateMap<ApplicationUser, GetUserDto>();
        CreateMap<CreateUserDto, ApplicationUser>();
        CreateMap<UpdateUserDto, ApplicationUser>();

        // Category
        CreateMap<Category, GetCategoryDto>();
        CreateMap<CreateCategoryDto, Category>();
        CreateMap<UpdateCategoryDto, Category>();

        // Product
        CreateMap<Product, GetProductDto>()
    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        // Order
        CreateMap<Order, GetOrderDto>();
        CreateMap<CreateOrderDto, Order>();
        CreateMap<UpdateOrderDto, Order>();

        // OrderItem
        CreateMap<OrderItem, GetOrderItemDto>();
        CreateMap<CreateOrderItemDto, OrderItem>();
        CreateMap<UpdateOrderItemDto, OrderItem>();

        // ActionLog
        CreateMap<ActionLog, GetActionLogDto>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));
        CreateMap<CreateActionLogDto, ActionLog>();

        // Table
        CreateMap<Table, GetTableDto>();
        CreateMap<CreateTableDto, Table>();
        CreateMap<UpdateTableDto, Table>();

        // Booking
        CreateMap<Booking, GetBookingDto>();
        CreateMap<CreateBookingDto, Booking>();
        CreateMap<UpdateBookingDto, Booking>();

        // Restaurant
        CreateMap<Restaurant, GetRestaurantDto>()
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(
                src => src.RestaurantCategories.Select(rc => rc.Category)
            ));
        CreateMap<CreateRestaurantDto, Restaurant>();
        CreateMap<UpdateRestaurantDto, Restaurant>();


        // JobApplication
        CreateMap<JobApplication, GetJobApplicationDto>();
        CreateMap<CreateJobApplicationDto, JobApplication>();
        // CreateMap<JobApplicationFormModel, CreateJobApplicationDto>();

        // RestaurantRequest
        CreateMap<RestaurantRequest, GetRestaurantRequestDto>();

        CreateMap<WorkingHourDto, WorkingHour>()
            .ForMember(dest => dest.Day, opt => opt.MapFrom(src => Enum.Parse<DayOfWeek>(src.DayOfWeek)))
            .ForMember(dest => dest.OpenTime, opt => opt.MapFrom(src => src.From))
            .ForMember(dest => dest.CloseTime, opt => opt.MapFrom(src => src.To));

        CreateMap<WorkingHour, WorkingHourDto>()
            .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.Day.ToString()))
            .ForMember(dest => dest.From, opt => opt.MapFrom(src => src.OpenTime))
            .ForMember(dest => dest.To, opt => opt.MapFrom(src => src.CloseTime))
            .ForMember(dest => dest.IsEnabled, opt => opt.MapFrom(src => true));

        CreateMap<CreateRestaurantRequestDto, RestaurantRequest>()
            .ForMember(dest => dest.WorkingHours, opt => opt.MapFrom(src => src.WorkingHours.Where(w => w.IsEnabled).ToList()));

        CreateMap<ApplicationUser, GetUserDto>()
            .ForMember(dest => dest.RestaurantName,
               opt => opt.MapFrom(src => src.Restaurant != null ? src.Restaurant.Name : null));

    }
}
