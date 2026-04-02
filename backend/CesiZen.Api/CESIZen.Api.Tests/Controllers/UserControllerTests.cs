using CesiZen.Api.Controllers;
using CesiZen.Api.Tests.Helpers;
using CesiZen.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen.Api.Tests.Controllers;

public class UserControllerTests
{
    [Fact]
    public async Task GetUsers_ShouldReturnOk_WithEmptyList_WhenNoUsersExist()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var controller = new UserController(db);

        // Act
        var result = await controller.GetUsers();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var users = okResult.Value.Should().BeAssignableTo<IEnumerable<User>>().Subject;

        users.Should().NotBeNull();
        users.Should().BeEmpty();
    }

    [Fact]
    public async Task GetUsers_ShouldReturnAllUsers_WhenUsersExist()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();

        db.Utilisateurs.AddRange(
            new User
            {
                Id = 1,
                Nom = "Alice",
                Email = "alice@test.com",
                MotDePasseHash = "hash1",
                Role = "USER",
                Actif = true,
                DateCreation = DateTime.UtcNow
            },
            new User
            {
                Id = 2,
                Nom = "Bob",
                Email = "bob@test.com",
                MotDePasseHash = "hash2",
                Role = "ADMIN",
                Actif = true,
                DateCreation = DateTime.UtcNow
            }
        );

        await db.SaveChangesAsync();

        var controller = new UserController(db);

        // Act
        var result = await controller.GetUsers();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var users = okResult.Value.Should().BeAssignableTo<IEnumerable<User>>().Subject;

        users.Should().HaveCount(2);
        users.Should().Contain(u => u.Email == "alice@test.com");
        users.Should().Contain(u => u.Email == "bob@test.com");
    }

    [Fact]
    public async Task GetUsers_ShouldReturnUsers_WithExpectedProperties()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();

        db.Utilisateurs.Add(new User
        {
            Id = 1,
            Nom = "Charlie",
            Email = "charlie@test.com",
            MotDePasseHash = "hash3",
            Role = "USER",
            Actif = false,
            DateCreation = DateTime.UtcNow
        });

        await db.SaveChangesAsync();

        var controller = new UserController(db);

        // Act
        var result = await controller.GetUsers();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var users = okResult.Value.Should().BeAssignableTo<IEnumerable<User>>().Subject;

        var user = users.Should().ContainSingle().Subject;
        user.Nom.Should().Be("Charlie");
        user.Email.Should().Be("charlie@test.com");
        user.Role.Should().Be("USER");
        user.Actif.Should().BeFalse();
    }
}