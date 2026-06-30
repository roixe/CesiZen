namespace CesiZen.Api.DTOs.Admin;

public record CategorieDto(
    int Id,
    string Nom
    );
public record UpsertCategorieDto(
    string Nom
    );