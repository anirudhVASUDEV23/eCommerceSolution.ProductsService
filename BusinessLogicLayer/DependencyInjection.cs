using BusinessLogicLayer.Services;
using eCommerce.BusinessLogicLayer.Mappers;
using eCommerce.BusinessLogicLayer.ServiceContracts;
using eCommerce.BusinessLogicLayer.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
        {
            // Register your business logic layer services here
            // Example:
            // services.AddScoped<IYourService, YourService>();
            services.AddAutoMapper(cfg => { },typeof(ProductAddRequestToProductMappingProfile).Assembly);
            services.AddScoped<IProductsService, ProductsService>();
            services.AddValidatorsFromAssemblyContaining<ProductAddRequestValidator>();
            return services;
        }
    }
}
