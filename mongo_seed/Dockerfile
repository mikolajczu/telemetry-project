FROM mongo:latest

COPY users.json /users.json

CMD mongoimport --uri "mongodb://mongodb_aspnet/telemetryDb" --jsonArray --collection users --drop --file /users.json
