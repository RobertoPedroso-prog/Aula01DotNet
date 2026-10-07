# Pendências

Atualizado em 2026-10-07.

## Banco de dados

- [ ] Apagar as 2 linhas antigas de `public."__EFMigrationsHistory"` (migrations do extinto schema `aula08`).
  O `psql` não existe no Windows e o Claude foi bloqueado, então rodar no prompt do Claude Code:
  ```
  ! docker exec supabase_db_redesaciar psql -U postgres -d postgres -c "DELETE FROM public.\"__EFMigrationsHistory\" WHERE \"MigrationId\" IN ('20260929012355_InitialCreate','20260929013925_AddEstoqueAndTemaService')"
  ```
  Saída esperada: `DELETE 2`. Não é urgente: as aulas usam `hipermidia."__EFMigrationsHistory"`.
  Obs.: o container `supabase_db_redesaciar` parece ser do projeto rede-saciar; o comando só toca nessa tabela.
- [ ] Limpar produtos de teste em `hipermidia."Produtos"`: "Teste Aula09" e os dois "Livro Dashboard" (criados pelo Claude). Conferir "Teclado Teste", "Mouse" e "Teclado".

## Código e git

- [ ] Nada foi commitado ainda (~78 arquivos alterados). Novos e não rastreados: `Hipermidia.Data/`, `Aula08.Web.Tests/`, `Aula09.Web/Components/DashboardResumo.razor`, `Aula09.Web/Services/ProdutoEventos.cs`, `Aula09.Web/Aula_09.pdf` e `.txt`, `PENDENCIAS.md`.
  As pastas `Migrations` das aulas 04 a 10 foram removidas (continuam no histórico do git).
- [ ] `Aula08.Web.Tests` ainda não está em `Aula01DotNet.sln`.
- [ ] Aviso MSB3277 nos testes: alinhar a versão do `Microsoft.EntityFrameworkCore.InMemory` com a do projeto web.
- [ ] Decidir se `Aula07.Web/run.log`, `Backups-Supabase/` e `GUIA-FORMATACAO.md` (já estavam não rastreados) entram no git ou no `.gitignore`.

## Testes que faltam

- [ ] Aula09: botão **Cancelar** da edição; preço mínimo de **Livros** (R$ 10); excluir produto que está em edição.
- [ ] Aulas 04 a 07 e 10: só foi feito teste de fumaça (a página inicial respondeu 200 com o schema `hipermidia`). Falta testar o CRUD, o login da Aula07 (Identity agora em `hipermidia`) e o Blazor WASM da Aula10.
- [ ] Aula08/Aula09: se o `Produtos` em `ProdutoDetalhe` deve mostrar o Estoque (hoje não mostra).

## Melhorias opcionais

- [ ] Mover os preços mínimos por categoria (Eletrônicos R$ 100, demais R$ 10), hoje fixos no `ProdutoFormAvancado`, para uma coluna em `Categorias`.
- [ ] Instalar `psql` (ou usar sempre `docker exec`) para facilitar manutenção do banco.
