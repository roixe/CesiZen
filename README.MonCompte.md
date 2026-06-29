# Page « Mon compte » — actions RGPD (UI)

Remplace la console JS par de vraies fonctionnalités utilisateur :
export de ses données (portabilité) et suppression de compte (effacement).

## Fichiers (à décompresser à la racine de CesiZen/, mêmes chemins)
- src/app/pages/account/account.ts      ← nouveau composant
- src/app/pages/account/account.html    ← nouveau template
- src/app/app.routes.ts                 ← route /account ajoutée (protégée authGuard)
- src/app/app.component.html            ← lien « Mon compte » ajouté au menu (si connecté)

## Rebuild
docker compose up -d --build

## Démonstration en soutenance
1. Connecte-toi → un lien « Mon compte » apparaît dans le menu.
2. Page Mon compte → bouton « Exporter mes données (JSON) » → un fichier
   mes-donnees-cesizen.json se télécharge (profil + historique).
3. Bouton « Supprimer mon compte » → confirmation en deux temps → le compte
   et ses données sont supprimés, déconnexion automatique.

Le token JWT est ajouté automatiquement par l'intercepteur existant : aucune
manipulation technique visible, c'est une vraie fonctionnalité produit.
