namespace eGymSystem.Shared.Dtos;

public sealed record ApiErrorResponseDto(
    string ErrorCode,
    string Message,
    IReadOnlyCollection<string> Details,
    string TraceId,
    bool Retryable);
