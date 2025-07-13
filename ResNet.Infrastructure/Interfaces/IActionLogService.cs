using Domain.Responses;
using ResNet.Domain.Dtos;

namespace Infrastructure.Interfaces;

public interface IActionLogService
{
    Task<PagedResponse<List<GetActionLogDto>>> GetActionLogsAsync(ActionLogFilter filter);
    Task<Response<GetActionLogDto>> GetActionLogByIdAsync(int id);
}
