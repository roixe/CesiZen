namespace CesiZen.Api.DTOs;

public record ArticleDto(
    int Id,
    string Titre,
    string Contenu,
    DateTime? DatePublication,
    bool Public,
    int CategorieId
);