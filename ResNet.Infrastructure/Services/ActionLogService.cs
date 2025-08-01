using System.Net;
using AutoMapper;

using Domain.Responses;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; 
using ResNet.Domain.Dtos;
using ResNet.Domain.Entities;

namespace Infrastructure.Services;


public class ActionLogService(
    DataContext context,
    IMapper mapper,
    ILogger<ActionLogService> logger
) : IActionLogService
{
    public async Task<PagedResponse<List<GetActionLogDto>>> GetActionLogsAsync(ActionLogFilter filter)
    {
        logger.LogInformation("GetActionLogsAsync called with filter {@Filter}", filter);

        IQueryable<ActionLog> query = context.ActionLogs
            .Include(a => a.User)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(filter.UserId))
            query = query.Where(a => a.UserId == filter.UserId);

        if (!string.IsNullOrWhiteSpace(filter.ActionType))
            query = query.Where(a => a.ActionType.ToLower().Contains(filter.ActionType.ToLower()));

        if (!string.IsNullOrWhiteSpace(filter.Entity))
            query = query.Where(a => a.Entity.ToLower().Contains(filter.Entity.ToLower()));

        if (filter.EntityId.HasValue)
            query = query.Where(a => a.EntityId == filter.EntityId);

        if (filter.From.HasValue)
            query = query.Where(a => a.Timestamp >= filter.From);

        if (filter.To.HasValue)
            query = query.Where(a => a.Timestamp <= filter.To);

        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);

        int totalCount = await query.CountAsync();

        var logs = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize)
            .ToListAsync();

        var dtos = mapper.Map<List<GetActionLogDto>>(logs);

        return new PagedResponse<List<GetActionLogDto>>(dtos, validFilter.PageNumber, validFilter.PageSize, totalCount);
    }

    public async Task<Response<GetActionLogDto>> GetActionLogByIdAsync(int id)
    {
        logger.LogInformation("GetActionLogByIdAsync called with id={Id}", id);

        var log = await context.ActionLogs
            .Include(a => a.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);

        if (log == null)
        {
            logger.LogWarning("ActionLog with id={Id} not found", id);
            return new Response<GetActionLogDto>(HttpStatusCode.NotFound, "Action log not found");
        }

        var dto = mapper.Map<GetActionLogDto>(log);
        return Response<GetActionLogDto>.Success(dto);
    }
}
