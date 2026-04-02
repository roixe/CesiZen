using System.Security.Claims;
using CesiZen.Api.Controllers;
using CesiZen.Api.DTOs;
using CesiZen.Api.Tests.Helpers;
using CesiZen.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen.Api.Tests.Controllers;

public class HistoriquesControllerTests
{
    private static ClaimsPrincipal BuildUser(int userId)
    {
        return new ClaimsPrincipal(
            new ClaimsIdentity(
                new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Role, "USER")
                },
                "TestAuth"
            )
        );
    }

    [Fact]
    public async Task GetMe_ShouldReturnUnauthorized_WhenUserIdClaimIsMissing()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new HistoriquesController(db);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        var result = await controller.GetMe();

        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenExerciseDoesNotExist()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new HistoriquesController(db);

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = BuildUser(1)
            }
        };

        var dto = new CreateHistoriqueDto(
            ExerciceId: 999,
            DureeEffectiveSec: 120
        );

        var result = await controller.Create(dto);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_ShouldCreateHistorique_AndEnregistrement_WhenExerciseExists()
    {
        using var db = TestDbContextFactory.Create();

        db.Exercices.Add(new Exercice
        {
            Id = 1,
            Nom = "5-5",
            InspireSec = 5,
            ApneeSec = 0,
            ExpireSec = 5
        });

        await db.SaveChangesAsync();

        var controller = new HistoriquesController(db);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = BuildUser(7)
            }
        };

        var dto = new CreateHistoriqueDto(
            ExerciceId: 1,
            DureeEffectiveSec: 180
        );

        var result = await controller.Create(dto);

        result.Should().BeOfType<OkObjectResult>();

        db.Historiques.Should().ContainSingle(h => h.UtilisateurId == 7 && h.DureeSec == 180);
        db.Enregistrements.Should().ContainSingle(e => e.ExerciceId == 1 && e.DureeEffectiveSec == 180);
    }
}