FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
COPY angular.json ./
COPY tsconfig*.json ./
RUN npm install
COPY src ./src
COPY public ./public
RUN npm run build -- --configuration docker

FROM nginx:alpine
COPY --from=build /app/dist/NfcTest/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]