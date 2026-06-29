using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CesiZen.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class sECCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "utilisateur");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "utilisateur",
                newName: "MotDePasseHash");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "utilisateur",
                newName: "Actif");

            migrationBuilder.RenameIndex(
                name: "IX_Users_Email",
                table: "utilisateur",
                newName: "IX_utilisateur_Email");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCreation",
                table: "utilisateur",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Nom",
                table: "utilisateur",
                type: "varchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "utilisateur",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_utilisateur",
                table: "utilisateur",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "categorie",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nom = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorie", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "exercice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nom = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Public = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InspireSec = table.Column<int>(type: "int", nullable: false),
                    ApneeSec = table.Column<int>(type: "int", nullable: false),
                    ExpireSec = table.Column<int>(type: "int", nullable: false),
                    Apnee2Sec = table.Column<int>(type: "int", nullable: false),
                    Cycles = table.Column<int>(type: "int", nullable: false),
                    DureeTotaleSec = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exercice", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "historique",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UtilisateurId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DureeSec = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_historique", x => x.Id);
                    table.ForeignKey(
                        name: "FK_historique_utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "utilisateur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "article",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Titre = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contenu = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DatePublication = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Public = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CategorieId = table.Column<int>(type: "int", nullable: false),
                    GereParUserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_article", x => x.Id);
                    table.ForeignKey(
                        name: "FK_article_categorie_CategorieId",
                        column: x => x.CategorieId,
                        principalTable: "categorie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_article_utilisateur_GereParUserId",
                        column: x => x.GereParUserId,
                        principalTable: "utilisateur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "maintient",
                columns: table => new
                {
                    UtilisateurId = table.Column<int>(type: "int", nullable: false),
                    CategorieId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_maintient", x => new { x.UtilisateurId, x.CategorieId });
                    table.ForeignKey(
                        name: "FK_maintient_categorie_CategorieId",
                        column: x => x.CategorieId,
                        principalTable: "categorie",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_maintient_utilisateur_UtilisateurId",
                        column: x => x.UtilisateurId,
                        principalTable: "utilisateur",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "enregistre",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    HistoriqueId = table.Column<int>(type: "int", nullable: false),
                    ExerciceId = table.Column<int>(type: "int", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DureeEffectiveSec = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_enregistre", x => x.Id);
                    table.ForeignKey(
                        name: "FK_enregistre_exercice_ExerciceId",
                        column: x => x.ExerciceId,
                        principalTable: "exercice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_enregistre_historique_HistoriqueId",
                        column: x => x.HistoriqueId,
                        principalTable: "historique",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_article_CategorieId",
                table: "article",
                column: "CategorieId");

            migrationBuilder.CreateIndex(
                name: "IX_article_GereParUserId",
                table: "article",
                column: "GereParUserId");

            migrationBuilder.CreateIndex(
                name: "IX_categorie_Nom",
                table: "categorie",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_enregistre_ExerciceId",
                table: "enregistre",
                column: "ExerciceId");

            migrationBuilder.CreateIndex(
                name: "IX_enregistre_HistoriqueId",
                table: "enregistre",
                column: "HistoriqueId");

            migrationBuilder.CreateIndex(
                name: "IX_historique_UtilisateurId",
                table: "historique",
                column: "UtilisateurId");

            migrationBuilder.CreateIndex(
                name: "IX_maintient_CategorieId",
                table: "maintient",
                column: "CategorieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "article");

            migrationBuilder.DropTable(
                name: "enregistre");

            migrationBuilder.DropTable(
                name: "maintient");

            migrationBuilder.DropTable(
                name: "exercice");

            migrationBuilder.DropTable(
                name: "historique");

            migrationBuilder.DropTable(
                name: "categorie");

            migrationBuilder.DropPrimaryKey(
                name: "PK_utilisateur",
                table: "utilisateur");

            migrationBuilder.DropColumn(
                name: "DateCreation",
                table: "utilisateur");

            migrationBuilder.DropColumn(
                name: "Nom",
                table: "utilisateur");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "utilisateur");

            migrationBuilder.RenameTable(
                name: "utilisateur",
                newName: "Users");

            migrationBuilder.RenameColumn(
                name: "MotDePasseHash",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "Actif",
                table: "Users",
                newName: "IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_utilisateur_Email",
                table: "Users",
                newName: "IX_Users_Email");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");
        }
    }
}
