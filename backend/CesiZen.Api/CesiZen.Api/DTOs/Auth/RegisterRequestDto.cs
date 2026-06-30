namespace CesiZen.Api.DTOs.Auth
{
    public record RegisterRequestDto(
        string Nom,
        string Email,
        string Password,
        bool Consentement = false // [SÉCU 4] consentement RGPD
    );
}
