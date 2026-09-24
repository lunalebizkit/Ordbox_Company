# Stage 1: Build Angular
FROM node:24-alpine as build-step
WORKDIR /app

COPY package.json /app
RUN npm install --force

COPY . /app
RUN npm run build

# Stage 2: Servir con Nginx
FROM nginx:alpine

COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=build-step /app/dist/Ordbox/browser /usr/share/nginx/html

# Crear y dar permisos a las carpetas necesarias
RUN mkdir -p /var/cache/nginx /run/nginx \
    && chown -R 1000:1000 /usr/share/nginx/html \
    && chown -R 1000:1000 /var/cache/nginx \
    && chown -R 1000:1000 /run/nginx  \
    && chmod -R 777 /usr/share/nginx/html \
    && chmod -R 777 /var/cache/nginx \
    && chmod -R 777 /run/nginx 
USER 1000

EXPOSE 80

# Generar env.js al iniciar y correr Nginx
CMD ["/bin/sh", "-c", "envsubst < /usr/share/nginx/html/assets/env/env.template.js > /usr/share/nginx/html/assets/env/env.js && exec nginx -g 'daemon off;' && chmod 777 /run/nginx/nginx.pid"]
