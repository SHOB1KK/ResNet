using Domain.Responses;
using ResNet.Domain.Filters;

public interface IJobApplicationService
{
    Task<PagedResponse<List<GetJobApplicationDto>>> GetAllApplicationsAsync(ApplicationFilter filter);
    Task<Response<GetJobApplicationDto>> GetApplicationByIdAsync(int id);
    Task<Response<GetJobApplicationDto>> CreateApplicationAsync(CreateJobApplicationDto applicationDto);
    Task<Response<GetJobApplicationDto>> UpdateApplicationStatusAsync(int id, string status);
    Task<Response<string>> DeleteApplicationAsync(int id);
}