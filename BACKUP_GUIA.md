# Guia de Backup e Restauração - Aula07.Web

## Pré-requisitos

Você precisa ter instalado as **PostgreSQL Client Tools**:

### Windows
1. Baixe o PostgreSQL em: https://www.postgresql.org/download/windows/
2. Durante a instalação, marque a opção "Command Line Tools"
3. Ou instale apenas as ferramentas: https://www.postgresql.org/download/windows/

### Verificar instalação
```powershell
pg_dump --version
pg_restore --version
psql --version
```

---

## 📦 FAZER BACKUP

### Passo 1: Certifique-se que o Docker está rodando
```bash
docker ps  # Deve mostrar o container PostgreSQL rodando
```

### Passo 2: Execute o script de backup
Abra o **PowerShell** como Administrador e execute:

```powershell
cd "C:\Users\DEV\RiderProjects\Aula01DotNet"
.\backup-banco.ps1
```

### Resultado
O script criará dois arquivos na pasta `Backups/`:
- `aula07_backup_YYYY-MM-DD_HH-MM-SS.dump` (comprimido, recomendado)
- `aula07_backup_YYYY-MM-DD_HH-MM-SS.sql` (texto)

**Tamanho esperado:** ~5-10 MB (dependendo dos dados)

---

## 🔄 RESTAURAR BACKUP

### Opção 1: Restaurar no mesmo servidor (Docker local)

```powershell
cd "C:\Users\DEV\RiderProjects\Aula01DotNet"
.\restore-banco.ps1 -BackupFile "Backups\aula07_backup_2025-01-15_10-30-45.dump"
```

### Opção 2: Restaurar em outro servidor PostgreSQL

Substitua os parâmetros:
```powershell
.\restore-banco.ps1 `
  -BackupFile "Backups\aula07_backup_2025-01-15_10-30-45.dump" `
  -Host "seu-outro-host.com" `
  -Port "5432" `
  -Database "nova_base" `
  -Username "outro_usuario" `
  -Password "outra_senha"
```

### Opção 3: Restaurar com psql (SQL)

```powershell
cd "Backups"
psql -h localhost -p 54322 -U postgres -d postgres -f aula07_backup_2025-01-15_10-30-45.sql
```

---

## 📋 ESTRUTURA DO BACKUP

Os scripts fazem backup completo incluindo:
- ✅ Esquema (schema `aula07`)
- ✅ Tabelas (Produtos, Categorias, AspNetUsers, etc)
- ✅ Dados (todos os registros)
- ✅ Índices
- ✅ Constraints
- ✅ Roles e permissões

---

## 🚨 TROUBLESHOOTING

### Erro: "pg_dump not found"
**Solução:** Instale PostgreSQL Client Tools ou adicione ao PATH:
```powershell
$env:PATH += ";C:\Program Files\PostgreSQL\15\bin"
```

### Erro: "Could not connect to database"
**Verifique:**
1. Docker está rodando: `docker ps`
2. Container PostgreSQL ativo: `docker logs <container_id>`
3. Conexão correta: `psql -h localhost -p 54322 -U postgres`

### Backup muito lento
**Aumentar timeout no script:**
```powershell
$env:PGCONNECT_TIMEOUT = 30
```

---

## 💾 MIGRAÇÃO PARA SUPABASE

Se quiser migrar para Supabase depois:

1. Faça o backup local (como acima)
2. Acesse Supabase Dashboard
3. Vá em "SQL Editor" → "Create" → "Import SQL dump"
4. Upload o arquivo `.dump` ou `.sql`

---

## 📅 AGENDAMENTO AUTOMÁTICO

Para fazer backups automáticos diariamente, crie uma tarefa no Windows Scheduler:

```powershell
$action = New-ScheduledTaskAction -Execute "powershell.exe" `
  -Argument "-File C:\Users\DEV\RiderProjects\Aula01DotNet\backup-banco.ps1"
$trigger = New-ScheduledTaskTrigger -Daily -At 2:00AM
Register-ScheduledTask -Action $action -Trigger $trigger -TaskName "Aula07BackupBanco" -Description "Backup diário do banco Aula07"
```

---

## ✅ CHECKLIST PÓS-BACKUP

- [ ] Arquivo criado em `Backups/`
- [ ] Tamanho do arquivo > 100 KB
- [ ] Timestamp no nome do arquivo
- [ ] Arquivo não corrompido: `pg_restore -l arquivo.dump`

---

## 📞 COMANDOS ÚTEIS

Listar bancos disponíveis:
```bash
psql -h localhost -p 54322 -U postgres -l
```

Conectar ao banco:
```bash
psql -h localhost -p 54322 -U postgres -d postgres
```

Executar query SQL:
```bash
psql -h localhost -p 54322 -U postgres -d postgres -c "SELECT COUNT(*) FROM aula07.\"Produtos\";"
```

Tamanho do backup:
```bash
dir "Backups\" | sort /R
```

---

**Data de criação:** 2025-01-15  
**Versão:** 1.0
