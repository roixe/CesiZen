using CesiZen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.Infrastructure.Data
{
    public class CesiZenDbContext : DbContext
    {
        public CesiZenDbContext(DbContextOptions<CesiZenDbContext> options) : base(options) { }

        public DbSet<User> Utilisateurs => Set<User>();
        public DbSet<Categorie> Categories => Set<Categorie>();
        public DbSet<Article> Articles => Set<Article>();
        public DbSet<Exercice> Exercices => Set<Exercice>();
        public DbSet<Historique> Historiques => Set<Historique>();
        public DbSet<Enregistre> Enregistrements => Set<Enregistre>();
        public DbSet<Maintient> Maintiens => Set<Maintient>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("utilisateur");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nom).HasMaxLength(150).IsRequired();
                e.Property(x => x.Email).HasMaxLength(255).IsRequired();
                e.HasIndex(x => x.Email).IsUnique();
                e.Property(x => x.MotDePasseHash).HasMaxLength(255).IsRequired();
                e.Property(x => x.Role).HasMaxLength(50).IsRequired();
                e.Property(x => x.Actif).IsRequired();
                e.Property(x => x.DateCreation).IsRequired();
            });

            // CATEGORIE
            modelBuilder.Entity<Categorie>(e =>
            {
                e.ToTable("categorie");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nom).HasMaxLength(150).IsRequired();
                e.HasIndex(x => x.Nom).IsUnique();
            });

            // ARTICLE
            modelBuilder.Entity<Article>(e =>
            {
                e.ToTable("article");
                e.HasKey(x => x.Id);
                e.Property(x => x.Titre).HasMaxLength(255).IsRequired();
                e.Property(x => x.Contenu).IsRequired();
                e.Property(x => x.Public).IsRequired();

                e.HasOne(x => x.Categorie)
                 .WithMany(c => c.Articles)
                 .HasForeignKey(x => x.CategorieId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(x => x.GereParUser)
                 .WithMany(u => u.ArticlesGeres)
                 .HasForeignKey(x => x.GereParUserId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // MAINTIENT (N-N)
            modelBuilder.Entity<Maintient>(e =>
            {
                e.ToTable("maintient");
                e.HasKey(x => new { x.UtilisateurId, x.CategorieId });

                e.HasOne(x => x.Utilisateur)
                 .WithMany(u => u.CategoriesMaintenues)
                 .HasForeignKey(x => x.UtilisateurId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Categorie)
                 .WithMany(c => c.Mainteneurs)
                 .HasForeignKey(x => x.CategorieId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // EXERCICE
            modelBuilder.Entity<Exercice>(e =>
            {
                e.ToTable("exercice");
                e.HasKey(x => x.Id);
                e.Property(x => x.Nom).HasMaxLength(150).IsRequired();
                e.Property(x => x.Type).HasMaxLength(50).IsRequired();
                e.Property(x => x.Public).IsRequired();

                e.Property(x => x.InspireSec).IsRequired();
                e.Property(x => x.ApneeSec).IsRequired();
                e.Property(x => x.ExpireSec).IsRequired();
                e.Property(x => x.Apnee2Sec).IsRequired();
                e.Property(x => x.Cycles).IsRequired();
                e.Property(x => x.DureeTotaleSec).IsRequired();
            });

            // HISTORIQUE
            modelBuilder.Entity<Historique>(e =>
            {
                e.ToTable("historique");
                e.HasKey(x => x.Id);

                e.Property(x => x.Date).IsRequired();
                e.Property(x => x.DureeSec).IsRequired();

                e.HasOne(x => x.Utilisateur)
                 .WithMany(u => u.Historiques)
                 .HasForeignKey(x => x.UtilisateurId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ENREGISTRE
            modelBuilder.Entity<Enregistre>(e =>
            {
                e.ToTable("enregistre");
                e.HasKey(x => x.Id);

                e.Property(x => x.DateDebut).IsRequired();
                e.Property(x => x.DureeEffectiveSec).IsRequired();

                e.HasOne(x => x.Historique)
                 .WithMany(h => h.ExercicesEnregistres)
                 .HasForeignKey(x => x.HistoriqueId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(x => x.Exercice)
                 .WithMany(ex => ex.Enregistrements)
                 .HasForeignKey(x => x.ExerciceId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Nom = "Admin",
                    Email = "admin@cesizen.local",
                    MotDePasseHash = "Admin", 
                    DateCreation = new DateTime(2025, 1, 1),
                    Role = "ADMIN",
                    Actif = true
                }
            );

            modelBuilder.Entity<Categorie>().HasData(
                new Categorie { Id = 1, Nom = "Respiration" },
                new Categorie { Id = 2, Nom = "Gestion du stress" }
            );

            modelBuilder.Entity<Exercice>().HasData(
                new Exercice
                {
                    Id = 1,
                    Nom = "Cohérence cardiaque 5-5",
                    Type = "RESPIRATION",
                    Description = "Inspire 5s, expire 5s. Environ 5 minutes.",
                    Public = true,
                    InspireSec = 5,
                    ApneeSec = 0,
                    ExpireSec = 5,
                    Apnee2Sec = 0,
                    Cycles = 30,
                    DureeTotaleSec = (5 + 5) * 30
                },
                new Exercice
                {
                    Id = 2,
                    Nom = "4-7-8",
                    Type = "RESPIRATION",
                    Description = "Inspire 4s, apnée 7s, expire 8s.",
                    Public = true,
                    InspireSec = 4,
                    ApneeSec = 7,
                    ExpireSec = 8,
                    Apnee2Sec = 0,
                    Cycles = 6,
                    DureeTotaleSec = (4 + 7 + 8) * 6
                },
                new Exercice
                {
                    Id = 3,
                    Nom = "Box breathing 4-4-4-4",
                    Type = "RESPIRATION",
                    Description = "Inspire 4s, apnée 4s, expire 4s, apnée 4s.",
                    Public = true,
                    InspireSec = 4,
                    ApneeSec = 4,
                    ExpireSec = 4,
                    Apnee2Sec = 4,
                    Cycles = 8,
                    DureeTotaleSec = (4 + 4 + 4 + 4) * 8
                }
            );

            modelBuilder.Entity<Article>().HasData(
                new Article
                {
                    Id = 1,
                    Titre = "Pourquoi la respiration aide à calmer le stress",
                    Contenu = "La respiration lente et régulière stimule le système parasympathique et favorise le retour au calme.",
                    DatePublication = new DateTime(2025, 1, 1),
                    Public = true,
                    CategorieId = 2,
                    GereParUserId = 1 
                }
            );
        }
    }
}
