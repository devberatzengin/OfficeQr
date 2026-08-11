FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy csproj ve restore dependencies
COPY ["OfficeQr/OfficeQr.csproj", "OfficeQr/"]
RUN dotnet restore "OfficeQr/OfficeQr.csproj"

# Copy tüm kod
COPY . .

# Build uygulamayı Release mode'de
RUN dotnet build "OfficeQr/OfficeQr.csproj" -c Release -o /app/build

# Publish
RUN dotnet publish "OfficeQr/OfficeQr.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

# Build stage'den publish edilmiş dosyaları kopyala
COPY --from=build /app/publish .

# Port expose et
EXPOSE 8080

# Environment variable
ENV ASPNETCORE_URLS=http://+:8080

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "OfficeQr.dll"]