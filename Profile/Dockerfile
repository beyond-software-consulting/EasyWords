FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Profile/Profile.csproj Profile/
RUN dotnet restore Profile/Profile.csproj
COPY . .
WORKDIR /src/Profile
RUN dotnet build Profile.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Profile.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Profile.dll"]
