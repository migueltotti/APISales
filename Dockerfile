# Step 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY *.sln ./
COPY ["../Sales.API/Sales.API.csproj", "Sales.API/"]
COPY ["../Sales.CrossCutting/Sales.CrossCutting.csproj", "Sales.CrossCutting/"]
COPY ["../Sales.Application/Sales.Application.csproj", "Sales.Application/"]
COPY ["../Sales.Domain/Sales.Domain.csproj", "Sales.Domain/"]
COPY ["../Sales.Infrastructure/Sales.Infrastructure.csproj", "Sales.Infrastructure/"]
RUN dotnet restore "Sales.API/Sales.API.csproj"
COPY . .
WORKDIR "/app/Sales.API"
RUN dotnet build "Sales.API.csproj" -c Release -o /app/build

# Step 2: publish
FROM build AS publish
RUN dotnet publish --no-restore -c Release -o /app/publish 

# Step 3: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_HTTP_PORTS=80
EXPOSE 80
ENTRYPOINT ["dotnet", "Sales.API.dll"]