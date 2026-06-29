namespace CesiZen.Api.DTOs;

public record EnregistreDto(
    int Id,
    int ExerciceId,
    string ExerciceNom,
    DateTime DateDebut,
    int DureeEffectiveSec
);

public record HistoriqueDto(
    int Id,
    int UtilisateurId,
    DateTime Date,
    int DureeSec,
    List<EnregistreDto> Exercices
);

public record CreateHistoriqueDto(
    int ExerciceId,
    int DureeEffectiveSec
);