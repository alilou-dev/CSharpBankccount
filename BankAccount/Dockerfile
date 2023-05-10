FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /BankAccount

# Copy csproj
COPY ../ .
# Restore as distinct layers
RUN dotnet restore
# Copy everything else and build
COPY ./BankAccount/ ./BankAccount/
RUN dotnet build -c Release -o /BankAccount/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /BankAccount
COPY --from=build-env ./BankAccount/out .
ENTRYPOINT ["dotnet", "BankAccount.dll"]