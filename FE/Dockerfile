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

RUN addgroup -S appgroup && adduser -S appuser -G appgroup

COPY nginx.conf /etc/nginx/nginx.conf
COPY --from=build-step /app/dist/Ordbox/browser /usr/share/nginx/html

RUN chown -R appuser:appgroup /usr/share/nginx/html /etc/nginx
USER appuser
EXPOSE 80

# Cuando el contenedor inicia, reemplaza el env.js con los valores de las variables de entorno
CMD ["/bin/sh",  "-c",  "envsubst < /usr/share/nginx/html/assets/env/env.template.js > /usr/share/nginx/html/assets/env/env.js && exec nginx -g 'daemon off;'"]

