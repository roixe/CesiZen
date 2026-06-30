using CesiZen.Api.Controllers;
using CesiZen.Api.DTOs;
using CesiZen.Api.Tests.Helpers;
using CesiZen.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen.Api.Tests.Controllers;

public class ArticlesControllerTests
{
    [Fact]
    public async Task GetAll_ShouldReturnAllArticles_WhenNoFilter()
    {
        // Arrange
        using var db = TestDbContextFactory.Create();

        db.Articles.AddRange(
            new Article
            {
                Id = 1,
                Titre = "Article public",
                Contenu = "Contenu 1",
                Public = true,
                DatePublication = new DateTime(2024, 1, 2),
                CategorieId = 1
            },
            new Article
            {
                Id = 2,
                Titre = "Article privé",
                Contenu = "Contenu 2",
                Public = false,
                DatePublication = new DateTime(2024, 1, 1),
                CategorieId = 1
            }
        );

        await db.SaveChangesAsync();

        var controller = new ArticlesController(db);

        // Act
        var result = await controller.GetAll(null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var articles = okResult.Value.Should().BeAssignableTo<IEnumerable<ArticleDto>>().Subject;

        articles.Should().HaveCount(2);
        articles.First().Titre.Should().Be("Article public");
    }

    [Fact]
    public async Task GetAll_ShouldFilterPublicArticles()
    {
        using var db = TestDbContextFactory.Create();

        db.Articles.AddRange(
            new Article
            {
                Id = 1,
                Titre = "Public",
                Contenu = "Contenu",
                Public = true,
                DatePublication = DateTime.UtcNow,
                CategorieId = 1
            },
            new Article
            {
                Id = 2,
                Titre = "Privé",
                Contenu = "Contenu",
                Public = false,
                DatePublication = DateTime.UtcNow.AddDays(-1),
                CategorieId = 1
            }
        );

        await db.SaveChangesAsync();

        var controller = new ArticlesController(db);

        var result = await controller.GetAll(true);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var articles = okResult.Value.Should().BeAssignableTo<IEnumerable<ArticleDto>>().Subject;

        articles.Should().HaveCount(1);
        articles.Single().Public.Should().BeTrue();
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenArticleDoesNotExist()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new ArticlesController(db);

        var result = await controller.GetById(999);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetById_ShouldReturnArticle_WhenArticleExists()
    {
        using var db = TestDbContextFactory.Create();

        db.Articles.Add(new Article
        {
            Id = 1,
            Titre = "Stress",
            Contenu = "Texte",
            Public = true,
            DatePublication = DateTime.UtcNow,
            CategorieId = 2
        });

        await db.SaveChangesAsync();

        var controller = new ArticlesController(db);

        var result = await controller.GetById(1);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var article = okResult.Value.Should().BeOfType<ArticleDto>().Subject;

        article.Id.Should().Be(1);
        article.Titre.Should().Be("Stress");
    }
}