# Backup completo dos dois bancos Supabase

$backupDir = "C:\Users\DEV\RiderProjects\Aula01DotNet\Backups-Supabase"
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"

# Criar diretorio de backup
if (-not (Test-Path $backupDir)) {
    New-Item -ItemType Directory -Path $backupDir | Out-Null
}

Write-Host "========================================"
Write-Host "BACKUP DOS BANCOS SUPABASE"
Write-Host "========================================"
Write-Host ""
Write-Host "Diretorio: $backupDir"
Write-Host ""

# Lista de containers
$containers = @(
    @{ Name = "redesaciar"; Container = "supabase_db_redesaciar" },
    @{ Name = "CIFRATER-AVA"; Container = "supabase_db_CIFRATER-AVA" }
)

foreach ($item in $containers) {
    $dbName = $item.Name
    $containerName = $item.Container

    Write-Host "========================================"
    Write-Host "Fazendo backup: $dbName"
    Write-Host "Container: $containerName"
    Write-Host "========================================"
    Write-Host ""

    # Diretorio especifico para este banco
    $dbBackupDir = "$backupDir\$dbName"
    if (-not (Test-Path $dbBackupDir)) {
        New-Item -ItemType Directory -Path $dbBackupDir | Out-Null
    }

    $backupFileSQL = "$dbBackupDir\backup_$timestamp.sql"
    $backupFileDump = "$dbBackupDir\backup_$timestamp.dump"

    # Backup SQL (texto)
    Write-Host "[1/2] Criando backup SQL..."
    try {
        docker exec $containerName pg_dump -U postgres -d postgres > $backupFileSQL 2>$null

        if ((Test-Path $backupFileSQL) -and ((Get-Item $backupFileSQL).Length -gt 0)) {
            $size = (Get-Item $backupFileSQL).Length / 1MB
            Write-Host "      OK - $([Math]::Round($size, 2)) MB"
            Write-Host "      Arquivo: $backupFileSQL"
        } else {
            Write-Host "      ERRO - Arquivo vazio ou nao criado"
        }
    }
    catch {
        Write-Host "      ERRO: $_"
    }

    # Backup Dump (comprimido)
    Write-Host "[2/2] Criando backup DUMP..."
    try {
        docker exec $containerName pg_dump -U postgres -d postgres -F c -b > $backupFileDump 2>$null

        if ((Test-Path $backupFileDump) -and ((Get-Item $backupFileDump).Length -gt 0)) {
            $size = (Get-Item $backupFileDump).Length / 1MB
            Write-Host "      OK - $([Math]::Round($size, 2)) MB"
            Write-Host "      Arquivo: $backupFileDump"
        } else {
            Write-Host "      ERRO - Arquivo vazio ou nao criado"
        }
    }
    catch {
        Write-Host "      ERRO: $_"
    }

    Write-Host ""
}

Write-Host "========================================"
Write-Host "BACKUP CONCLUIDO!"
Write-Host "========================================"
Write-Host ""
Write-Host "Caminho dos backups:"
Write-Host "  $backupDir"
Write-Host ""
Write-Host "Estrutura de arquivos:"
Get-ChildItem -Path $backupDir -Recurse -File | ForEach-Object {
    $relativePath = $_.FullName.Replace($backupDir, "").TrimStart("\")
    $size = $_.Length / 1MB
    Write-Host "  $relativePath ($([Math]::Round($size, 2)) MB)"
}
Write-Host ""
Write-Host "Proximo passo: Copiar para HD externo em: $backupDir"
