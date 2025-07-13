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

public class JobApplicationService(
    DataContext context,
    IMapper mapper,
    ILogger<JobApplicationService> logger,
    INotificationService notificationService
) : IJobApplicationService
{
    public async Task<PagedResponse<List<GetJobApplicationDto>>> GetAllApplicationsAsync(ApplicationFilter filter)
    {
        logger.LogInformation("GetAllApplicationsAsync called with filter {@Filter}", filter);

        IQueryable<JobApplication> query = context.JobApplications.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filter.FirstName))
            query = query.Where(j => j.FirstName.ToLower().Contains(filter.FirstName.ToLower()));

        if (!string.IsNullOrWhiteSpace(filter.LastName))
            query = query.Where(j => j.LastName.ToLower().Contains(filter.LastName.ToLower()));

        if (!string.IsNullOrWhiteSpace(filter.RestaurantName))
            query = query.Where(j => j.RestaurantName.ToLower().Contains(filter.RestaurantName.ToLower()));

        if (!string.IsNullOrWhiteSpace(filter.Status))
            query = query.Where(j => j.Status == filter.Status);

        int totalCount = await query.CountAsync();
        var validFilter = new ValidFilter(filter.PageNumber, filter.PageSize);

        var applications = await query
            .OrderByDescending(j => j.CreatedAt)
            .Skip((validFilter.PageNumber - 1) * validFilter.PageSize)
            .Take(validFilter.PageSize)
            .ToListAsync();

        var dtos = mapper.Map<List<GetJobApplicationDto>>(applications);
        return new PagedResponse<List<GetJobApplicationDto>>(dtos, validFilter.PageNumber, validFilter.PageSize, totalCount);
    }

    public async Task<Response<GetJobApplicationDto>> GetApplicationByIdAsync(int id)
    {
        logger.LogInformation("GetApplicationByIdAsync called with id={Id}", id);

        var application = await context.JobApplications.AsNoTracking().FirstOrDefaultAsync(j => j.Id == id);
        if (application == null)
        {
            logger.LogWarning("Application with id={Id} not found", id);
            return new Response<GetJobApplicationDto>(HttpStatusCode.NotFound, "Application not found");
        }

        var dto = mapper.Map<GetJobApplicationDto>(application);
        return Response<GetJobApplicationDto>.Success(dto);
    }

    public async Task<Response<GetJobApplicationDto>> CreateApplicationAsync(CreateJobApplicationDto applicationDto)
    {
        logger.LogInformation("CreateApplicationAsync called");

        var application = mapper.Map<JobApplication>(applicationDto);
        application.Status = RequestStatus.Pending;
        await context.JobApplications.AddAsync(application);

        var result = await context.SaveChangesAsync();
        if (result == 0)
            return new Response<GetJobApplicationDto>(HttpStatusCode.BadRequest, "Application not created");

        var dto = mapper.Map<GetJobApplicationDto>(application);
        return Response<GetJobApplicationDto>.Success(dto);
    }

    public async Task<Response<GetJobApplicationDto>> UpdateApplicationStatusAsync(int id, string status)
    {
        logger.LogInformation("UpdateApplicationStatusAsync called with id={Id}, status={Status}", id, status);

        var application = await context.JobApplications.FirstOrDefaultAsync(j => j.Id == id);
        if (application == null)
            return new Response<GetJobApplicationDto>(HttpStatusCode.NotFound, "Application not found");

        if (!new[] { RequestStatus.Pending, RequestStatus.Accepted, RequestStatus.Rejected }.Contains(status))
            return new Response<GetJobApplicationDto>(HttpStatusCode.BadRequest, "Invalid status");

        application.Status = status;
        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<GetJobApplicationDto>(HttpStatusCode.BadRequest, "Status not updated");


        await notificationService.SendJobApplicationStatusChangedEmail(application);

        var dto = mapper.Map<GetJobApplicationDto>(application);
        return Response<GetJobApplicationDto>.Success(dto);
    }


    public async Task<Response<string>> DeleteApplicationAsync(int id)
    {
        logger.LogInformation("DeleteApplicationAsync called with id={Id}", id);

        var application = await context.JobApplications.FirstOrDefaultAsync(j => j.Id == id);
        if (application == null)
            return new Response<string>(HttpStatusCode.NotFound, "Application not found");

        context.JobApplications.Remove(application);
        var result = await context.SaveChangesAsync();

        if (result == 0)
            return new Response<string>(HttpStatusCode.BadRequest, "Application not deleted");

        return Response<string>.Success("Application deleted successfully");
    }
}