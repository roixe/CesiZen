# CesiZen
Projet Développement et Test 

Étapes d’installation :
1. Récupération du projet
Cloner le dépôt ou extraire l’archive :
git clone https://github.com/roixe/CesiZen.git
cd CESIZen
2. Configuration de la base de données
Créer la base de données MySQL :
CREATE DATABASE cesizen_dev;
Configurer la chaîne de connexion dans :
backend/CesiZen.Api/CesiZen.Api/appsettings.Development.json
3. Application des migrations Entity Framework
Se placer dans le projet backend :
cd backend/CesiZen.Api/CesiZen.Api
Restaurer les dépendances :
dotnet restore
Appliquer les migrations :
dotnet ef database update
4. Lancement du backend
Toujours dans le dossier backend :
dotnet run
L’API est alors accessible sur :
https://localhost:7203
5. Installation du frontend
Se placer dans le projet Angular :
cd ../../../frontend/cesi-zen-ui
Installer les dépendances :
npm ci
6. Lancement du frontend
Démarrer Angular avec le proxy API :
ng serve --proxy-config proxy.conf.json
7. Accès à l’application
Ouvrir le navigateur à l’adresse :
http://localhost:4200
Le proxy Angular redirige automatiquement les appels /api vers :
https://localhost:7203
