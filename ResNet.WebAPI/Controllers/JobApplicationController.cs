using Domain.Constants;
using Domain.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResNet.Domain.Dtos;
using ResNet.Domain.Filters;
using Infrastructure.Interfaces;
using ResNet.Domain.Constants;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobApplicationController(IJobApplicationService jobApplicationService)
{
    [HttpGet]
    [Authorize(Roles = Roles.Admin)]
    public async Task<PagedResponse<List<GetJobApplicationDto>>> GetAllApplications([FromQuery] ApplicationFilter filter)
    {
        return await jobApplicationService.GetAllApplicationsAsync(filter);
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetJobApplicationDto>> GetApplicationById(int id)
    {
        return await jobApplicationService.GetApplicationByIdAsync(id);
    }

    [HttpPost]
    public async Task<Response<GetJobApplicationDto>> CreateApplication(CreateJobApplicationDto applicationDto)
    {
        return await jobApplicationService.CreateApplicationAsync(applicationDto);
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<GetJobApplicationDto>> UpdateApplicationStatus(int id, [FromBody] string status)
    {
        return await jobApplicationService.UpdateApplicationStatusAsync(id, status);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<Response<string>> DeleteApplication(int id)
    {
        return await jobApplicationService.DeleteApplicationAsync(id);
    }
}
