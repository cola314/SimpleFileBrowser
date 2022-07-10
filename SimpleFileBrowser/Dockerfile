#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 9420
VOLUME ["/data"]
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_lts.x | bash -
RUN apt-get install -y nodejs

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y libpng-dev libjpeg-dev curl libxi6 build-essential libgl1-mesa-glx
RUN curl -sL https://deb.nodesource.com/setup_14.x | bash -
RUN apt-get install -y nodejs
WORKDIR /src
COPY ["SimpleFileBrowser/SimpleFileBrowser.csproj", "SimpleFileBrowser/"]
RUN dotnet restore "SimpleFileBrowser/SimpleFileBrowser.csproj"
COPY . .
WORKDIR "/src/SimpleFileBrowser"
RUN dotnet build "SimpleFileBrowser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleFileBrowser.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SimpleFileBrowser.dll"]