using CesiZen.Api.Controllers;
using CesiZen.Api.DTOs;
using CesiZen.Api.Tests.Helpers;
using CesiZen.Domain.Entities;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace CesiZen.Api.Tests.Controllers;

public class ExercicesControllerTests
{
    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoExercisesExist()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new ExercicesController(db);

        var result = await controller.GetAll(null, null);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var exercices = okResult.Value.Should().BeAssignableTo<IEnumerable<ExerciceDto>>().Subject;

        exercices.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllExercises_WhenNoFilter()
    {
        using var db = TestDbContextFactory.Create();

        db.Exercices.AddRange(
            new Exercice
            {
                Id = 1,
                Nom = "5-5",
                Type = "RESPIRATION",
                Description = "Cohérence cardiaque",
                Public = true,
                InspireSec = 5,
                ApneeSec = 0,
                ExpireSec = 5,
                Apnee2Sec = 0,
                Cycles = 6,
                DureeTotaleSec = 60
            },
            new Exercice
            {
                Id = 2,
                Nom = "7-4-8",
                Type = "RESPIRATION",
                Description = "Relaxation",
                Public = false,
                InspireSec = 7,
                ApneeSec = 4,
                ExpireSec = 8,
                Apnee2Sec = 0,
                Cycles = 5,
                DureeTotaleSec = 95
            }
        );

        await db.SaveChangesAsync();

        var controller = new ExercicesController(db);

        var result = await controller.GetAll(null, null);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var exercices = okResult.Value.Should().BeAssignableTo<IEnumerable<ExerciceDto>>().Subject;

        exercices.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_ShouldFilterByType()
    {
        using var db = TestDbContextFactory.Create();

        db.Exercices.AddRange(
            new Exercice
            {
                Id = 1,
                Nom = "5-5",
                Type = "RESPIRATION",
                Description = "Respiration",
                Public = true,
                InspireSec = 5,
                ApneeSec = 0,
                ExpireSec = 5,
                Apnee2Sec = 0,
                Cycles = 6,
                DureeTotaleSec = 60
            },
            new Exercice
            {
                Id = 2,
                Nom = "Méditation 10 min",
                Type = "RELAXATION",
                Description = "Relaxation",
                Public = true,
                InspireSec = 0,
                ApneeSec = 0,
                ExpireSec = 0,
                Apnee2Sec = 0,
                Cycles = 0,
                DureeTotaleSec = 600
            }
        );

        await db.SaveChangesAsync();

        var controller = new ExercicesController(db);

        var result = await controller.GetAll("RESPIRATION", null);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var exercices = okResult.Value.Should().BeAssignableTo<IEnumerable<ExerciceDto>>().Subject;

        exercices.Should().HaveCount(1);
        exercices.Single().Type.Should().Be("RESPIRATION");
    }

    [Fact]
    public async Task GetAll_ShouldFilterByPublicFlag()
    {
        using var db = TestDbContextFactory.Create();

        db.Exercices.AddRange(
            new Exercice
            {
                Id = 1,
                Nom = "5-5",
                Type = "RESPIRATION",
                Description = "Public",
                Public = true,
                InspireSec = 5,
                ApneeSec = 0,
                ExpireSec = 5,
                Apnee2Sec = 0,
                Cycles = 6,
                DureeTotaleSec = 60
            },
            new Exercice
            {
                Id = 2,
                Nom = "7-4-8",
                Type = "RESPIRATION",
                Description = "Privé",
                Public = false,
                InspireSec = 7,
                ApneeSec = 4,
                ExpireSec = 8,
                Apnee2Sec = 0,
                Cycles = 5,
                DureeTotaleSec = 95
            }
        );

        await db.SaveChangesAsync();

        var controller = new ExercicesController(db);

        var result = await controller.GetAll(null, true);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var exercices = okResult.Value.Should().BeAssignableTo<IEnumerable<ExerciceDto>>().Subject;

        exercices.Should().HaveCount(1);
        exercices.Single().Public.Should().BeTrue();
    }

    [Fact]
    public async Task GetAll_ShouldFilterByTypeAndPublic()
    {
        using var db = TestDbContextFactory.Create();

        db.Exercices.AddRange(
            new Exercice
            {
                Id = 1,
                Nom = "5-5",
                Type = "RESPIRATION",
                Description = "Public respiration",
                Public = true,
                InspireSec = 5,
                ApneeSec = 0,
                ExpireSec = 5,
                Apnee2Sec = 0,
                Cycles = 6,
                DureeTotaleSec = 60
            },
            new Exercice
            {
                Id = 2,
                Nom = "7-4-8",
                Type = "RESPIRATION",
                Description = "Privé respiration",
                Public = false,
                InspireSec = 7,
                ApneeSec = 4,
                ExpireSec = 8,
                Apnee2Sec = 0,
                Cycles = 5,
                DureeTotaleSec = 95
            },
            new Exercice
            {
                Id = 3,
                Nom = "Relaxation",
                Type = "RELAXATION",
                Description = "Public relaxation",
                Public = true,
                InspireSec = 0,
                ApneeSec = 0,
                ExpireSec = 0,
                Apnee2Sec = 0,
                Cycles = 0,
                DureeTotaleSec = 600
            }
        );

        await db.SaveChangesAsync();

        var controller = new ExercicesController(db);

        var result = await controller.GetAll("RESPIRATION", true);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var exercices = okResult.Value.Should().BeAssignableTo<IEnumerable<ExerciceDto>>().Subject;

        exercices.Should().HaveCount(1);
        exercices.Single().Nom.Should().Be("5-5");
    }

    [Fact]
    public async Task GetAll_ShouldReturnExercisesOrderedByName()
    {
        using var db = TestDbContextFactory.Create();

        db.Exercices.AddRange(
            new Exercice
            {
                Id = 1,
                Nom = "7-4-8",
                Type = "RESPIRATION",
                Description = "Relaxation",
                Public = true,
                InspireSec = 7,
                ApneeSec = 4,
                ExpireSec = 8,
                Apnee2Sec = 0,
                Cycles = 5,
                DureeTotaleSec = 95
            },
            new Exercice
            {
                Id = 2,
                Nom = "4-6",
                Type = "RESPIRATION",
                Description = "Apaisement",
                Public = true,
                InspireSec = 4,
                ApneeSec = 0,
                ExpireSec = 6,
                Apnee2Sec = 0,
                Cycles = 6,
                DureeTotaleSec = 60
            },
            new Exercice
            {
                Id = 3,
                Nom = "5-5",
                Type = "RESPIRATION",
                Description = "Cohérence",
                Public = true,
                InspireSec = 5,
                ApneeSec = 0,
                ExpireSec = 5,
                Apnee2Sec = 0,
                Cycles = 6,
                DureeTotaleSec = 60
            }
        );

        await db.SaveChangesAsync();

        var controller = new ExercicesController(db);

        var result = await controller.GetAll(null, null);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var exercices = okResult.Value.Should().BeAssignableTo<IEnumerable<ExerciceDto>>().Subject;

        exercices.Select(e => e.Nom)
            .Should()
            .ContainInOrder("4-6", "5-5", "7-4-8");
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenExerciseDoesNotExist()
    {
        using var db = TestDbContextFactory.Create();
        var controller = new ExercicesController(db);

        var result = await controller.GetById(999);

        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetById_ShouldReturnExercise_WhenExerciseExists()
    {
        using var db = TestDbContextFactory.Create();

        db.Exercices.Add(new Exercice
        {
            Id = 1,
            Nom = "5-5",
            Type = "RESPIRATION",
            Description = "Cohérence cardiaque",
            Public = true,
            InspireSec = 5,
            ApneeSec = 0,
            ExpireSec = 5,
            Apnee2Sec = 0,
            Cycles = 6,
            DureeTotaleSec = 60
        });

        await db.SaveChangesAsync();

        var controller = new ExercicesController(db);

        var result = await controller.GetById(1);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var exercice = okResult.Value.Should().BeOfType<ExerciceDto>().Subject;

        exercice.Id.Should().Be(1);
        exercice.Nom.Should().Be("5-5");
        exercice.Type.Should().Be("RESPIRATION");
        exercice.Public.Should().BeTrue();
    }
}