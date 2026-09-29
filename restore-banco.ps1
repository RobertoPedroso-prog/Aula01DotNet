# Script para restaurar backup do banco PostgreSQL Aula07

param(
    [Parameter(Mandatory=$true)]
    [string]$BackupFile,

    [string]$Host = "localhost",
    [string]$Port = "54322",
    [string]$Database = "postgres",
    [string]$Username = "postgres",
    [string]$Password = "postgres"
)

Write-Host "🔄 Iniciando restauração do banco de dados..."
Write-Host "Arquivo: $BackupFile"
Write-Host "Host: $Host"
Write-Host "Port: $Port"
Write-Host "Database: $Database"
Write-Host ""

# Verificar se arquivo existe
if (-not (Test-Path $BackupFile)) {
    Write-Host "❌ Arquivo de backup não encontrado: $BackupFile"
    exit 1
}

# Definir variável de ambiente PGPASSWORD
$env:PGPASSWORD = $Password

# Detectar tipo de arquivo
if ($BackupFile -like "*.dump") {
    Write-Host "📦 Detectado arquivo em formato dump (comprimido)..."
    Write-Host ""
    Write-Host "Executando: pg_restore -h $Host -U $Username -d $Database -v $BackupFile"

    try {
        & pg_restore -h $Host -p $Port -U $Username -d $Database -v $BackupFile
        Write-Host ""
        Write-Host "✅ Restauração concluída com sucesso!"
    }
    catch {
        Write-Host "❌ Erro ao restaurar: $_"
        exit 1
    }
}
elseif ($BackupFile -like "*.sql") {
    Write-Host "📄 Detectado arquivo SQL (texto)..."
    Write-Host ""
    Write-Host "Executando: psql -h $Host -U $Username -d $Database -f $BackupFile"

    try {
        & psql -h $Host -p $Port -U $Username -d $Database -f $BackupFile
        Write-Host ""
        Write-Host "✅ Restauração concluída com sucesso!"
    }
    catch {
        Write-Host "❌ Erro ao restaurar: $_"
        exit 1
    }
}
else {
    Write-Host "❌ Tipo de arquivo não reconhecido. Use .dump ou .sql"
    exit 1
}

# Limpar variável de ambiente
Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "✨ Processo finalizado!"
