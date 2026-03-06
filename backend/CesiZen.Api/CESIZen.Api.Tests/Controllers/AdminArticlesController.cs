using System.Security.Claims;
using CesiZen.Api.Controllers;
using CesiZen.Api.DTOs;
using CesiZen.Api.DTOs.Admin;
using CesiZen.Api.Tests.Helpers;
using CesiZen.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen.Api.Tests.Controllers;

public class AdminArticlesControllerTests
{
    private static ClaimsPrincipal BuildAdminUser(int userId)
    {
        return new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Role, "ADMIN")
                },
                "TestAuth"
            )
        );
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllArticles()
    {
        using var db = TestDbContextFactory.Create();

        db.Categories.Add(new Categorie { Id = 1, Nom = "Stress" });

        db.Articles.AddRange(
            new Article
            {
                Id = 1,
                Titre = "Article 1",
                Contenu = "Contenu 1",
                Public = true,
                DatePublication = new DateTime(2024, 1, 2),
                CategorieId = 1
            },
            new Article
            {
                Id = 2,
                Titre = "Article 2",
                Contenu = "Contenu 2",
                Public = false,
                DatePublication = null,
                CategorieId = 1
            }
        );

        await db.SaveChangesAsync();

        var controller = new AdminArticlesController(db);

        var result = await controller.GetAll();

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var articles = okResult.Value.Should().BeAssignableTo<IEnumerable<ArticleDto>>().Subject;

        articles.Should().HaveCount(2);
        articles.First().Titre.Should().Be("Article 1");
    }

    [Fact]
    public async Task Create_ShouldReturnUnauthorized_WhenUserIdClaimIsMissing()
    {
        using var db = TestDbContextFactory.Create();

        db.Categories.Add(new Categorie { Id = 1, Nom = "Stress" });
        await db.SaveChangesAsync();

        var controller = new AdminArticlesController(db);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        var dto = new UpsertArticleDto("Titre", "Contenu", 1, true);

        var result = await controller.Create(dto);

        result.Result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenCategoryDoesNotExist()
    {
        using var db = TestDbContextFactory.Create();

        var controller = new AdminArticlesController(db);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = BuildAdminUser(7)
            }
        };

        var dto = new UpsertArticleDto("Titre", "Contenu", 999, true);

        var result = await controller.Create(dto);

        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_ShouldCreatePublicArticle_WithPublicationDate()
    {
        using var db = TestDbContextFactory.Create();

        db.Categories.Add(new Categorie { Id = 1, Nom = "Stress" });
        await db.SaveChangesAsync();

        var controller = new AdminArticlesController(db);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = BuildAdminUser(7)
            }
        };

        var dto = new UpsertArticleDto("Titre public", "Contenu public", 1, true);

        var result = await controller.Create(dto);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var article = okResult.Value.Should().BeOfType<ArticleDto>().Subject;

        article.Titre.Should().Be("Titre public");
        article.Public.Should().BeTrue();
        article.DatePublication.Should().NotBeNull();

        db.Articles.Should().ContainSingle(a =>
            a.Titre == "Titre public" &&
            a.CategorieId == 1 &&
            a.Public &&
            a.GereParUserId == 7);
    }

    [Fact]
    public async Task Create_ShouldCreatePrivateArticle_WithoutPublicationDate()
    {
        using var db = TestDbContextFactory.Create();

        db.Categories.Add(new Categorie { Id = 1, Nom = "Stress" });
        await db.SaveChangesAsync();

        var controller = new AdminArticlesController(db);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = BuildAdminUser(12)
            }
        };

        var dto = new UpsertArticleDto("Titre privé", "Contenu privé", 1, false);

        var result = await controller.Create(dto);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var article = okResult.Value.Should().BeOfType<ArticleDto>().Subject;

        article.Titre.Should().Be("Titre privé");
        article.Public.Should().BeFalse();
        article.DatePublication.Should().BeNull();

        db.Articles.Should().ContainSingle(a =>
            a.Titre == "Titre privé" &&
            !a.Public &&
            a.DatePublication == null &&
            a.GereParUserId == 12);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenArticleDoesNotExist()
    {
        using var db = TestDbContextFactory.Create();

        db.Categories.Add(new Categorie { Id = 1, Nom = "Stress" });
        await db.SaveChangesAsync();

        var controller = new AdminArticlesController(db);

        var dto = new UpsertArticleDto("Titre", "Contenu", 1, true);

        var result = await controller.Update(999, dto);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenCategoryDoesNotExist()
    {
        using var db = TestDbContextFactory.Create();

        db.Categories.Add(new Categorie { Id = 1, Nom = "Stress" });

        db.Articles.Add(new Article
        {
            Id = 1,
            Titre = "Titre",
            Contenu = "Contenu",
            Public = false,
            CategorieId = 1
        });

        await db.SaveChangesAsync();

        var controller = new AdminArticlesController(db);

        var dto = new UpsertArticleDto("Nouveau titre", "Nouveau contenu", 999, true);

        var result = await controller.Update(1, dto);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_ShouldSetPublicationDate_WhenPublishingForTheFirstTime()
    {
        using var db = TestDbContextFactory.Create();

        db.Categories.Add(new Categorie { Id = 1, Nom = "Stress" });

        db.Articles.Add(new Article
        {
            Id = 1,
            Titre = "Titre",
            Contenu = "Contenu",
            Public = false,
            DatePublication = null,
            CategorieId = 1
        });

        await db.SaveChangesAsync();

        var controller = new AdminArticlesController(db);

        var dto = new UpsertArticleDto("Titre modifié", "Contenu modifié", 1, true);

        var result = await controller.Update(1, dto);

        result.Should().BeOfType<NoContentResult>();

        var article = db.Articles.Single(a => a.Id == 1);
        article.Public.Should().BeTrue();
        article.DatePublication.Should().NotBeNull();
        article.Titre.Should().Be("Titre modifié");
    }

    [Fact]
    public async Task Update_ShouldClearPublicationDate_WhenUnpublishing()
    {
        using var db = TestDbContextFactory.Create();

        db.Categories.Add(new Categorie { Id = 1, Nom = "Stress" });

        db.Articles.Add(new Article
        {
            Id = 1,
            Titre = "Titre",
            Contenu = "Contenu",
            Public = true,
            DatePublication = DateTime.UtcNow,
            CategorieId = 1
        });

        await db.SaveChangesAsync();

        var controller = new AdminArticlesController(db);

        var dto = new UpsertArticleDto("Titre modifié", "Contenu modifié", 1, false);

        var result = await controller.Update(1, dto);

        result.Should().BeOfType<NoContentResult>();

        var article = db.Articles.Single(a => a.Id == 1);
        article.Public.Should().BeFalse();
        article.DatePublication.Should().BeNull();
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenArticleDoesNotExist()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new AdminArticlesController(db);

        var result = await controller.Delete(999);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ShouldRemoveArticle_WhenArticleExists()
    {
        using var db = TestDbContextFactory.Create();

        db.Categories.Add(new Categorie { Id = 1, Nom = "Stress" });

        db.Articles.Add(new Article
        {
            Id = 1,
            Titre = "Titre",
            Contenu = "Contenu",
            Public = true,
            DatePublication = DateTime.UtcNow,
            CategorieId = 1
        });

        await db.SaveChangesAsync();

        var controller = new AdminArticlesController(db);

        var result = await controller.Delete(1);

        result.Should().BeOfType<NoContentResult>();
        db.Articles.Should().BeEmpty();
    }
}