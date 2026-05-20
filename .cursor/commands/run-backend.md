# Run Backend (.NET API)

Start the .NET API for this repository.

Run this in a **background** terminal from the workspace root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .vscode/scripts/run-backend.ps1
```

The script finds the `api` folder and runs `dotnet run` against the `*.Api.csproj` project.

Do not modify application code; only start the API.
