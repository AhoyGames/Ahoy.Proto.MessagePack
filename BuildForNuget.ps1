dotnet test
if ($lastexitcode -ne 0) {
    Write-Host -NoNewLine 'Failed. Press any key to close...';
    [System.Console]::ReadKey()
    throw ("Failed")
}

function PackProj($proj) {
    dotnet pack -c Release --include-source --include-symbols -o nugets $proj
}

Remove-Item -path nugets -recurse
New-Item -path nugets -itemtype directory
PackProj("Ahoy.MessagePack.NewtonsoftJson\Ahoy.MessagePack.NewtonsoftJson.csproj")
PackProj("Ahoy.MessagePack.Protobuf\Ahoy.MessagePack.Protobuf.csproj")
PackProj("Ahoy.MessagePack.SystemJson\Ahoy.MessagePack.SystemJson.csproj")
PackProj("Ahoy.MessagePack.Thrift\Ahoy.MessagePack.Thrift.csproj")
PackProj("Ahoy.Proto.MessagePack\Ahoy.Proto.MessagePack.csproj")
