FROM mcr.microsoft.com/dotnet/sdk:3.1
EXPOSE 9402
VOLUME ["/usr/app/storage"]

WORKDIR /usr/app
COPY . .
CMD ["dotnet", "run"]
