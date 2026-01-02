#build 
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/Booking/Booking.API/Booking.API.csproj", "src/Booking/Booking.API/"]
RUN dotnet restore "src/Booking/Booking.API/Booking.API.csproj"

COPY . . 

RUN dotnet publish "src/Booking/Booking.API/Booking.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

#start
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Booking.API.dll"]
