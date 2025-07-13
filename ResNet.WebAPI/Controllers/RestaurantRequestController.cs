using Domain.Constants;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResNet.Domain.Constants;
using ResNet.Domain.Filters;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RestaurantRequestController(IRestaurantRequestService requestService)
{
    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<PagedResponse<List<GetRestaurantRequestDto>>> GetAllRequests([FromQuery] RequestFilter filter)
    {
        return await requestService.GetAllRequestsAsync(filter);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetRestaurantRequestDto>> GetRequestById(int id)
    {
        return await requestService.GetRequestByIdAsync(id);
    }

    [HttpPost]
    public async Task<Response<GetRestaurantRequestDto>> CreateRequest(CreateRestaurantRequestDto requestDto)
    {
        return await requestService.CreateRequestAsync(requestDto);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetRestaurantRequestDto>> UpdateRequestStatus(int id, [FromBody] string status)
    {
        return await requestService.UpdateRequestStatusAsync(id, status);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<string>> DeleteRequest(int id)
    {
        return await requestService.DeleteRequestAsync(id);
    }
}
