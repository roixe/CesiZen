using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CesiZen.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Nom { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string MotDePasseHash { get; set; } = null!;
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;
        public string Role { get; set; } = "USER";
        public bool Actif { get; set; } = true;

        // [SÉCU 4 / RGPD] Horodatage du consentement (null = pas de consentement enregistré)
        public DateTime? DateConsentement { get; set; }

        public ICollection<Article> ArticlesGeres { get; set; } = new List<Article>();
        public ICollection<Historique> Historiques { get; set; } = new List<Historique>();
        public ICollection<Maintient> CategoriesMaintenues { get; set; } = new List<Maintient>();

    }
}
