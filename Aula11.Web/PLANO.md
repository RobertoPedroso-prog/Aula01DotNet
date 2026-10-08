# Plano de Execução — Aula11.Web (Comunicação com Web API)

> **Status (2026-10-08): implementado e testado no navegador.** `Aula11.Api` (API REST versionada por URL, com DTOs, paginação e filtros) e `Aula11.BlazorWasm` (front consumindo pela camada de serviço com `HttpClient` injetado). Os 5 itens da Atividade Avaliativa do PDF estão atendidos. Durante a implementação foram achados e corrigidos alguns problemas do código de exemplo do slide (seção "Contexto e gap").

## Fontes analisadas

- `Aula_11.pdf` (6 páginas: REST, DTOs, HttpClient, versionamento por URL, Resultado Esperado e Atividade Avaliativa)
- `Aula_11.txt` (código do slide: `Produto`, `ProdutoDto`, `PagedResult<T>`, `ProdutosController` v1 e a página Blazor com filtros e paginação)
- Aula10 (`Aula10.Web`): base do front Blazor WASM e do padrão API + schema compartilhado `hipermidia`

## Contexto e gap identificado

| Slide | O que foi feito aqui | Motivo |
|-------|----------------------|--------|
| Lista estática em memória no controller | EF Core no schema compartilhado `hipermidia` (`Hipermidia.Data`), via `ProdutoService` | Padrão do repositório desde a Aula 04; paginação e filtro acontecem no banco (`Skip/Take/Count`) |
| `dotnet new webapi` (template com OpenAPI) | `--use-controllers --no-openapi` | O pacote `Microsoft.OpenApi` do template tem aviso de vulnerabilidade (NU1903, já visto na Aula10) |
| Front "Aula10.BlazorWasm ou Aula08.BlazorServer" | `Aula11.BlazorWasm`, copiado da Aula10 (sem Counter/Weather) | O PDF usa `HttpClient`, o caso natural do WASM |
| `url += $"&nome={filtroNome}"` | `Uri.EscapeDataString(nome)` | Nome com espaço, `&` ou `#` quebrava a URL |
| `<select @bind="filtroAtivo">` com `bool?` e opção `""` | `string` no select, convertido para `bool?` | O binding de `""` para `bool?` falha |
| Botão Próxima sem limite | Desabilitado na última página (`TotalPages`); Anterior desabilitado na primeira | Evita pedir página inexistente |
| Filtro não volta à página 1 | Qualquer mudança de filtro, versão ou tamanho volta à página 1 | Senão a busca podia cair numa página vazia |
| `page`/`pageSize` sem validação | 400 `ValidationProblem` se `page < 1` ou `pageSize` fora de 1..100 | Boas práticas de status code do slide |
| `new HttpClient()` na página: proibido pelo slide | Páginas usam `ProdutosApiClient` (DI); o `HttpClient` é criado só no `Program.cs` com `ApiBaseUrl` de `wwwroot/appsettings.json` | "Regra de ouro" do slide |

Problemas encontrados durante os testes:
1. **`Location` do POST da v2 apontava para a v1.** As duas versões têm a ação `GetPorId` no controller `Produtos`, e `CreatedAtAction` resolvia a da v1. Corrigido com rotas nomeadas (`ProdutoV1PorId`, `ProdutoV2PorId`) e `CreatedAtRoute`.
2. **Enter no filtro de nome não aplicava o filtro.** `@bind` só atualiza ao sair do campo, e a busca rodava antes. Corrigido com `@bind:event="oninput"`.
3. **Campo chamado `page` colide com a diretiva `@page`** do Razor ("Página @page de ..."); o campo virou `pagina`.

## Arquitetura

```
Aula11.Web/
├── Aula11.Api/                          (ASP.NET Core, porta 5234)
│   ├── Controllers/
│   │   ├── ApiControllerBase.cs         (validação de paginação -> 400)
│   │   ├── V1/ProdutosController.cs     (api/v1/produtos: ProdutoDto)
│   │   └── V2/ProdutosController.cs     (api/v2/produtos: ProdutoV2Dto + Estoque no POST)
│   ├── DTOs/                            (ProdutoDto, ProdutoV2Dto, PagedResult<T>, CriarProdutoRequest[V2])
│   ├── Models/                          (Produto, Categoria: entidades internas, nunca expostas)
│   ├── Services/ProdutoService.cs       (listar com filtros/paginação, obter, criar)
│   ├── Data/                            (AppDbContext schema "hipermidia", DbInitializer)
│   └── Program.cs                       (HipermidiaMigrator, EF, CORS localhost)
└── Aula11.BlazorWasm/                   (Blazor WASM + PWA do template, porta 5015)
    ├── Services/ProdutosApiClient.cs    (único lugar que fala HTTP; devolve Resposta<T> com Erro legível)
    ├── Models/                          (PagedResult, ProdutoDto, CriarProdutoRequest)
    ├── Pages/Produtos.razor             (filtros, paginação, seletor de versão, POST)
    └── wwwroot/appsettings.json         (ApiBaseUrl)
```

## Endpoints

| Verbo | Rota | Resultado |
|-------|------|-----------|
| GET | `/api/v1/produtos?page=1&pageSize=2&ativo=true&nome=note` | 200 `PagedResult<ProdutoDto>` (Id, Nome, Preco); 400 se paginação inválida |
| GET | `/api/v1/produtos/{id}` | 200 ou 404 |
| POST | `/api/v1/produtos` | 201 + `Location`, ou 400 com mensagens (DataAnnotations e categoria inexistente) |
| GET / POST | `/api/v2/produtos...` | Igual à v1, mas o item traz `ativo`, `estoque` e `categoria`, e o POST aceita `estoque` e `ativo` |

`PagedResult<T>`: `page`, `pageSize`, `totalItems`, `totalPages`, `items`.

## Passo a passo

1. `dotnet new webapi -n Aula11.Api --use-controllers --no-openapi` dentro de `Aula11.Web`; remover `WeatherForecast`.
2. Referenciar `Hipermidia.Data` e ligar o EF ao schema `hipermidia` (sem migrations próprias: `HipermidiaMigrator.Aplicar` no startup).
3. Criar entidades, DTOs, `PagedResult<T>`, `ProdutoService`, controllers v1 e v2 (versionamento por URL, sem pacote extra).
4. POST com validação no backend (`[ApiController]` devolve 400 automático) e categoria padrão = primeira cadastrada.
5. Copiar `Aula10.BlazorWasm` para `Aula11.BlazorWasm`, renomear, remover Counter/Weather e criar `ProdutosApiClient`, modelos e a página `Produtos.razor`.
6. Adicionar os dois projetos à solução (`Aula01DotNet.sln`, pasta `Aula11.Web`).

## Atividade Avaliativa

| # | Item | Onde | Situação |
|---|------|------|----------|
| 1 | Criar `POST /api/v1/produtos` | `V1/ProdutosController.Post` | Atendido (201 + Location) |
| 2 | Validar DTO no backend | `CriarProdutoRequest` (DataAnnotations) + regra de categoria | Atendido (400 com mensagens em português) |
| 3 | Consumir POST no Blazor | `ProdutosApiClient.CriarAsync` + formulário em `Produtos.razor` | Atendido |
| 4 | Mensagens de erro/sucesso | Alertas de sucesso ("criado com sucesso (id N)") e de erro (validação da API, API fora do ar) | Atendido |
| 5 | Criar `api/v2/produtos` com campo extra | `V2/ProdutosController` (+ `ativo`, `estoque`, `categoria`); o front tem seletor de versão e mostra as colunas extras na v2 | Atendido |

## Testes realizados (2026-10-08)

**API (HTTP direto):**
- Paginação: `pageSize=2` → página 1 e 2 diferentes, `totalItems`/`totalPages` corretos.
- Filtros: `ativo=false`, `nome=note`, `nome=teclado&ativo=true` (combinados).
- `GET /produtos/999` → 404; `page=0` e `pageSize=500` → 400.
- POST v1: nome vazio, nome curto, preço 0, preço 100000, categoria 999, JSON inválido → todos 400; POST v2 com estoque -1 → 400.
- POST válido v1 e v2 → 201 e o `Location` abre o recurso na versão certa (200).

**Front (Blazor WASM no navegador):**
- Carrega "Página 1 de N" com 2 por página; Próxima/Anterior; Próxima desabilitado na última página.
- Filtro por nome + Enter (`tecl` → Teclado Teste e Teclado), combinado com status; mensagem quando não há resultado.
- Seletor v1/v2: colunas Ativo/Estoque/Categoria só na v2; tamanho de página 2/5/10.
- Criar: validação do cliente (mensagens em português), cadastro v2 com mensagem de sucesso e lista atualizada.
- Com a API desligada: erro ao criar e erro ao buscar com botão "Tentar novamente"; sem faixa de erro do Blazor.

Observação sobre os testes: a automação do navegador deixa a aba em segundo plano (`visibilityState: hidden`), então digitação e cliques reais foram perdidos em parte dos passos; esses passos foram feitos disparando os eventos do DOM (`input`, `change`, `keydown`), que são os que o Blazor escuta.

## Como rodar

```bash
# Terminal 1: API (http://localhost:5234)
dotnet run --project Aula11.Api --launch-profile http

# Terminal 2: front (http://localhost:5015)
dotnet run --project Aula11.BlazorWasm --no-launch-profile --urls http://localhost:5015
```

O primeiro acesso do WASM leva alguns segundos (download do runtime .NET).

## Resultado esperado (checklist do slide)

- API REST versionada (URL) ✅
- DTOs bem definidos (`ProdutoDto`, `ProdutoV2Dto`, `PagedResult<T>`, requests) ✅
- Paginação e filtros (nome, status) ✅
- Blazor consumindo a API corretamente (DI, URL escapada, erros tratados) ✅
- Base para JWT e segurança: ainda não há autenticação (próximo passo do curso)

## Refatoração de layout e CSS (2026-10-08)

O visual herdado do template (menu lateral roxo, estilos soltos) foi substituído:

- **Layout**: `MainLayout` com barra superior fixa (`NavMenu`), conteúdo em `container` e rodapé; menu recolhível no celular (botão ☰ com `aria-expanded` correto). O sidebar, os ícones em CSS e as páginas de exemplo saíram.
- **CSS**: `wwwroot/css/app.css` organizado em seções, com variáveis (`--app-primary`, `--app-radius`, `--app-shadow`, cores do cabeçalho). Cores de texto, fundo e borda usam as variáveis do Bootstrap 5.3, então **o tema claro/escuro acompanha o sistema** (script em `index.html` define `data-bs-theme`). Estilos de cartão, tabela, formulário, validação, paginação e erro do Blazor passaram a usar esses tokens.
- **Telas**: Início com cartões; Produtos dividido em três cartões (filtros, lista com paginação no rodapé, novo produto); página "não encontrada" em português. A lógica e os `id`s dos campos não mudaram.
- **PWA**: `manifest.webmanifest` e `index.html` em português, `theme-color` alinhado ao cabeçalho.
- **Verificado**: tema claro e escuro, página inicial, produtos (v1/v2, paginação) e celular (390 px: menu recolhido, sem rolagem horizontal).

## Pendências / observações

- Sem testes automatizados de API (a verificação foi por HTTP e navegador).
- Os produtos criados durante os testes ficaram em `hipermidia."Produtos"` (ver `PENDENCIAS.md`).
- Sem JWT: qualquer cliente local pode chamar o POST.
