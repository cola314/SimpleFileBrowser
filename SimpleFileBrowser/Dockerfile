FROM mcr.microsoft.com/dotnet/sdk:3.1
EXPOSE 9402
VOLUME ["/usr/app/storage"]

WORKDIR /usr/app
COPY . .

RUN apt-get update -yq \
	&& apt-get install curl gnupg \
	&& curl -sL https://deb.nodesource.com/setup_14.x  | bash - \
	&& apt-get -y install nodejs

CMD ["dotnet", "run"]
