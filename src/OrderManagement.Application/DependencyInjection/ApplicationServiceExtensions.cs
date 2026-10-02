using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Application.Common.Behaviors;
using OrderManagement.Application.Orders.Commands.CreateOrder;

namespace OrderManagement.Application.DependencyInjection
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<CreateOrderHandler>();

                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssemblyContaining<CreateOrderValidator>();

            return services;
        }
    }
}