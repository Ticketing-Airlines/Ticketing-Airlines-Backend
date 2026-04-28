using Airline1.Dtos.Responses;
using Airline1.IRepositories;
using Airline1.IService;
using Airline1.Models;
using System.Text.Json;

namespace Airline1.Services
{
    public class PaymentMethodService(IPaymentMethodRepository methodRepo, IPaymentFAQRepository faqRepo) : IPaymentMethodService
    {
        private readonly IPaymentMethodRepository _methodRepo = methodRepo;
        private readonly IPaymentFAQRepository _faqRepo = faqRepo;

        public async Task<ActivePaymentMethodsDto> GetActiveMethodsAsync(string? category, bool? featured)
        {
            List<PaymentMethod> methods;

            if (!string.IsNullOrWhiteSpace(category) && category != "All")
            {
                methods = await _methodRepo.GetActiveByCategoryAsync(category);
            }
            else if (featured == true)
            {
                methods = await _methodRepo.GetActiveFeaturedAsync();
            }
            else
            {
                methods = await _methodRepo.GetAllActiveAsync();
            }

            var categories = methods.Select(m => m.Category).Distinct().OrderBy(c => c).ToList();
            categories.Insert(0, "All");

            var responseMethods = methods.Select(MapToResponse).ToList();

            return new ActivePaymentMethodsDto
            {
                PaymentMethods = responseMethods,
                Categories = categories,
                Total = responseMethods.Count,
                Metadata = new ActivePaymentMethodsMetadata
                {
                    LastUpdated = DateTime.UtcNow,
                    Version = "1.0"
                }
            };
        }

        public async Task<List<PaymentFAQResponse>> GetFAQsAsync()
        {
            var faqs = await _faqRepo.GetAllActiveAsync();
            return faqs.Select(MapToFAQResponse).ToList();
        }

        private static PaymentMethodResponse MapToResponse(PaymentMethod pm)
        {
            return new PaymentMethodResponse
            {
                Id = pm.Id,
                Category = pm.Category,
                Name = pm.Name,
                Description = pm.Description,
                ProcessingTime = pm.ProcessingTime,
                Fee = new PaymentFeeDto
                {
                    Type = pm.FeeType,
                    Amount = pm.FeeAmount,
                    FixedAmount = pm.FeeFixedAmount,
                    Currency = pm.FeeCurrency,
                    DisplayText = pm.FeeDisplayText
                },
                Color = pm.Color,
                Featured = pm.Featured,
                IsActive = pm.IsActive,
                Providers = SafeDeserializeList(pm.ProvidersJson),
                Icon = pm.Icon,
                Features = SafeDeserializeList(pm.FeaturesJson),
                Availability = new PaymentAvailabilityDto
                {
                    IsAvailable = pm.IsAvailable,
                    MaintenanceSchedule = pm.MaintenanceSchedule
                },
                Order = pm.DisplayOrder
            };
        }

        private static PaymentFAQResponse MapToFAQResponse(PaymentFAQ faq)
        {
            return new PaymentFAQResponse
            {
                Id = faq.Id,
                Category = faq.Category,
                Question = faq.Question,
                Answer = faq.Answer,
                Icon = faq.Icon,
                Color = faq.Color,
                Order = faq.DisplayOrder
            };
        }

        private static List<string> SafeDeserializeList(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return [];
            try
            {
                return JsonSerializer.Deserialize<List<string>>(json) ?? [];
            }
            catch
            {
                return [];
            }
        }
    }
}
