using CesiZen.Api.Controllers;
using CesiZen.Api.Tests.Helpers;
using CesiZen.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Linq;

namespace CesiZen.Api.Tests.Controllers;

public class UserControllerTests
{
    [Fact]
    public async Task GetUsers_ShouldReturnOk_WithEmptyList_WhenNoUsersExist()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new UserController(db);

        var result = await controller.GetUsers();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var users = (okResult.Value as IEnumerable)!.Cast<object>().ToList();

        users.Should().NotBeNull();
        users.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUsers_ShouldReturnAllUsers_WhenUsersExist()
    {
        using var db = TestDbContextFactory.Create();

        db.Utilisateurs.AddRange(
            new User
            {
                Id = 1, Nom = "Alice", Email = "alice@test.com",
                MotDePasseHash = "hash1", Role = "USER", Actif = true,
                DateCreation = DateTime.UtcNow
            },
            new User
            {
                Id = 2, Nom = "Bob", Email = "bob@test.com",
                MotDePasseHash = "hash2", Role = "ADMIN", Actif = true,
                DateCreation = DateTime.UtcNow
            }
        );
        await db.SaveChangesAsync();

        var controller = new UserController(db);
        var result = await controller.GetUsers();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var users = (okResult.Value as IEnumerable)!.Cast<object>().ToList();

        users.Should().HaveCount(2);

        // Vérifie qu'on a bien Alice et Bob, et SURTOUT pas le hash de mot de passe (sécurité)
        var emails = users.Select(u => u.GetType().GetProperty("Email")!.GetValue(u)?.ToString());
        emails.Should().Contain("alice@test.com");
        emails.Should().Contain("bob@test.com");

        users.First().GetType().GetProperty("MotDePasseHash").Should().BeNull(
            "le hash de mot de passe ne doit jamais être exposé par l'API");
    }

    [Fact]
    public async Task GetUsers_ShouldReturnUsers_WithExpectedProperties()
    {
        using var db = TestDbContextFactory.Create();

        db.Utilisateurs.Add(new User
        {
            Id = 1, Nom = "Charlie", Email = "charlie@test.com",
            MotDePasseHash = "hash3", Role = "USER", Actif = false,
            DateCreation = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var controller = new UserController(db);
        var result = await controller.GetUsers();

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var users = (okResult.Value as IEnumerable)!.Cast<object>().ToList();

        users.Should().ContainSingle();
        var user = users.First();
        var t = user.GetType();

        t.GetProperty("Nom")!.GetValue(user).Should().Be("Charlie");
        t.GetProperty("Email")!.GetValue(user).Should().Be("charlie@test.com");
        t.GetProperty("Role")!.GetValue(user).Should().Be("USER");
        t.GetProperty("Actif")!.GetValue(user).Should().Be(false);
    }
}