using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CesiZen.Domain.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public string Titre { get; set; } = null!;
        public string Contenu { get; set; } = null!;
        public DateTime? DatePublication { get; set; }
        public bool Public { get; set; } = true;

        public int CategorieId { get; set; }
        public Categorie Categorie { get; set; } = null!;

        public int GereParUserId { get; set; }
        public User GereParUser { get; set; } = null!;
    }
}
