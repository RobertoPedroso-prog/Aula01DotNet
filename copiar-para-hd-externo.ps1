# Script para copiar backups para HD externo

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "COPIAR BACKUPS PARA HD EXTERNO" -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Detectar HDs externos conectados
Write-Host "Verificando unidades disponiveis..." -ForegroundColor Green
Write-Host ""

$drives = Get-Volume | Where-Object { $_.DriveType -eq 'Removable' }

if ($drives.Count -eq 0) {
    Write-Host "Nenhum HD externo detectado!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Passos:" -ForegroundColor Yellow
    Write-Host "1. Conecte seu HD externo"
    Write-Host "2. Execute este script novamente"
    exit 1
}

Write-Host "Unidades removiveis encontradas:" -ForegroundColor Green
Write-Host ""

$drives | ForEach-Object {
    $driveletter = $_.DriveLetter
    $label = $_.FileSystemLabel
    $size = $_.Size / 1GB
    Write-Host "  [$driveletter] $label ($([Math]::Round($size, 2)) GB)" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "Digite a letra da unidade (ex: D, E, F):" -ForegroundColor Yellow
$driveLetter = Read-Host

# Validar unidade
if ($driveLetter.Length -ne 1) {
    Write-Host "Letra invalida!" -ForegroundColor Red
    exit 1
}

$destinationPath = "$($driveLetter):\Backups-Supabase-Copia-$((Get-Date).ToString('yyyy-MM-dd_HH-mm-ss'))"
$sourcePath = "C:\Users\DEV\RiderProjects\Aula01DotNet\Backups-Supabase"

Write-Host ""
Write-Host "Informacoes da copia:" -ForegroundColor Green
Write-Host "  De:   $sourcePath" -ForegroundColor White
Write-Host "  Para: $destinationPath" -ForegroundColor White
Write-Host ""

$confirm = Read-Host "Deseja prosseguir? (S/N)"
if ($confirm -ne "S" -and $confirm -ne "s") {
    Write-Host "Operacao cancelada." -ForegroundColor Yellow
    exit 0
}

Write-Host ""
Write-Host "Copiando arquivos..." -ForegroundColor Green
Write-Host ""

try {
    Copy-Item -Path $sourcePath -Destination $destinationPath -Recurse -Force -Verbose
    Write-Host ""
    Write-Host "Copia concluida com sucesso!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Informacoes da copia:" -ForegroundColor Green
    Write-Host "  Destino: $destinationPath" -ForegroundColor Cyan

    $items = Get-ChildItem -Path $destinationPath -Recurse -File
    $totalSize = ($items | Measure-Object -Property Length -Sum).Sum / 1MB

    Write-Host "  Arquivos: $($items.Count)" -ForegroundColor Cyan
    Write-Host "  Tamanho: $([Math]::Round($totalSize, 2)) MB" -ForegroundColor Cyan

    Write-Host ""
    Write-Host "Voce pode desconectar o HD externo com seguranca!" -ForegroundColor Green
}
catch {
    Write-Host "ERRO ao copiar: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "Dicas:" -ForegroundColor Yellow
    Write-Host "- Verifique se o HD externo tem espaco suficiente"
    Write-Host "- Verifique permissoes de acesso ao HD"
    Write-Host "- Tente novamente"
    exit 1
}
