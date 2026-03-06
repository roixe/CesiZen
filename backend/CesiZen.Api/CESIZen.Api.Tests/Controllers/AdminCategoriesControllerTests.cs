using CesiZen.Api.Controllers;
using CesiZen.Api.DTOs.Admin;
using CesiZen.Api.Tests.Helpers;
using CesiZen.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen.Api.Tests.Controllers;

public class AdminCategoriesControllerTests
{
    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoCategoriesExist()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new AdminCategoriesController(db);

        var result = await controller.GetAll();

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var categories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategorieDto>>().Subject;

        categories.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldReturnCategoriesOrderedByName()
    {
        using var db = TestDbContextFactory.Create();

        db.Categories.AddRange(
            new Categorie { Id = 1, Nom = "Sommeil" },
            new Categorie { Id = 2, Nom = "Anxiété" },
            new Categorie { Id = 3, Nom = "Stress" }
        );

        await db.SaveChangesAsync();

        var controller = new AdminCategoriesController(db);

        var result = await controller.GetAll();

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var categories = okResult.Value.Should().BeAssignableTo<IEnumerable<CategorieDto>>().Subject;

        categories.Select(c => c.Nom)
            .Should()
            .ContainInOrder("Anxiété", "Sommeil", "Stress");
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenNameIsTooShort()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new AdminCategoriesController(db);

        var dto = new UpsertCategorieDto("A");

        var result = await controller.Create(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnConflict_WhenCategoryAlreadyExists_IgnoringCase()
    {
        using var db = TestDbContextFactory.Create();

        db.Categories.Add(new Categorie
        {
            Id = 1,
            Nom = "Stress"
        });

        await db.SaveChangesAsync();

        var controller = new AdminCategoriesController(db);

        var dto = new UpsertCategorieDto("stress");

        var result = await controller.Create(dto);

        result.Result.Should().BeOfType<ConflictObjectResult>();
    }

    [Fact]
    public async Task Create_ShouldCreateCategory_WhenDtoIsValid()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new AdminCategoriesController(db);

        var dto = new UpsertCategorieDto("Respiration");

        var result = await controller.Create(dto);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var category = okResult.Value.Should().BeOfType<CategorieDto>().Subject;

        category.Id.Should().BeGreaterThan(0);
        category.Nom.Should().Be("Respiration");

        db.Categories.Should().ContainSingle(c => c.Nom == "Respiration");
    }

    [Fact]
    public async Task Create_ShouldTrimCategoryName()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new AdminCategoriesController(db);

        var dto = new UpsertCategorieDto("   Relaxation   ");

        var result = await controller.Create(dto);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var category = okResult.Value.Should().BeOfType<CategorieDto>().Subject;

        category.Nom.Should().Be("Relaxation");
        db.Categories.Should().ContainSingle(c => c.Nom == "Relaxation");
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenNameIsTooShort()
    {
        using var db = TestDbContextFactory.Create();

        db.Categories.Add(new Categorie { Id = 1, Nom = "Stress" });
        await db.SaveChangesAsync();

        var controller = new AdminCategoriesController(db);

        var dto = new UpsertCategorieDto("A");

        var result = await controller.Update(1, dto);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenCategoryDoesNotExist()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new AdminCategoriesController(db);

        var dto = new UpsertCategorieDto("Nouveau nom");

        var result = await controller.Update(999, dto);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_ShouldUpdateCategory_WhenDtoIsValid()
    {
        using var db = TestDbContextFactory.Create();

        db.Categories.Add(new Categorie
        {
            Id = 1,
            Nom = "Ancien nom"
        });

        await db.SaveChangesAsync();

        var controller = new AdminCategoriesController(db);

        var dto = new UpsertCategorieDto("Nouveau nom");

        var result = await controller.Update(1, dto);

        result.Should().BeOfType<NoContentResult>();

        var category = db.Categories.Single(c => c.Id == 1);
        category.Nom.Should().Be("Nouveau nom");
    }

    [Fact]
    public async Task Update_ShouldTrimCategoryName()
    {
        using var db = TestDbContextFactory.Create();

        db.Categories.Add(new Categorie
        {
            Id = 1,
            Nom = "Stress"
        });

        await db.SaveChangesAsync();

        var controller = new AdminCategoriesController(db);

        var dto = new UpsertCategorieDto("   Bien-être   ");

        var result = await controller.Update(1, dto);

        result.Should().BeOfType<NoContentResult>();

        var category = db.Categories.Single(c => c.Id == 1);
        category.Nom.Should().Be("Bien-être");
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenCategoryDoesNotExist()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new AdminCategoriesController(db);

        var result = await controller.Delete(999);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ShouldReturnConflict_WhenCategoryIsUsedByArticles()
    {
        using var db = TestDbContextFactory.Create();

        db.Categories.Add(new Categorie
        {
            Id = 1,
            Nom = "Stress"
        });

        db.Articles.Add(new Article
        {
            Id = 1,
            Titre = "Comprendre le stress",
            Contenu = "Contenu",
            Public = true,
            DatePublication = DateTime.UtcNow,
            CategorieId = 1
        });

        await db.SaveChangesAsync();

        var controller = new AdminCategoriesController(db);

        var result = await controller.Delete(1);

        result.Should().BeOfType<ConflictObjectResult>();
        db.Categories.Should().ContainSingle(c => c.Id == 1);
    }

    [Fact]
    public async Task Delete_ShouldRemoveCategory_WhenCategoryIsNotUsed()
    {
        using var db = TestDbContextFactory.Create();

        db.Categories.Add(new Categorie
        {
            Id = 1,
            Nom = "Respiration"
        });

        await db.SaveChangesAsync();

        var controller = new AdminCategoriesController(db);

        var result = await controller.Delete(1);

        result.Should().BeOfType<NoContentResult>();
        db.Categories.Should().BeEmpty();
    }
}