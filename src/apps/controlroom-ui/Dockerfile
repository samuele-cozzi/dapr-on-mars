# stage 1

FROM node:18-alpine3.15 AS control-ui-build
WORKDIR /app
COPY . .
#WORKDIR "/src/applications/controlroom-ui"
RUN npm ci --force
RUN npm run build

# stage 2

FROM nginx:alpine
COPY --from=control-ui-build /app/dist/controlroom-ui /usr/share/nginx/html
EXPOSE 80