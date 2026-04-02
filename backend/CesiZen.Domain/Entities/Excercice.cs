using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CesiZen.Domain.Entities
{
    public class Exercice
    {
        public int Id { get; set; }
        public string Nom { get; set; } = null!;
        public string Type { get; set; } = "RESPIRATION";
        public string? Description { get; set; }
        public bool Public { get; set; } = true;

        public int InspireSec { get; set; }
        public int ApneeSec { get; set; } = 0;
        public int ExpireSec { get; set; }
        public int Apnee2Sec { get; set; } = 0;
        public int Cycles { get; set; } = 5;

        public int DureeTotaleSec { get; set; }

        public ICollection<Enregistre> Enregistrements { get; set; } = new List<Enregistre>();
    }
}
