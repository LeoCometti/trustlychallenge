#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
#EXPOSE 80
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TrustlyChallenge.csproj", "TrustlyChallenge/"] #ADDED #COPY ["TrustlyChallenge/TrustlyChallenge.csproj", "TrustlyChallenge/"]
RUN dotnet restore "TrustlyChallenge/TrustlyChallenge.csproj"
WORKDIR "/src/TrustlyChallenge" #ADDED #MOVED
COPY . .
#WORKDIR "/src/TrustlyChallenge"
RUN dotnet build "TrustlyChallenge.csproj" -c Release -o /app/build #RUN dotnet build "TrustlyChallenge.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TrustlyChallenge.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet TrustlyChallenge.dll #ENTRYPOINT ["dotnet", "TrustlyChallenge.dll"]

#cmd -> docker build -f Dockerfile -t "project:latest" ..