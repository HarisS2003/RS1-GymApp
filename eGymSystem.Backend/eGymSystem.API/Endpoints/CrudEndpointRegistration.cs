namespace eGymSystem.API.Endpoints;

internal static class CrudEndpointRegistration
{
    public static IEndpointRouteBuilder MapCrudSkeletonEndpoints(this IEndpointRouteBuilder app)
    {
        MapCrud(app, "/api/auth", "Auth");
        MapCrud(app, "/api/users", "Users");
        MapCrud(app, "/api/trainers", "Trainers");
        MapCrud(app, "/api/trainings", "Trainings");
        MapCrud(app, "/api/training-requests", "TrainingRequests");
        MapCrud(app, "/api/products", "Products");
        MapCrud(app, "/api/basket", "Basket");
        MapCrud(app, "/api/orders", "Orders");
        MapCrud(app, "/api/memberships", "Memberships");
        MapCrud(app, "/api/admin", "Admin");
        MapCrud(app, "/api/notifications", "Notifications");
        return app;
    }

    private static void MapCrud(IEndpointRouteBuilder app, string route, string tag)
    {
        var group = app.MapGroup(route).WithTags(tag);
        group.MapGet("/", () => Results.StatusCode(StatusCodes.Status501NotImplemented));
        group.MapGet("/{id:int}", (int id) => Results.StatusCode(StatusCodes.Status501NotImplemented));
        group.MapPost("/", () => Results.StatusCode(StatusCodes.Status501NotImplemented));
        group.MapPut("/{id:int}", (int id) => Results.StatusCode(StatusCodes.Status501NotImplemented));
        group.MapDelete("/{id:int}", (int id) => Results.StatusCode(StatusCodes.Status501NotImplemented));
    }
}
