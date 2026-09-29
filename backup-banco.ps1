# Script para fazer backup do banco PostgreSQL Aula07
# Conecta ao PostgreSQL local rodando em Docker

# Configurações de conexão
$host = "localhost"
$port = "54322"
$database = "postgres"
$username = "postgres"
$password = "postgres"

# Diretório de backup
$backupDir = "C:\Users\DEV\RiderProjects\Aula01DotNet\Backups"
$timestamp = Get-Date -Format "yyyy-MM-dd_HH-mm-ss"
$backupFile = "$backupDir\aula07_backup_$timestamp.dump"
$backupSql = "$backupDir\aula07_backup_$timestamp.sql"

# Criar diretório se não existir
if (-not (Test-Path $backupDir)) {
    New-Item -ItemType Directory -Path $backupDir | Out-Null
    Write-Host "✓ Diretório de backup criado: $backupDir"
}

Write-Host "🔄 Iniciando backup do banco de dados..."
Write-Host "Host: $host"
Write-Host "Port: $port"
Write-Host "Database: $database"
Write-Host ""

# Definir variável de ambiente PGPASSWORD para não pedir senha
$env:PGPASSWORD = $password

# Backup em formato dump (comprimido e restaurável)
Write-Host "📦 Criando backup em formato dump (comprimido)..."
try {
    & pg_dump -h $host -p $port -U $username -d $database -F c -b -v | Out-Null
    & pg_dump -h $host -p $port -U $username -d $database -F c -b -v -f $backupFile
    Write-Host "✅ Backup dump criado: $backupFile"
}
catch {
    Write-Host "❌ Erro ao criar backup dump: $_"
    Write-Host "   Certifique-se que pg_dump está instalado (PostgreSQL client tools)"
}

# Backup em formato SQL (texto, mais legível)
Write-Host ""
Write-Host "📄 Criando backup em formato SQL (texto)..."
try {
    & pg_dump -h $host -p $port -U $username -d $database -b -v -f $backupSql
    Write-Host "✅ Backup SQL criado: $backupSql"
}
catch {
    Write-Host "❌ Erro ao criar backup SQL: $_"
}

# Limpar variável de ambiente
Remove-Item Env:\PGPASSWORD -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "📊 Resumo do backup:"
if (Test-Path $backupFile) {
    $size = (Get-Item $backupFile).Length / 1MB
    Write-Host "   Dump: $(Split-Path $backupFile -Leaf) ($([Math]::Round($size, 2)) MB)"
}
if (Test-Path $backupSql) {
    $size = (Get-Item $backupSql).Length / 1MB
    Write-Host "   SQL:  $(Split-Path $backupSql -Leaf) ($([Math]::Round($size, 2)) MB)"
}

Write-Host ""
Write-Host "✨ Backup concluído com sucesso!"
Write-Host ""
Write-Host "Para restaurar o backup depois, use:"
Write-Host "  pg_restore -h localhost -U postgres -d postgres $backupFile"
Write-Host ""
Write-Host "Ou para SQL:"
Write-Host "  psql -h localhost -U postgres -d postgres -f $backupSql"
