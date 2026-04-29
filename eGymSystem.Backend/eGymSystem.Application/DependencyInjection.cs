using eGymSystem.Application.Abstractions.Messaging;
using eGymSystem.Application.Abstractions.Validation;
using eGymSystem.Application.Messaging;
using eGymSystem.Application.Messaging.Behaviors;
using eGymSystem.Application.Modules.Training.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace eGymSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();

        services.AddScoped(typeof(ICommandBehavior<,>), typeof(CommandLoggingBehavior<,>));
        services.AddScoped(typeof(ICommandBehavior<,>), typeof(CommandValidationBehavior<,>));

        services.AddScoped<ICommandHandler<CreateTrainingRequestCommand, CreateTrainingRequestResult>, CreateTrainingRequestCommandHandler>();
        services.AddScoped<ICommandValidator<CreateTrainingRequestCommand>, CreateTrainingRequestCommandValidator>();

        return services;
    }
}
