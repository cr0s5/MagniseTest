# Running the Application

## Configure Connection String
Open the appsettings.json file in the root of Magnise.Web project
Update the "DbConnection" connection to point to your database

## Configure Admin credentials to connect to the API
Open the appsettings.json file in the root of Magnise.Web project
Update the "Admin" details to be able to connect to the "platform.fintacharts.com" APIs

## Run Migrations

## After starting the application
There is a hosted service that updates/adds market assets to the database from "platform.fintacharts.com"
The hosted service connects to the "platform.fintacharts.com" websocket and adds assets prices to the database for historical purposes

## User Authorization
Application APIs (except login) and Web Socket can be connected after authorization
Successful authorization is saving the token given provided from "platform.fintacharts.com" in the session data

## APIs
Authorize: POST https://localhost:5000/api/user/login with form data
GET all available assets: GET https://localhost:5000/api/assets
GET historical prices of specific asset: GET https://localhost:5000/api/assets/history-prices/{id} with additional filters form-to datetime in query data

## WebSocket
Start getting realtime assets prices: https://localhost:5000/ws/{id} where id is assetId
to start a websocket send the object '{ type: "start" }'
to end the websocket send the object '{ type: "end" }'
to subscribe another asset send the object '{ type: "subscribe", id: {{assetId}} }'

## Testing
Added additional html and js files to test authorization and connect via websocket : https://localhost:5000/static