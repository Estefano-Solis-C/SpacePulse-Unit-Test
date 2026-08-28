# SpacePulse Automated Testing Suite

Suite de pruebas automatizadas unitarias, de integracion y contratos API (Jasmine, xUnit, Pytest, Postman).

## Ejecucion de Pruebas

### 1. Pruebas de Integracion (Pytest)
```bash
cd integration/pytest
pip install -r requirements.txt
pytest -v
```

### 2. Pruebas Unitarias Backend (.NET xUnit)
```bash
cd unit/backend
dotnet test
```

### 3. Orquestador Local
```powershell
cd devops
.\test-runner.ps1 -Target all
```
