# Build

Use .NET SDK 10 and a local SPT 4.1.6 installation with WeekendDrops 1.0.7. Set `SPT_ROOT` to its `SPT_Runtime` directory; game references and the item mods are not distributed here. A gitignored `Directory.Build.local.props` can set `SptRoot` instead.

```sh
dotnet build src/WeekendCollections/WeekendCollections.csproj -c Release
dotnet build client/Client.csproj -c Release
dotnet run --project tests/Tests.csproj -c Release
python tools/validate.py
python tools/package.py
```

Tests use temporary collection data and the installed Weekend assembly. They never use real player profiles. Backend reward arithmetic and merge/migration are covered; raid rendering and remote Fika UI require in-game validation.
