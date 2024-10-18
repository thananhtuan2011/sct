#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ./*.sln ./
COPY ["API_SoCongThuong/API_SoCongThuong.csproj", "./API_SoCongThuong/"]
COPY ["EF_Core/EF_Core.csproj", "./EF_Core/"]
RUN dotnet restore "./EF_Core/EF_Core.csproj"
RUN dotnet restore "./API_SoCongThuong/API_SoCongThuong.csproj"
COPY . .
WORKDIR "/src/EF_Core"
RUN dotnet build "EF_Core.csproj" -c Release -o /app/build

WORKDIR "/src/API_SoCongThuong"
RUN dotnet build "API_SoCongThuong.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API_SoCongThuong.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API_SoCongThuong.dll"]