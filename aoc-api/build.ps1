dotnet publish .\AocApi.csproj -c Release -o bin
if (test-path .\bin\Debug)
{
    remove-item .\bin\Debug -recurse
}
if (test-path .\bin\Release) {
    remove-item .\bin\Release -recurse
}