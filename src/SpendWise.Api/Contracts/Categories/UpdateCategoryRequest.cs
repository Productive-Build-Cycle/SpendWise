namespace SpendWise.Api.Contracts.Categories;

public sealed record UpdateCategoryRequest(
    string CategoryName,
    string? Description);