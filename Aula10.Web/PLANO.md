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
│   ├── appsettings.json         (aula10 schema)
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
- Habilitar CORS para `https://localhost:5002` (porta WASM dev)
- Adicionar migrations para schema `aula10`

**Models (Produto, Categoria):**
- Portar de Aula09.Web com mesmos campos
- Adicionar data annotations para validação

**DbContext + DbInitializer:**
- Schema `aula10`
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
dotnet run  # HTTPS://localhost:5002
```

**Casos:**
1. ✅ Acessar https://localhost:5002 → carrega lista de produtos
2. ✅ Preencher formulário → POST bem-sucedido → lista atualiza
3. ✅ Preencher inválido → validação client-side rejeita
4. ✅ Parar API → WASM mostra erro (ou cache offline se implementado)
5. ✅ Instalar PWA (DevTools → Application → Install)
6. ✅ Modo offline (DevTools → Network → Offline) → lista ainda funciona

### 8. Deploy Local (Publish)

```bash
cd Aula10.BlazorWasm
dotnet publish -c Release

# Saída: bin/Release/net8.0/publish/wwwroot

# Servir localmente
dotnet tool install --global dotnet-serve
cd bin/Release/net8.0/publish/wwwroot
dotnet serve -d .
# Acessa em http://localhost:8080
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
- [ ] `dotnet ef database update --project Aula10.Api` cria schema `aula10`
- [ ] Seed data popula Produtos/Categorias
- [ ] PostgreSQL acessível em localhost:54322

### API (Aula10.Api)
- [ ] Roda em https://localhost:5001
- [ ] GET /api/produtos retorna JSON array com 2+ produtos
- [ ] POST /api/produtos cria novo e retorna 201 Created
- [ ] CORS permite origem https://localhost:5002
- [ ] Validação: rejeita preço <= 0 com BadRequest

### WASM (Aula10.BlazorWasm)
- [ ] Roda em https://localhost:5002
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

## Status de Implementação (2026-09-29)

### ✅ Implementado e Testado

**Aula10.Api (WebAPI)**
- Controllers: ProdutosController com GET e POST ✅
- Models: Produto e Categoria com validações ✅
- DbContext: AppDbContext configurado com PostgreSQL ✅
- Migrations: InitialCreate executada, banco criado ✅
- Seed Data: 3 produtos (Notebook, Mouse, Clean Code) ✅
- CORS: Habilitado para localhost ✅
- API rodando em http://localhost:5233/api/produtos ✅
- Teste: GET /api/produtos retorna [Notebook, Mouse, Clean Code] com HTTP 200 ✅

**Aula10.BlazorWasm (WASM SPA)**
- Scaffold: Criado com template `dotnet new blazorwasm --pwa` ✅
- Program.cs: HttpClient configurado para API ✅
- Pages/Produtos.razor: Página criada com GET e POST forms ✅
- ProdutoDto: Model para serialização ✅
- PWA: manifest.webmanifest + service-worker.js configurados ✅
- Compilação: Projeto compila sem erros ✅
- Servidor: Rodando em http://localhost:5014 ✅

### ⚠️ Em Progresso

**WASM Carregamento**
- Página carrega até "Loading" screen
- Possível issue: dotnet.js module loading (MIME type ou compilação)
- Solução: Pode exigir ajuste de launchSettings.json ou clean build

### 🔧 Próximos Passos (Se Continuar)

1. Investigar erro de module loading do dotnet.js
2. Testar com `dotnet build -c Release` 
3. Verificar MIME types do servidor
4. Validar CORS headers completos

---

## Status Final Esperado

```
✅ Aula10.Api
  - ProdutosController (GET /api/produtos, POST /api/produtos/{id})
  - DbContext + migrations (schema aula10)
  - Seed: 2 categorias, 2+ produtos
  - CORS habilitado para localhost:5002

✅ Aula10.BlazorWasm
  - SPA funcional em Blazor WASM
  - HttpClient injetado + consumo de API
  - Página Produtos com GET e POST
  - Estados: loading, erro, sucesso
  - PWA manifest + service worker

✅ PWA
  - Instalável em desktop/mobile
  - Cache offline funcional
  - Offline fallback para GET /api/produtos

✅ Validação Dupla
  - Client-side: data annotations
  - Server-side: ModelState + regras customizadas

✅ Documentação
  - PLANO.md atualizado com implementação
  - Git commit com testes e screenshots
```

---

## Próximos Passos (Futuro)

- [ ] Autenticação JWT
- [ ] Cache inteligente (Workbox)
- [ ] Paginação de produtos
- [ ] Componentes compartilhados (class library)
- [ ] E2E testing (Playwright)
