# App_MicroService_Product

 dotnet ef migrations add "init" -p ./src/App_MicroService_Wallet.Infrastructure/App_MicroService_Wallet.Infrastructure.csproj -s ./src/App_MicroService_Wallet.WebApi/App_MicroService_Wallet.WebApi.csproj

dotnet ef database update -p ./src/App_MicroService_Wallet.Infrastructure/App_MicroService_Wallet.Infrastructure.csproj -s ./src/App_MicroService_Wallet.WebApi/App_MicroService_Wallet.WebApi.csproj --context ApplicationDbContext

Microservice Product : http://localhost:6100/swagger/index.html

docker run --rm -p 6100:6000 -p 6101:6001 -e ASPNETCORE_HTTP_PORT=https://+:6001 -e ASPNETCORE_URLS=http://+:6000 microservice-product


docker compuse build

docker compose up

Scaffold-DbContext "Name=ConnectionStrings:DefaultConnection" Microsoft.EntityFrameworkCore.SqlServer -OutputDir QueryPersistence -force

