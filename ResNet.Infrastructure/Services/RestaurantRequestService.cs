using System.Net;
using AutoMapper;
using Domain.Constants;
using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ResNet.Domain.Constants;
using ResNet.Domain.Dtos;
using ResNet.Domain.Entities;
using ResNet.Domain.Filters;

namespace Infrastructure.Services;

public class RestaurantRequestService(
    DataContext context,
    IMapper mapper,
    ILogger<RestaurantRequestService> logger,
    INotificationService notificationService
) : IRestaurantRequestService
{
    public async Task<PagedResponse<List<GetRestaurantRequestDto>>> GetAllRequestsAsync(RequestFilter filter)
    {
        logger.LogInformation("GetAllRequestsAsync called with filter {@Filter}", filter);

        IQueryable<RestaurantRequest> query = context.RestaurantRequests.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.Name))
            query = query.Where(r => r.Name.ToLower().Contains(filter.Name.ToLower()));

        if (!string.IsNullOrWhiteSpace(filter.Status))
            query = query.Where(r => r.Status == filter.Status);

        int totalCount = await query.CountAsync();
        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);

        var requests = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize)
            .ToListAsync();

        var dtos = mapper.Map<List<GetRestaurantRequestDto>>(requests);
        return new PagedResponse<List<GetRestaurantRequestDto>>(dtos, validFilter.PageNumber, validFilter.PageSize, totalCount);
    }

    public async Task<Response<GetRestaurantRequestDto>> GetRequestByIdAsync(int id)
    {
        logger.LogInformation("GetRequestByIdAsync called with id={Id}", id);

        var request = await context.RestaurantRequests.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
        if (request == null)
        {
            logger.LogWarning("Request with id={Id} not found", id);
            return new Response<GetRestaurantRequestDto>(HttpStatusCode.NotFound, "Request not found");
        }

        var dto = mapper.Map<GetRestaurantRequestDto>(request);
        return Response<GetRestaurantRequestDto>.Success(dto);
    }

    public async Task<Response<GetRestaurantRequestDto>> CreateRequestAsync(CreateRestaurantRequestDto requestDto)
    {
        logger.LogInformation("CreateRequestAsync called");

        var exists = await context.RestaurantRequests.AnyAsync(r => r.Name.ToLower() == requestDto.Name.ToLower() && r.Status != RequestStatus.Rejected);
        if (exists)
            return new Response<GetRestaurantRequestDto>(HttpStatusCode.BadRequest, "Request with this restaurant name already exists");

        var request = mapper.Map<RestaurantRequest>(requestDto);
        request.Status = RequestStatus.Pending;
        await context.RestaurantRequests.AddAsync(request);

        var result = await context.SaveChangesAsync();
        if (result == 0)
            return new Response<GetRestaurantRequestDto>(HttpStatusCode.BadRequest, "Request not created");

        var dto = mapper.Map<GetRestaurantRequestDto>(request);
        return Response<GetRestaurantRequestDto>.Success(dto);
    }

    public async Task<Response<GetRestaurantRequestDto>> UpdateRequestStatusAsync(int id, string status)
    {
        logger.LogInformation("UpdateRequestStatusAsync called with id={Id}, status={Status}", id, status);

        var request = await context.RestaurantRequests.FirstOrDefaultAsync(r => r.Id == id);
        if (request == null)
            return new Response<GetRestaurantRequestDto>(HttpStatusCode.NotFound, "Request not found");

        if (!new[] { RequestStatus.Pending, RequestStatus.Accepted, RequestStatus.Rejected }.Contains(status))
            return new Response<GetRestaurantRequestDto>(HttpStatusCode.BadRequest, "Invalid status");

        request.Status = status;
        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<GetRestaurantRequestDto>(HttpStatusCode.BadRequest, "Status not updated");

        await notificationService.SendRestaurantRequestStatusChangedEmail(request);

        var dto = mapper.Map<GetRestaurantRequestDto>(request);
        return Response<GetRestaurantRequestDto>.Success(dto);
    }

    public async Task<Response<string>> DeleteRequestAsync(int id)
    {
        logger.LogInformation("DeleteRequestAsync called with id={Id}", id);

        var request = await context.RestaurantRequests.FirstOrDefaultAsync(r => r.Id == id);
        if (request == null)
            return new Response<string>(HttpStatusCode.NotFound, "Request not found");

        context.RestaurantRequests.Remove(request);
        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "Request not deleted");

        return Response<string>.Success("Request deleted successfully");
    }
}