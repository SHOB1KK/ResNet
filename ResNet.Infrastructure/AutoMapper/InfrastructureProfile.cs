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

        // RestaurantRequest
        CreateMap<RestaurantRequest, GetRestaurantRequestDto>();
        CreateMap<CreateRestaurantRequestDto, RestaurantRequest>();
    }
}
