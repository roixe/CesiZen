using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CesiZen.Domain.Entities
{
    public class Categorie
    {
        public int Id { get; set; }
        public string Nom { get; set; } = null!;

        public ICollection<Article> Articles { get; set; } = new List<Article>();
        public ICollection<Maintient> Mainteneurs { get; set; } = new List<Maintient>();
    }
}