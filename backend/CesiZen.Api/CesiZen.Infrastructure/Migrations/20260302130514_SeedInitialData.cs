using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CesiZen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "categorie",
                columns: new[] { "Id", "Nom" },
                values: new object[,]
                {
                    { 1, "Respiration" },
                    { 2, "Gestion du stress" }
                });

            migrationBuilder.InsertData(
                table: "exercice",
                columns: new[] { "Id", "Apnee2Sec", "ApneeSec", "Cycles", "Description", "DureeTotaleSec", "ExpireSec", "InspireSec", "Nom", "Public", "Type" },
                values: new object[,]
                {
                    { 1, 0, 0, 30, "Inspire 5s, expire 5s. Environ 5 minutes.", 300, 5, 5, "Cohérence cardiaque 5-5", true, "RESPIRATION" },
                    { 2, 0, 7, 6, "Inspire 4s, apnée 7s, expire 8s.", 114, 8, 4, "4-7-8", true, "RESPIRATION" },
                    { 3, 4, 4, 8, "Inspire 4s, apnée 4s, expire 4s, apnée 4s.", 128, 4, 4, "Box breathing 4-4-4-4", true, "RESPIRATION" }
                });

            migrationBuilder.InsertData(
                table: "utilisateur",
                columns: new[] { "Id", "Actif", "DateCreation", "Email", "MotDePasseHash", "Nom", "Role" },

                values: new object[] { 1, true, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@cesizen.local", "AQAAAAEAAYagAAAAEN7+03cLBoJ2S8AK7lV7Qkr1bI40I4yPC/sqTXaG7PPnzTGWXGU2Z095QpWrDvx//w==", "Admin", "ADMIN" });
            migrationBuilder.InsertData(
                table: "article",
                columns: new[] { "Id", "CategorieId", "Contenu", "DatePublication", "GereParUserId", "Public", "Titre" },
                values: new object[] { 1, 2, "La respiration lente et régulière stimule le système parasympathique et favorise le retour au calme.", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, "Pourquoi la respiration aide à calmer le stress" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "article",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "categorie",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "exercice",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "exercice",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "exercice",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "categorie",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "utilisateur",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
