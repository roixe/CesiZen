namespace CesiZen.Api.DTOs.Auth
{
    public record AuthUserDto(
        int Id,
        string Nom,
        string Email,
        string Role
    );

    public record AuthResponseDto(
        string Token,
        AuthUserDto User
    );
}
