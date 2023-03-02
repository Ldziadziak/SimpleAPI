# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
EXPOSE 80
EXPOSE 443

COPY ["SimpleAPI/SimpleAPI.csproj", "SimpleAPI/"]
COPY ["ChatGPTLibrary/ChatGPTLibrary.csproj", "ChatGPTLibrary/"]
RUN dotnet restore "SimpleAPI/SimpleAPI.csproj"
RUN dotnet restore "ChatGPTLibrary/ChatGPTLibrary.csproj"
COPY . .
WORKDIR "/src/SimpleAPI"
RUN dotnet build "SimpleAPI.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "SimpleAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false
WORKDIR /src/ChatGPTLibrary
RUN dotnet build "ChatGPTLibrary.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleAPI.dll"]
