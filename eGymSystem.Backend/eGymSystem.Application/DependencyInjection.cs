using eGymSystem.Application.Abstractions.Messaging;
using eGymSystem.Application.Abstractions.Validation;
using eGymSystem.Application.Messaging;
using eGymSystem.Application.Messaging.Behaviors;
using eGymSystem.Application.Modules.Training.Commands;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace eGymSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly);
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();

        services.AddScoped(typeof(ICommandBehavior<,>), typeof(CommandLoggingBehavior<,>));
        services.AddScoped(typeof(ICommandBehavior<,>), typeof(CommandValidationBehavior<,>));

        services.AddScoped<ICommandHandler<CreateTrainingRequestCommand, CreateTrainingRequestResult>, CreateTrainingRequestCommandHandler>();
        services.AddScoped<ICommandValidator<CreateTrainingRequestCommand>, CreateTrainingRequestCommandValidator>();
        services.AddSingleton(TimeProvider.System);

        return services;
    }
}
