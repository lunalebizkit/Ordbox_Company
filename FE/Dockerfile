# Stage 1
FROM node:24-alpine as build-step
RUN mkdir -p /app
WORKDIR /app

COPY package.json /app

RUN npm install --force


#Variables de entorno 
COPY . /app
RUN npm run build

# Stage 2
FROM nginx:alpine
COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=build-step /app/dist/Ordbox/browser /usr/share/nginx/html

# Cuando el contenedor inicia, reemplaza el env.js con los valores de las variables de entorno
CMD ["/bin/sh",  "-c",  "envsubst < /usr/share/nginx/html/assets/env/env.template.js > /usr/share/nginx/html/assets/env/env.js && exec nginx -g 'daemon off;'"]

