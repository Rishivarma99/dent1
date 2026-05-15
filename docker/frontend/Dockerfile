# syntax=docker/dockerfile:1.7

# ---------- build ----------
FROM node:20-alpine AS build
WORKDIR /src

COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci

COPY frontend/ ./
RUN npm run build -- --configuration=production

# ---------- development ----------
FROM node:20-alpine AS development
WORKDIR /src
COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci
COPY frontend/ ./
EXPOSE 4200
ENTRYPOINT ["npx", "ng", "serve", "--host", "0.0.0.0", "--port", "4200", "--poll", "2000"]

# ---------- production ----------
FROM nginx:1.27-alpine AS production
COPY docker/frontend/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /src/dist/frontend/browser /usr/share/nginx/html
EXPOSE 80
