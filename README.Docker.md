# Déploiement Docker — CESIZen

Déploiement conteneurisé de l'application (Activité 3). Trois services :
**cesizen-web** (Angular 21 + Nginx) · **cesizen-api** (ASP.NET Core 8) · **cesizen-db** (MySQL 8).

## Arborescence des fichiers ajoutés

```
CesiZen/
├── docker-compose.yml                ← orchestration (racine)
├── .env.example                      ← modèle de secrets (copier en .env)
├── .github/workflows/deploy.yml      ← déploiement continu (sur tag vX.Y.Z)
├── backend/
│   ├── Dockerfile                    ← image API (.NET, multi-stage)
│   └── .dockerignore
└── frontend/cesi-zen-ui/
    ├── Dockerfile                    ← image front (Angular + Nginx)
    ├── nginx.conf                    ← service statique + reverse-proxy /api
    └── .dockerignore
```

## Deux correctifs préalables (obligatoires)

1. **Program.cs** — appliquer les migrations au démarrage : voir `PATCH_Program.cs.txt`
   (sans cela, la base d'un conteneur neuf reste vide et l'API échoue).
2. **environment.prod.ts** — remplacer par la version corrigée fournie
   (`production: true` et `apiBaseUrl: '/api'`).

## Lancement (environnement de test/démo)

```bash
# 1. Configuration
cp .env.example .env
# éditer .env : définir des mots de passe forts et une clé JWT
#   ex. clé JWT :  openssl rand -base64 48

# 2. Construction et démarrage
docker compose up -d --build

# 3. Suivi
docker compose ps
docker compose logs -f cesizen-api
```

Application disponible sur **http://localhost:8080**
API (santé) : **http://localhost:8080/api/health**

## Arrêt / réinitialisation

```bash
docker compose down            # arrêt (conserve les données)
docker compose down -v         # arrêt + suppression du volume (remet la base à zéro)
```

## Notes

- Seul le port **8080** (web) est exposé ; l'API et la base ne sont accessibles
  que sur le réseau interne `cesizen-net` (réduction de la surface d'attaque).
- En **production**, exposer Nginx en **443** avec un certificat TLS valide
  (Let's Encrypt), placer les secrets dans un coffre-fort, et activer la
  migration via une étape dédiée du pipeline CD plutôt qu'au démarrage de l'API.
- Le fichier `.env` ne doit **jamais** être versionné (l'ajouter au `.gitignore`).
