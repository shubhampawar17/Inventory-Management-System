FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY IMS.csproj ./
RUN dotnet restore IMS.csproj

COPY . ./
RUN dotnet publish IMS.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-10000} dotnet IMS.dll"]
