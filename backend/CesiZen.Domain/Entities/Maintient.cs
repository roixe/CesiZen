using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CesiZen.Domain.Entities
{
    public class Maintient
    {
        public int UtilisateurId { get; set; }
        public User Utilisateur { get; set; } = null!;

        public int CategorieId { get; set; }
        public Categorie Categorie { get; set; } = null!;
    }
}