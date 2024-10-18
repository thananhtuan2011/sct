FROM node:16.13.1-slim AS build
ARG NODE_ENV
WORKDIR /app
COPY ./package.json ./
COPY ./package-lock.json ./
COPY ./yarn.lock ./
RUN yarn install --production=false
COPY . .
RUN yarn build --configuration=${NODE_ENV}

FROM nginx:1.19.8-alpine AS final
COPY ./backend.nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/ /usr/share/nginx/html
EXPOSE 80
