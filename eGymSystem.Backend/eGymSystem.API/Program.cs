using eGymSystem.Application;
using eGymSystem.Application.Abstractions.Messaging;
using eGymSystem.Application.Abstractions.Validation;
using eGymSystem.Application.Modules.Training.Commands;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/api/v1/training-requests", async (
    CreateTrainingRequestHttpRequest request,
    ICommandDispatcher dispatcher,
    CancellationToken cancellationToken) =>
{
    try
    {
        var command = new CreateTrainingRequestCommand(
            request.UserId,
            request.TrainerId,
            request.RequestedTimeUtc);

        var result = await dispatcher.Send(command, cancellationToken);
        return Results.Created($"/api/v1/training-requests/{result.RequestId}", result);
    }
    catch (CommandValidationException ex)
    {
        var errors = ex.Errors
            .Select((error, idx) => new { idx, error })
            .ToDictionary(x => $"rule_{x.idx + 1}", x => new[] { x.error });

        return Results.ValidationProblem(errors, statusCode: StatusCodes.Status400BadRequest);
    }
})
.WithName("CreateTrainingRequest")
.WithOpenApi();

app.Run();

public sealed record CreateTrainingRequestHttpRequest(int UserId, int TrainerId, DateTime RequestedTimeUtc);
