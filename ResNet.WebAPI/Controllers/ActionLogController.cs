using Domain.Constants;
using Domain.Responses;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResNet.Domain.Constants;
using ResNet.Domain.Dtos;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ActionLogController(IActionLogService actionLogService)
{
    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<PagedResponse<List<GetActionLogDto>>> GetActionLogs([FromQuery] ActionLogFilter filter)
    {
        return await actionLogService.GetActionLogsAsync(filter);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetActionLogDto>> GetActionLogById(int id)
    {
        return await actionLogService.GetActionLogByIdAsync(id);
    }
}
