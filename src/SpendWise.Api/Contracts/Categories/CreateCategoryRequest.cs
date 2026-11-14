namespace SpendWise.Api.Contracts.Categories;

public sealed record CreateCategoryRequest(
    string CategoryName,
    string CategoryType,
    string? Description);