# frontend/cesi-zen-ui/Dockerfile — build Angular 21 puis service Nginx
# Contexte de build : ./frontend/cesi-zen-ui (défini dans docker-compose.yml)

# ---------- Étape 1 : build Angular ----------
FROM node:20 AS build
WORKDIR /app

# Installation déterministe des dépendances (utilise package-lock.json)
COPY package*.json ./
RUN npm ci

# Build de production (configuration par défaut = production dans angular.json)
COPY . .
RUN npx ng build --configuration production

# ---------- Étape 2 : service statique Nginx ----------
FROM nginx:1.27-alpine
# Sortie de build Angular 21 : dist/<projet>/browser
COPY --from=build /app/dist/cesi-zen-ui/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
