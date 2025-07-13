using System.Threading.Tasks;
using ResNet.Domain.Entities;

public interface INotificationService
{
    Task SendRestaurantRequestStatusChangedEmail(RestaurantRequest request);
    Task SendJobApplicationStatusChangedEmail(JobApplication application);
}
