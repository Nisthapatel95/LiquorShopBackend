FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["LiquorShop.API/LiquorShop.API.csproj", "LiquorShop.API/"]
RUN dotnet restore "LiquorShop.API/LiquorShop.API.csproj"

# Copy full source and publish
COPY . .
WORKDIR "/src/LiquorShop.API"
RUN dotnet publish "LiquorShop.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install Tesseract OCR native binaries for Linux container
RUN apt-get update && apt-get install -y tesseract-ocr tesseract-ocr-eng libtesseract-dev && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

# Set environment variable to listen on Render default port
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "LiquorShop.API.dll"]
