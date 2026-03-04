namespace CesiZen.Api.DTOs.Auth
{

    public record LoginRequestDto(
        string Email,
        string Password
    );
}
