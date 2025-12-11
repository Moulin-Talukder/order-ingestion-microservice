using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OrderIngest.Api.Services
{
    public interface ILogisticsService 
    { 
        Task NotifyLogisticsAsync(Guid orderId); 
    }

    public class MockLogisticsService : ILogisticsService
    {
        private readonly ILogger<MockLogisticsService> _logger;
        public MockLogisticsService(ILogger<MockLogisticsService> logger)
        {
            _logger = logger;
        }

        public async Task NotifyLogisticsAsync(Guid orderId)
        {
            try
            {
                await Task.Delay(2000);
                _logger.LogInformation("Mock logistics notified for {orderId}", orderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logistics notify failed for {orderId}", orderId);
            }
        }
    }
}
