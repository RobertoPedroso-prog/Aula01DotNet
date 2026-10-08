# Plano de Execução — Aula10.Web (Blazor WebAssembly + ASP.NET Core API)

## Contexto

`Aula10.Web` implementa uma **SPA (Single Page Application)** em Blazor WebAssembly que consome uma **API REST** em ASP.NET Core. Este é um salto arquitetural importante: passamos de **Blazor Server** (renderização no servidor) para **Blazor WASM** (execução no navegador).

**Objetivo da Aula**:
- Compreender diferenças entre client-side (WASM) vs server-side (Blazor Server)
- Implementar API RESTful com ASP.NET Core WebAPI
- Consumir API via HttpClient injetado
- Habilitar **PWA (Progressive Web App)** com cache offline
- Realizar deploy local e entender trade-offs de performance/segurança

**Base**: Estrutura `Produto`/`Categoria` de `Aula09.Web`, mas separada em dois projetos:
- `Aula10.Api` (WebAPI, backend)
- `Aula10.BlazorWasm` (SPA, frontend client-side)

**Desafios Avaliativos** (4 itens):
1. Implementar POST de Produto na API
2. Consumir POST no WASM com feedback (loading/erro)
3. Validação dupla (client + server)
4. Implementar cache offline simples com Service Worker

---

## Arquitetura da Solução

```
Aula10.Web/
│
├── Aula10.Api/                  (WebAPI ASP.NET Core)
│   ├── Models/
│   │   ├── Produto.cs
│   │   └── Categoria.cs
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── DbInitializer.cs
│   ├── Controllers/
│   │   └── ProdutosController.cs (GET /api/produtos, POST /api/produtos)
│   ├── Program.cs               (DbContext + CORS)
│   ├── appsettings.json         (schema hipermidia)
│   └── Aula10.Api.csproj
│
├── Aula10.BlazorWasm/           (Blazor WASM - SPA)
│   ├── Models/
│   │   └── ProdutoDto.cs
│   ├── Pages/
│   │   ├── Produtos.razor       (listagem + formulário)
│   │   └── Index.razor          (página inicial)
│   ├── Components/
│   │   └── ProdutoForm.razor    (formulário reusável)
│   ├── Shared/
│   │   └── MainLayout.razor
│   ├── wwwroot/
│   │   ├── manifest.json        (PWA metadata)
│   │   ├── service-worker.js    (cache offline)
│   │   └── app.css
│   ├── Program.cs               (HttpClient registration)
│   ├── App.razor
│   └── Aula10.BlazorWasm.csproj
│
└── PLANO.md (este arquivo)
```

---

## Passo a Passo de Execução

### 1. Criar Aula10.Api (WebAPI)

```bash
cd C:\Users\DEV\RiderProjects\Aula01DotNet\Aula10.Web
dotnet new webapi -n Aula10.Api
dotnet sln add Aula10.Api/Aula10.Api.csproj
```

### 2. Configurar Aula10.Api

**Program.cs:**
- Registrar DbContext com factory (pooled, como Aula09)
- Habilitar CORS para as origens `localhost` (WASM dev e publicado)
- Adicionar migrations para schema `hipermidia`

**Models (Produto, Categoria):**
- Portar de Aula09.Web com mesmos campos
- Adicionar data annotations para validação

**DbContext + DbInitializer:**
- Schema `hipermidia` (compartilhado, migrations em Hipermidia.Data)
- Seed: 2 categorias, 2+ produtos

**ProdutosController:**
- `[HttpGet]` → lista todos
- `[HttpPost]` → cria novo com validação
- `[HttpGet("{id}")]` → detalhes (opcional)

### 3. Criar Aula10.BlazorWasm (SPA)

```bash
cd C:\Users\DEV\RiderProjects\Aula01DotNet\Aula10.Web
dotnet new blazorwasm -n Aula10.BlazorWasm --pwa
dotnet sln add Aula10.BlazorWasm/Aula10.BlazorWasm.csproj
```

### 4. Configurar Aula10.BlazorWasm

**Program.cs:**
```csharp
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("https://localhost:5001/")
    });
```

**ProdutoDto (Models/ProdutoDto.cs):**
```csharp
public class ProdutoDto
{
    public int Id { get; set; }
    public string Nome { get; set; }
    public decimal Preco { get; set; }
}
```

**Pages/Produtos.razor:**
- Injeta HttpClient
- OnInitializedAsync: carrega lista com `Http.GetFromJsonAsync<List<ProdutoDto>>("/api/produtos")`
- Estados: loading, erro, sucesso
- Exibe lista em tabela
- Formulário para criar novo (POST)

**ProdutoForm.razor (componente):**
- EditContext com validação
- Campos: Nome (required), Preco (> 0), Categoria
- `OnValidSubmit` → `Http.PostAsJsonAsync("/api/produtos", dto)` com try/catch
- Feedback: loading spinner, mensagens de erro

**App.razor:**
- Link manifest: `<link rel="manifest" href="manifest.json" />`
- Meta tema: `<meta name="theme-color" content="#007bff" />`

### 5. Habilitar PWA

**wwwroot/manifest.json:**
```json
{
  "name": "Aula10 Blazor WASM",
  "short_name": "Aula10",
  "start_url": "/",
  "display": "standalone",
  "background_color": "#ffffff",
  "theme_color": "#007bff",
  "icons": []
}
```

**wwwroot/service-worker.js:**
- Event `install`: cachear `/api/produtos` na primeira visita
- Event `fetch`: retornar cache se offline, senão fazer requisição
- Fallback: mostrar mensagem "offline" se não houver cache

**App.razor:**
- Registrar service worker: `<script>navigator.serviceWorker.register('service-worker.js')</script>`

### 6. Validação Dupla

**API (ProdutosController):**
```csharp
[HttpPost]
public async Task<IActionResult> Post([FromBody] ProdutoDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    // Validações customizadas
    if (dto.Preco <= 0)
        return BadRequest("Preço deve ser maior que 0");
    
    // Persist...
    return CreatedAtAction(nameof(Get), new { id = produto.Id }, dto);
}
```

**WASM (ProdutoForm.razor):**
```razor
<DataAnnotationsValidator />
<ValidationSummary />
<!-- 
  + validação client-side via data annotations
  + tratamento de erro se POST falhar
-->
```

### 7. Testes Manuais

**Setup:**
```bash
# Terminal 1: API
cd Aula10.Api
dotnet run  # HTTPS://localhost:5001

# Terminal 2: WASM
cd Aula10.BlazorWasm
dotnet run --launch-profile http  # http://localhost:5014
```

**Casos:**
1. ✅ Acessar http://localhost:5014 → carrega lista de produtos
2. ✅ Preencher formulário → POST bem-sucedido → lista atualiza
3. ✅ Preencher inválido → validação client-side rejeita
4. ✅ Parar API → WASM mostra erro (ou cache offline se implementado)
5. ✅ Instalar PWA (DevTools → Application → Install)
6. ✅ Modo offline (DevTools → Network → Offline) → lista ainda funciona

### 8. Deploy Local (Publish)

```bash
cd Aula10.BlazorWasm
dotnet publish -c Release

# Saída: bin/Release/net10.0/publish/wwwroot

# Servir localmente (a API precisa estar rodando em http://localhost:5233)
dotnet tool install --global dotnet-serve
dotnet serve -d bin/Release/net10.0/publish/wwwroot -p 5050 --fallback-file index.html --gzip --brotli
# Acessa em http://localhost:5050
```

### 9. PLANO.md Final

Documentar:
- Status da implementação (✅ completo, parcial, bloqueado)
- Desafios concluídos (4/4)
- Testes realizados
- Resultado final (SPA funcional + PWA)
- Insights sobre WASM vs Server

---

## Verificação e Checklist

### Compilação e Build
- [ ] `dotnet build` sem erros
- [ ] Aula10.Api compila
- [ ] Aula10.BlazorWasm compila

### Database
- [ ] `dotnet ef database update --project Aula10.Api` cria schema `hipermidia`
- [ ] Seed data popula Produtos/Categorias
- [ ] PostgreSQL acessível em localhost:54322

### API (Aula10.Api)
- [ ] Roda em https://localhost:5001
- [ ] GET /api/produtos retorna JSON array com 2+ produtos
- [ ] POST /api/produtos cria novo e retorna 201 Created
- [ ] CORS permite origens localhost (5014 dev, 5050 publicado)
- [ ] Validação: rejeita preço <= 0 com BadRequest

### WASM (Aula10.BlazorWasm)
- [ ] Roda em http://localhost:5014
- [ ] Página Produtos carrega lista via HTTP
- [ ] Exibe loading enquanto busca
- [ ] Formulário valida (client-side) antes de enviar
- [ ] POST cria produto e lista atualiza

### PWA
- [ ] manifest.json acessível e válido
- [ ] Service Worker registra sem erros (DevTools Console)
- [ ] Browser oferece "Instalar" (PWA badge)
- [ ] Offline: GET /api/produtos retorna cache (se implementado)

### Testes Manuais (manual browser)
- [ ] Abrir DevTools → Application → Cache Storage
- [ ] Abrir DevTools → Network → simular Offline
- [ ] POST com dados válidos → sucesso
- [ ] POST com dados inválidos → erro mensagem clara

### Git
- [ ] Todos arquivos adicionados
- [ ] Commit com mensagem: "Implementa Aula10.Web: Blazor WASM + ASP.NET Core API"
- [ ] Attribution line adicionada

---

## Status de Implementação (2026-10-07)

Revisado contra `Aula_10.pdf` (Resultado Esperado e Atividade Avaliativa).

### Atividade Avaliativa

| # | Item | Onde está | Situação |
|---|------|-----------|----------|
| 1 | Criar POST de Produto na API | `ProdutosController.PostProduto` (DataAnnotations no DTO; 400 com mensagens; categoria opcional, padrão = primeira) | Atendido |
| 2 | Consumir POST no WASM | `Pages/Produtos.razor` → `Http.PostAsJsonAsync` | Atendido |
| 3 | Exibir loading e erro | Spinner ao carregar e ao salvar; erro da lista com **Tentar novamente**; erro do POST traduz o ProblemDetails da API | Atendido |
| 4 | Cache offline simples | Lista salva em `localStorage` a cada GET bem-sucedido; sem API, mostra a última lista com aviso e data. Além disso o service worker publicado guarda o app inteiro (PWA) | Atendido |

### Resultado Esperado

- SPA funcional em Blazor WASM consumindo a API real (lista e cadastro): testado no navegador.
- PWA: `manifest.webmanifest`, ícones e service worker; no build publicado o service worker fica ativo com cache `offline-cache-*` (84 itens) e o app abre com o servidor e a API desligados.
- Deploy local: `dotnet publish -c Release` + `dotnet serve` (porta 5050) funcionando.
- Validação dupla: DataAnnotations no DTO do WASM (cliente) e no DTO da API (servidor).

### Ajustes feitos nesta revisão

- **CORS**: a policy só liberava `https://localhost:5002`, mas o WASM roda em `http://localhost:5014` (dev) ou `:5050` (publicado); agora aceita qualquer origem `localhost`. Removido `UseHttpsRedirection` (a API local é HTTP e o redirect quebra o preflight).
- Link **Produtos** no menu do WASM (a página existia, mas não havia como chegar nela).
- Banco: a API usa o schema compartilhado `hipermidia` (migrations em `Hipermidia.Data`), não mais um schema `hipermidia`.
- O carregamento "parado em Loading" anotado em 2026-09-29 era só o primeiro download do runtime WASM (dezenas de arquivos); depois fica em cache e abre rápido.

### Como rodar

```bash
# Terminal 1: API (http://localhost:5233)
dotnet run --project Aula10.Api --launch-profile http

# Terminal 2: WASM em desenvolvimento (http://localhost:5014)
dotnet run --project Aula10.BlazorWasm --launch-profile http
```

No primeiro acesso o WASM leva alguns segundos para baixar o runtime.

### Observações

- O service worker de desenvolvimento (`service-worker.js`) não faz cache; o modo offline completo só existe no build publicado (`service-worker.published.js`).
- A API não tem autenticação; JWT fica como evolução (o PDF cita como próxima base).
