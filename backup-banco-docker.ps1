# Script para fazer backup usando Docker
# Nao precisa instalar PostgreSQL Client Tools

$backupDir = "C:\Users\DEV\RiderProjects\Aula01DotNet\Backups"
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$backupFile = "$backupDir\aula07_backup_$timestamp.dump"
$backupSql = "$backupDir\aula07_backup_$timestamp.sql"

# Criar diretorio de backup
if (-not (Test-Path $backupDir)) {
    New-Item -ItemType Directory -Path $backupDir | Out-Null
    Write-Host "OK: Diretorio criado: $backupDir"
}

Write-Host "Iniciando backup via Docker..."
Write-Host ""

# Encontrar container PostgreSQL
Write-Host "Procurando container PostgreSQL..."
$container = docker ps --filter "ancestor=postgres" --format "{{.ID}}" | Select-Object -First 1

if (-not $container) {
    Write-Host "ERRO: Container PostgreSQL nao encontrado!"
    Write-Host "Certifique-se que o Docker esta rodando com PostgreSQL"
    exit 1
}

Write-Host "OK: Container encontrado: $container"
Write-Host ""

# Backup em formato dump
Write-Host "Criando backup dump (comprimido)..."
try {
    docker exec -e PGPASSWORD=postgres $container pg_dump -h localhost -U postgres -d postgres -F c -b > $backupFile 2>$null

    if ($LASTEXITCODE -eq 0 -and (Test-Path $backupFile)) {
        $size = (Get-Item $backupFile).Length / 1MB
        Write-Host "OK: Backup dump criado: $backupFile ($([Math]::Round($size, 2)) MB)"
    }
}
catch {
    Write-Host "ERRO ao criar backup dump: $_"
}

# Backup em formato SQL
Write-Host ""
Write-Host "Criando backup SQL (texto)..."
try {
    docker exec -e PGPASSWORD=postgres $container pg_dump -h localhost -U postgres -d postgres -b > $backupSql 2>$null

    if ($LASTEXITCODE -eq 0 -and (Test-Path $backupSql)) {
        $size = (Get-Item $backupSql).Length / 1MB
        Write-Host "OK: Backup SQL criado: $backupSql ($([Math]::Round($size, 2)) MB)"
    }
}
catch {
    Write-Host "ERRO ao criar backup SQL: $_"
}

Write-Host ""
Write-Host "Backup concluido!"
Write-Host ""
Write-Host "Arquivos criados em: $backupDir"
Write-Host ""
Write-Host "Para restaurar depois:"
Write-Host "  docker exec -i container_id psql -U postgres -f backup.sql"
