FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

ENV ASPNETCORE_URLS=http://localhost:5000

COPY *.sln ./
COPY Magnise.Web/*.csproj Magnise.Web/
COPY Contracts/*.csproj Contracts/
COPY Contracts.Impl/*.csproj Contracts.Impl/
COPY DataAccess/*.csproj DataAccess/

RUN dotnet restore

COPY . .
WORKDIR /app/Magnise.Web
RUN dotnet publish -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /publish .
EXPOSE 80

ENTRYPOINT ["dotnet", "Magnise.Web.dll"]