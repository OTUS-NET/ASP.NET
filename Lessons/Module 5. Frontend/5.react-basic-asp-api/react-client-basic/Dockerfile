FROM node:22.12-alpine
WORKDIR /src
COPY . .
WORKDIR "/src/react-client-basic"
RUN npm install
RUN npm install -g dotenv-cli
RUN npm run build-docker
WORKDIR "/src/react-client-basic/ClientApp"
EXPOSE 5272
CMD ["npm", "run", "preview"]