namespace CesiZen.Api.DTOs.Auth
{
    public record RegisterRequestDto(
        string Nom,
        string Email,
        string Password
    );
}
