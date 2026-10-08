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

- [x] Commitado em 2026-10-07 (commits 0b2cd33 e e1f2ea4). As pastas `Migrations` das aulas 04 a 10 foram removidas (continuam no histórico do git).
- [ ] `Aula08.Web.Tests` ainda não está em `Aula01DotNet.sln` (já está no git).
- [ ] Aviso MSB3277 nos testes: alinhar a versão do `Microsoft.EntityFrameworkCore.InMemory` com a do projeto web.
- [ ] Decidir se `Aula07.Web/run.log`, `Backups-Supabase/` e `GUIA-FORMATACAO.md` (já estavam não rastreados) entram no git ou no `.gitignore`.

- [ ] **Aula10 não commitada**: alterações em `Aula10.Api` (CORS, controller, Program), `Aula10.BlazorWasm` (Produtos.razor, ProdutoDto, NavMenu) e `Aula10.Web/PLANO.md`.

## Aula10

- [ ] JWT (o PDF cita como próxima base): a API ainda não tem autenticação.
- [ ] `dotnet-serve` foi instalado só numa pasta temporária; para o deploy local do PLANO rodar `dotnet tool install --global dotnet-serve`.
- [ ] Produtos de teste criados pelo Claude via WASM: "Cabo USB" (R$ 25) em `hipermidia."Produtos"`.

## Testes que faltam

- [ ] Aula09: botão **Cancelar** da edição; preço mínimo de **Livros** (R$ 10); excluir produto que está em edição.
- [ ] Aulas 04 a 07: só foi feito teste de fumaça (a página inicial respondeu 200 com o schema `hipermidia`). Falta testar o CRUD e o login da Aula07 (Identity agora em `hipermidia`).
- [ ] Aula10: a lista com erro e sem cache (caminho "Tentar novamente" sem lista salva) não foi testada; o resto foi testado no navegador (GET, POST, validação, offline, PWA publicado).
- [ ] Aula08/Aula09: se o `Produtos` em `ProdutoDetalhe` deve mostrar o Estoque (hoje não mostra).

## Melhorias opcionais

- [ ] Mover os preços mínimos por categoria (Eletrônicos R$ 100, demais R$ 10), hoje fixos no `ProdutoFormAvancado`, para uma coluna em `Categorias`.
- [ ] Instalar `psql` (ou usar sempre `docker exec`) para facilitar manutenção do banco.
