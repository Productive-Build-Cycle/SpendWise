namespace SpendWise.Application.DTOs;

public sealed record CategoryDto(
    Guid Id,
    string CategoryName,
    string? Description,
    string CategoryType,
    bool IsDefault,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);