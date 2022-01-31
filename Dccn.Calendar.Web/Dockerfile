FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY . .
WORKDIR /src/Dccn.Calendar.Web
RUN dotnet publish Dccn.Calendar.Web.csproj -c Release -o /app

FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build /app .
EXPOSE 80
# See https://github.com/sherlock1982/ews-managed-api/issues/25
ENV DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER=0
ENTRYPOINT ["dotnet", "Dccn.Calendar.Web.dll"]