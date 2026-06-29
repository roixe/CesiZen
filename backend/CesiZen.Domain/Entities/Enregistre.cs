using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CesiZen.Domain.Entities
{
    public class Enregistre
    {
        public int Id { get; set; }

        public int HistoriqueId { get; set; }
        public Historique Historique { get; set; } = null!;

        public int ExerciceId { get; set; }
        public Exercice Exercice { get; set; } = null!;

        public DateTime DateDebut { get; set; } = DateTime.UtcNow;
        public int DureeEffectiveSec { get; set; } = 0;
    }
}

