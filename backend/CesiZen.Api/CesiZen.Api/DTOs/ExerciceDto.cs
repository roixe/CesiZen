namespace CesiZen.Api.DTOs;

public record ExerciceDto(
    int Id,
    string Nom,
    string Type,
    string? Description,
    bool Public,
    int InspireSec,
    int ApneeSec,
    int ExpireSec,
    int Apnee2Sec,
    int Cycles,
    int DureeTotaleSec
);