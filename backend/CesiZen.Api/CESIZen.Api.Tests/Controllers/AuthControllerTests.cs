using CesiZen.Api.Controllers;
using CesiZen.Api.DTOs.Auth;
using CesiZen.Api.Tests.Helpers;
using CesiZen.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace CesiZen.Api.Tests.Controllers;

public class AuthControllerTests
{
    private static IConfiguration BuildConfiguration()
    {
        var settings = new Dictionary<string, string?>
        {
            ["Jwt:Issuer"] = "CesiZenTest",
            ["Jwt:Audience"] = "CesiZenTestAudience",
            ["Jwt:Key"] = "super_secret_test_key_for_cesizen_2025"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();
    }

    [Fact]
    public async Task Register_ShouldCreateUser_AndReturnToken()
    {
        using var db = TestDbContextFactory.Create();
        var config = BuildConfiguration();
        var controller = new AuthController(db, config);

        var dto = new RegisterRequestDto(
            "Alice",
            "alice@test.com",
            "Password123!"
        );

        var result = await controller.Register(dto);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;

        response.Token.Should().NotBeNullOrWhiteSpace();
        response.User.Email.Should().Be("alice@test.com");

        db.Utilisateurs.Should().ContainSingle(u => u.Email == "alice@test.com");
    }

    [Fact]
    public async Task Register_ShouldReturnConflict_WhenEmailAlreadyExists()
    {
        using var db = TestDbContextFactory.Create();
        var config = BuildConfiguration();

        db.Utilisateurs.Add(new User
        {
            Nom = "Alice",
            Email = "alice@test.com",
            MotDePasseHash = "hash",
            Role = "USER",
            Actif = true,
            DateCreation = DateTime.UtcNow
        });

        await db.SaveChangesAsync();

        var controller = new AuthController(db, config);

        var dto = new RegisterRequestDto(
            "Alice 2",
            "alice@test.com",
            "Password123!"
        );

        var result = await controller.Register(dto);

        result.Result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
    {
        using var db = TestDbContextFactory.Create();
        var config = BuildConfiguration();

        var hasher = new PasswordHasher<User>();
        var user = new User
        {
            Nom = "Bob",
            Email = "bob@test.com",
            Role = "USER",
            Actif = true,
            DateCreation = DateTime.UtcNow
        };
        user.MotDePasseHash = hasher.HashPassword(user, "GoodPassword123!");

        db.Utilisateurs.Add(user);
        await db.SaveChangesAsync();

        var controller = new AuthController(db, config);

        var dto = new LoginRequestDto(
            "bob@test.com",
            "WrongPassword!"
        );

        var result = await controller.Login(dto);

        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_ShouldReturnUnauthorized_WhenUserIsInactive()
    {
        using var db = TestDbContextFactory.Create();
        var config = BuildConfiguration();

        var hasher = new PasswordHasher<User>();
        var user = new User
        {
            Nom = "Bob",
            Email = "bob@test.com",
            Role = "USER",
            Actif = false,
            DateCreation = DateTime.UtcNow
        };
        user.MotDePasseHash = hasher.HashPassword(user, "GoodPassword123!");

        db.Utilisateurs.Add(user);
        await db.SaveChangesAsync();

        var controller = new AuthController(db, config);

        var dto = new LoginRequestDto(
            "bob@test.com",
            "GoodPassword123!"
        );

        var result = await controller.Login(dto);

        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        using var db = TestDbContextFactory.Create();
        var config = BuildConfiguration();

        var hasher = new PasswordHasher<User>();
        var user = new User
        {
            Nom = "Bob",
            Email = "bob@test.com",
            Role = "USER",
            Actif = true,
            DateCreation = DateTime.UtcNow
        };
        user.MotDePasseHash = hasher.HashPassword(user, "GoodPassword123!");

        db.Utilisateurs.Add(user);
        await db.SaveChangesAsync();

        var controller = new AuthController(db, config);

        var dto = new LoginRequestDto(
            "bob@test.com",
            "GoodPassword123!"
        );

        var result = await controller.Login(dto);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<AuthResponseDto>().Subject;

        response.Token.Should().NotBeNullOrWhiteSpace();
        response.User.Email.Should().Be("bob@test.com");
    }
}