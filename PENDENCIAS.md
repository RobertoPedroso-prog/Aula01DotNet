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

- [x] Aula10 commitada e enviada em 2026-10-07 (commit a5fba74).
- [x] Correção do Weather commitada em 2026-10-08: `Aula10.BlazorWasm/Pages/Weather.razor` passou a buscar `sample-data/weather.json` pelo endereço do próprio site; antes dava 404/"Failed to fetch" porque o `HttpClient` aponta para a API.

## Aula10

- [ ] JWT (o PDF cita como próxima base): a API ainda não tem autenticação.
- [ ] `dotnet-serve` foi instalado só numa pasta temporária; para o deploy local do PLANO rodar `dotnet tool install --global dotnet-serve`.
- [ ] Produtos de teste criados pelo Claude via WASM: "Cabo USB" (R$ 25) e "Retest Aula10" (R$ 33) em `hipermidia."Produtos"`.
- [ ] Aviso NU1903 na API: `Microsoft.OpenApi 2.0.0` (vem com `Microsoft.AspNetCore.OpenApi`) tem vulnerabilidade conhecida de severidade alta; atualizar o pacote.
- [ ] PWA: depois de publicar de novo, a versão antiga continua ativa até fechar todas as abas do site (atualização controlada do template). Para testar logo, desregistrar o service worker e limpar os caches no DevTools (Application).

## Aula11 (criada em 2026-10-08)

- [x] Aula11 commitada e enviada em 2026-10-08 (API, front com o novo layout, PLANO.md e solução).
- [ ] Produtos de teste criados pelo Claude em `hipermidia."Produtos"`: "Cabo HDMI Aula11", "Mouse Gamer Aula11", "Fonte 500W Aula11" (inativo), "Monitor Aula11 UI" e, das rodadas da Aula10, "Teste 3 Aula10".
- [ ] Sem testes automatizados de API (a verificação foi por HTTP e navegador); sem JWT.
- [ ] A automação do navegador deixa a aba em segundo plano e perde digitação/cliques reais; para testar na mão, usar a aba normalmente.

## Testes que faltam

- [ ] Aula09: botão **Cancelar** da edição; preço mínimo de **Livros** (R$ 10); excluir produto que está em edição.
- [ ] Aulas 04 a 07: só foi feito teste de fumaça (a página inicial respondeu 200 com o schema `hipermidia`). Falta testar o CRUD e o login da Aula07 (Identity agora em `hipermidia`).
- [x] Aula10: lista com erro e sem cache ("Tentar novamente") testada em 2026-10-08, junto com GET, POST, validação, offline e PWA publicado.
- [ ] Aula08/Aula09: se o `Produtos` em `ProdutoDetalhe` deve mostrar o Estoque (hoje não mostra).

## Melhorias opcionais

- [ ] Mover os preços mínimos por categoria (Eletrônicos R$ 100, demais R$ 10), hoje fixos no `ProdutoFormAvancado`, para uma coluna em `Categorias`.
- [ ] Instalar `psql` (ou usar sempre `docker exec`) para facilitar manutenção do banco.
