using Domain.Constants;
using Domain.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResNet.Domain.Dtos;
using Infrastructure.Interfaces;
using System.Security.Claims;
using ResNet.Domain.Constants;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = Roles.Admin + "," + Roles.Owner)]
public class StatisticController(IStatisticService statisticService) : ControllerBase
{
    private string GetUserId() => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
    private string? GetUserRole() => User.FindFirstValue(ClaimTypes.Role);
    private int? GetUserRestaurantId()
    {
        var restIdClaim = User.FindFirstValue("RestaurantId");
        if (int.TryParse(restIdClaim, out var id))
            return id;
        return null;
    }

    private bool IsAdmin() => GetUserRole()?.Equals(Roles.Admin) ?? false;
    private bool IsOwner() => GetUserRole()?.Equals(Roles.Owner) ?? false;

    [HttpGet("global/orders")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<OrderStatisticsDto>> GetGlobalOrderStatistics(DateTime? from = null, DateTime? to = null)
    {
        return await statisticService.GetGlobalOrderStatisticsAsync(from, to);
    }

    [HttpGet("orders")]
    public async Task<ActionResult<Response<OrderStatisticsDto>>> GetOrderStatistics(int? restaurantId = null, DateTime? from = null, DateTime? to = null)
    {
        if (IsAdmin())
        {
            if (restaurantId == null)
                return BadRequest(new Response<OrderStatisticsDto>(System.Net.HttpStatusCode.BadRequest, "restaurantId is required for admin"));
            return await statisticService.GetOrderStatisticsAsync(restaurantId.Value, from, to);
        }
        else if (IsOwner())
        {
            var ownerRestId = GetUserRestaurantId();
            if (ownerRestId == null)
                return Forbid();

            return await statisticService.GetOrderStatisticsAsync(ownerRestId.Value, from, to);
        }
        else
        {
            return Forbid();
        }
    }

    [HttpGet("global/top-products")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<PagedResponse<List<ProductSalesDto>>> GetGlobalTopSellingProducts(DateTime? from = null, DateTime? to = null, int pageNumber = 1, int pageSize = 10)
    {
        return await statisticService.GetGlobalTopSellingProductsAsync(from, to, pageNumber, pageSize);
    }

    [HttpGet("top-products")]
    public async Task<ActionResult<PagedResponse<List<ProductSalesDto>>>> GetTopSellingProducts(int? restaurantId = null, DateTime? from = null, DateTime? to = null, int pageNumber = 1, int pageSize = 10)
    {
        if (IsAdmin())
        {
            if (restaurantId == null)
                return BadRequest(new PagedResponse<List<ProductSalesDto>>(System.Net.HttpStatusCode.BadRequest, "restaurantId is required for admin"));
            return await statisticService.GetTopSellingProductsAsync(restaurantId.Value, from, to, pageNumber, pageSize);
        }
        else if (IsOwner())
        {
            var ownerRestId = GetUserRestaurantId();
            if (ownerRestId == null)
                return Forbid();

            return await statisticService.GetTopSellingProductsAsync(ownerRestId.Value, from, to, pageNumber, pageSize);
        }
        else
        {
            return Forbid();
        }
    }

    [HttpGet("global/order-types")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<OrderTypeStatisticsDto>> GetGlobalOrderTypeStatistics(DateTime? from = null, DateTime? to = null)
    {
        return await statisticService.GetGlobalOrderTypeStatisticsAsync(from, to);
    }

    [HttpGet("order-types")]
    public async Task<ActionResult<Response<OrderTypeStatisticsDto>>> GetOrderTypeStatistics(int? restaurantId = null, DateTime? from = null, DateTime? to = null)
    {
        if (IsAdmin())
        {
            if (restaurantId == null)
                return BadRequest(new Response<OrderTypeStatisticsDto>(System.Net.HttpStatusCode.BadRequest, "restaurantId is required for admin"));
            return await statisticService.GetOrderTypeStatisticsAsync(restaurantId.Value, from, to);
        }
        else if (IsOwner())
        {
            var ownerRestId = GetUserRestaurantId();
            if (ownerRestId == null)
                return Forbid();
            return await statisticService.GetOrderTypeStatisticsAsync(ownerRestId.Value, from, to);
        }
        else
        {
            return Forbid();
        }
    }

    [HttpGet("global/average-check")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<decimal>> GetGlobalAverageCheck(DateTime? from = null, DateTime? to = null)
    {
        return await statisticService.GetGlobalAverageCheckAsync(from, to);
    }

    [HttpGet("average-check")]
    public async Task<ActionResult<Response<decimal>>> GetAverageCheck(int? restaurantId = null, DateTime? from = null, DateTime? to = null)
    {
        if (IsAdmin())
        {
            if (restaurantId == null)
                return BadRequest(new Response<decimal>(System.Net.HttpStatusCode.BadRequest, "restaurantId is required for admin"));
            return await statisticService.GetAverageCheckAsync(restaurantId.Value, from, to);
        }
        else if (IsOwner())
        {
            var ownerRestId = GetUserRestaurantId();
            if (ownerRestId == null)
                return Forbid();
            return await statisticService.GetAverageCheckAsync(ownerRestId.Value, from, to);
        }
        else
        {
            return Forbid();
        }
    }

    [HttpGet("global/daily-orders-trend")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<List<DailyOrdersDto>>> GetGlobalDailyOrdersTrend(DateTime from, DateTime to)
    {
        return await statisticService.GetGlobalDailyOrdersTrendAsync(from, to);
    }

    [HttpGet("daily-orders-trend")]
    public async Task<ActionResult<Response<List<DailyOrdersDto>>>> GetDailyOrdersTrend(int? restaurantId, DateTime from, DateTime to)
    {
        if (IsAdmin())
        {
            if (restaurantId == null)
                return BadRequest(new Response<List<DailyOrdersDto>>(System.Net.HttpStatusCode.BadRequest, "restaurantId is required for admin"));
            return await statisticService.GetDailyOrdersTrendAsync(restaurantId.Value, from, to);
        }
        else if (IsOwner())
        {
            var ownerRestId = GetUserRestaurantId();
            if (ownerRestId == null)
                return Forbid();
            return await statisticService.GetDailyOrdersTrendAsync(ownerRestId.Value, from, to);
        }
        else
        {
            return Forbid();
        }
    }
}
