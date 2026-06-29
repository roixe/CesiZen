
namespace CesiZen.Api.DTOs.Admin;

public record UpsertArticleDto(
    string Titre,
    string Contenu,
    int CategorieId,
    bool Public
);