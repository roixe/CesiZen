using CesiZen.Api.Controllers;
using CesiZen.Api.DTOs;
using CesiZen.Api.Tests.Helpers;
using CesiZen.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen.Api.Tests.Controllers;

public class CategoriesControllerTests
{
    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoCategoriesExist()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();
        var controller = new CategoriesController(db);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var categories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategorieDto>>().Subject;

        categories.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllCategories()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();

        db.Categories.AddRange(
            new Categorie { Id = 1, Nom = "Stress" },
            new Categorie { Id = 2, Nom = "Sommeil" }
        );

        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var categories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategorieDto>>().Subject;

        categories.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_ShouldReturnCategoriesOrderedByName()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();

        db.Categories.AddRange(
            new Categorie { Id = 1, Nom = "Sommeil" },
            new Categorie { Id = 2, Nom = "Stress" },
            new Categorie { Id = 3, Nom = "Anxiété" }
        );

        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var categories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategorieDto>>().Subject;

        categories.Select(c => c.Nom)
            .Should()
            .ContainInOrder("Anxiété", "Sommeil", "Stress");
    }

    [Fact]
    public async Task GetAll_ShouldReturnCorrectDto()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();

        db.Categories.Add(new Categorie
        {
            Id = 1,
            Nom = "Respiration"
        });

        await db.SaveChangesAsync();

        var controller = new CategoriesController(db);

        // Act
        var result = await controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var categories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategorieDto>>().Subject;

        var cat = categories.Should().ContainSingle().Subject;

        cat.Id.Should().Be(1);
        cat.Nom.Should().Be("Respiration");
    }
}