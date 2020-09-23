# escape=`
#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat
FROM mcr.microsoft.com/powershell:nanoserver-1903 AS downloadnodejs
RUN mkdir -p C:\nodejsfolder
WORKDIR C:\nodejsfolder
SHELL ["pwsh", "-Command", "$ErrorActionPreference = 'Stop';$ProgressPreference='silentlyContinue';"]
RUN Invoke-WebRequest -OutFile nodejs.zip -UseBasicParsing "https://nodejs.org/dist/v10.16.3/node-v10.16.3-win-x64.zip"; `
Expand-Archive nodejs.zip -DestinationPath C:\; `
Rename-Item "C:\node-v10.16.3-win-x64" c:\nodejs

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-nanoserver-1903 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=downloadnodejs C:\nodejs\ C:\Windows\system32\

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1903 AS build
COPY --from=downloadnodejs C:\nodejs\ C:\Windows\system32\
WORKDIR /src
COPY ["WebApiTest/WebApiTest.csproj", "WebApiTest/"]
RUN dotnet restore "WebApiTest/WebApiTest.csproj"
COPY . .
WORKDIR "/src/WebApiTest"
RUN dotnet build "WebApiTest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApiTest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApiTest.dll"]