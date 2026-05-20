# Run Frontend (Angular)

Start the Angular dev server for this repository.

1. If I have not already given a port, ask me which port to use (default **4200**).
2. Run this in a **background** terminal from the workspace root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .vscode/scripts/run-frontend.ps1 -Port <port>
```

The script finds `angular.json` automatically and runs `ng serve` from that folder.

Do not modify application code; only start the dev server.
