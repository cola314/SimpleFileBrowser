FROM mcr.microsoft.com/dotnet/sdk:6.0
EXPOSE 9402
VOLUME ["/usr/app/storage"]

WORKDIR /usr/app
COPY . .

RUN apt-get update \
	&& apt-get install -y curl gnupg \
	&& curl -sL https://deb.nodesource.com/setup_14.x  | bash - \
	&& apt-get install -y nodejs

CMD ["dotnet", "run"]
