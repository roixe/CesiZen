# Renforcement sécurité — CESIZen (points 1, 2, 3, 4 + Dependabot)

Décompresse à la racine de `CesiZen/` : les fichiers remplacent les existants (même chemins).

## Ce que chaque point apporte

| # | Mesure | Fichiers concernés | Effet |
|---|--------|--------------------|-------|
| 1 | **Rate limiting login** | `Program.cs`, `AuthController.cs` | 5 tentatives/min/IP sur `/api/auth/login`, puis HTTP 429 → bloque la force brute (OWASP A07) |
| 2 | **HTTPS/TLS + HSTS** | `nginx.conf`, `docker-compose.yml`, certificat | Chiffre le trafic navigateur ↔ serveur → protège du MITM. HSTS force le HTTPS. |
| 3 | **Endpoints RGPD** | `UserController.cs` | `GET /api/user/me/export` (portabilité) et `DELETE /api/user/me` (effacement). Liste des users désormais réservée ADMIN. |
| 4 | **Consentement RGPD** | `RegisterRequestDto.cs`, `User.cs`, `auth.ts`, `register.ts`, `register.html` | Case obligatoire à l'inscription + horodatage `DateConsentement` en base. |
| + | **Dependabot** | `.github/dependabot.yml` | PR automatiques de mise à jour des dépendances vulnérables (veille). |

## Étapes d'installation

### 1) Migration de base de données (pour le point 4)
Le point 4 ajoute une colonne `DateConsentement`. Génère la migration depuis le dossier
`backend/CesiZen.Api` (où se trouve la solution) :

```powershell
# une seule fois si l'outil n'est pas installé :
dotnet tool install --global dotnet-ef

cd backend\CesiZen.Api
dotnet ef migrations add AddConsentement --project CesiZen.Infrastructure --startup-project CesiZen.Api
```

Pas besoin de l'appliquer à la main : au prochain `docker compose up`, le `Migrate()` du
`Program.cs` ajoutera la colonne automatiquement (la base existante est conservée).

### 2) Certificat TLS auto-signé (pour le point 2)
Sans OpenSSL sous Windows, on le génère via un conteneur, à la racine du projet :

```powershell
mkdir certs
docker run --rm -v ${PWD}/certs:/certs alpine/openssl req -x509 -nodes -days 365 `
  -newkey rsa:2048 -keyout /certs/cesizen.key -out /certs/cesizen.crt -subj "/CN=localhost"
```

Ajoute `certs/` au `.gitignore` (ne jamais versionner une clé privée) :

```powershell
Add-Content .gitignore "`ncerts/"
```

### 3) Reconstruire et lancer

```powershell
docker compose up -d --build
```

- HTTP  : http://localhost:8080
- HTTPS : https://localhost:8443  (le navigateur affiche un avertissement « certificat auto-signé » : c'est normal en démo, tu cliques sur « Continuer ». En production : certificat valide type Let's Encrypt.)

## Démonstrations possibles en soutenance

- **Rate limiting** : tente 6 connexions ratées d'affilée → la 6ᵉ renvoie « 429 Too Many Requests ».
- **HTTPS/HSTS** : ouvre https://localhost:8443, montre le cadenas et l'en-tête `Strict-Transport-Security` (onglet Réseau du navigateur).
- **RGPD export** : connecté, appelle `GET /api/user/me/export` → renvoie le profil + l'historique en JSON.
- **RGPD suppression** : `DELETE /api/user/me` → le compte et ses données sont effacés (204).
- **Consentement** : sur l'inscription, le bouton reste désactivé tant que la case n'est pas cochée.
- **Dependabot** : onglet **Security → Dependabot** du dépôt, montre les alertes / PR automatiques.

## Note honnêteté dossier ↔ code
Ces ajouts rendent le code cohérent avec le plan de sécurisation. Pense à mettre à jour le
dossier : le chiffrement en transit (HTTPS) et les droits RGPD (accès/portabilité/effacement,
consentement) sont désormais réellement implémentés. Le chiffrement AES « au repos » des champs
sensibles reste, lui, une perspective (à présenter comme telle).
