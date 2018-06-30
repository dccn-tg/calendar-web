FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
# See https://github.com/sherlock1982/ews-managed-api/issues/25
ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Dccn.Calendar.Web/Dccn.Calendar.Web.csproj Dccn.Calendar.Web/
COPY Dccn.Calendar/Dccn.Calendar.csproj Dccn.Calendar/
RUN dotnet restore Dccn.Calendar.Web/Dccn.Calendar.Web.csproj
COPY . .
WORKDIR /src/Dccn.Calendar.Web
RUN dotnet build Dccn.Calendar.Web.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Dccn.Calendar.Web.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Dccn.Calendar.Web.dll"]
