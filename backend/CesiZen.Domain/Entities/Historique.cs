using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CesiZen.Domain.Entities
{
    public class Historique
    {
        public int Id { get; set; }
        public int UtilisateurId { get; set; }
        public User Utilisateur { get; set; } = null!;

        public DateTime Date { get; set; } = DateTime.UtcNow;
        public int DureeSec { get; set; } = 0;

        public ICollection<Enregistre> ExercicesEnregistres { get; set; } = new List<Enregistre>();
    }
}

