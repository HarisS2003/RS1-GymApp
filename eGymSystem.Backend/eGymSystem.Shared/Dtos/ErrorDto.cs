namespace eGymSystem.Shared.Dtos;

public sealed class ErrorDto
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? TraceId { get; set; }
    public string? Details { get; set; }
}
