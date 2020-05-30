FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.3-bionic

WORKDIR /app

COPY /release ./

ENTRYPOINT ["dotnet", "AccountMicroservice.dll"]