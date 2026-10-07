# Plano de Execução — Aula09.Web (Blazor Components Avançados)

## Contexto

`Aula09.Web` é a próxima etapa após `Aula08.Web`: aprofundamento em **Blazor Components Avançados** com foco em `RenderFragments`, validação customizada em formulários, e dashboards interativos. A aula base é **Aula08.Web** (Blazor Server funcional com CRUD de Produtos).

**Objetivo da Aula**:
- Dominar `RenderFragments` (ChildContent) para criar componentes altamente reutilizáveis
- Implementar validação customizada com `EditContext` + `ValidationMessageStore`
- Construir um dashboard interativo que reflete mudanças em tempo real
- Aplicar padrões avançados de comunicação entre componentes

**Decisões confirmadas com usuário**:
- **Persistência**: reutilizar EF Core + PostgreSQL com schema compartilhado `hipermidia` (todas as aulas)
- **Base**: portar models `Produto`/`Categoria` de `Aula08.Web`
- **Autenticação**: não incluir (fora do escopo)
- **Componente Card**: usar `RenderFragment` para flexibilidade máxima

**Desafios Avaliativos** (4 itens):
1. Card com ícone dinâmico
2. Validação de preço mínimo por categoria
3. Atualizar dashboard em tempo real após cadastro
4. Usar `CascadingParameter` para tema (dark/light)

---

## Arquitetura da Solução

```
Aula09.Web/
├── Models/ (portado de Aula08)
│   ├── Produto.cs (+ campo Estoque)
│   └── Categoria.cs
│
├── Data/ (portado de Aula08)
│   ├── AppDbContext.cs (schema 'hipermidia')
│   └── DbInitializer.cs
│
├── Services/
│   └── ProdutoService.cs (assíncronos com factory)
│
├── Components/
│   ├── Card.razor (RenderFragment + ChildContent) — Desafio 1 (ícone dinâmico)
│   ├── ProdutoFormAvancado.razor (EditContext + validação customizada) — Desafio 2
│   └── DashboardCard.razor (métrica em card reutilizável)
│
├── Pages/
│   ├── Produtos.razor (CRUD com form avançado)
│   ├── Dashboard.razor (total + preço médio, atualização em tempo real) — Desafio 3
│   └── Configuracoes.razor (tema dark/light com CascadingParameter) — Desafio 4
│
├── Services/
│   └── TemaService.cs (Scoped, armazena preferência dark/light)
│
├── Program.cs (DbContextFactory + TemaService)
├── appsettings.json (connection string (schema hipermidia))
├── App.razor / Routes.razor / _Imports.razor (estrutura Blazor)
└── PLANO.md
```

---

## Passo a Passo de Execução

### 1. Scaffold e Estrutura Base
- Copiar arquivos de `Aula08.Web` → `Aula09.Web`:
  - `Program.cs`, `App.razor`, `Routes.razor`, `_Imports.razor`, `MainLayout.razor`
  - Models, Data, Services
  - `appsettings.json` (com schema `aula09`)
  - `.csproj` com mesmas dependências
- Registrar na solution: `dotnet sln add Aula09.Web/Aula09.Web.csproj`

### 2. Modelo Produto com Estoque
Atualizar `Models/Produto.cs`:
- Adicionar `public int Estoque { get; set; }` com validação `[Range(0, int.MaxValue)]`
- Manter Nome, Preco, Categoria, Ativo
- Migrations automáticas (`dotnet ef migrations add AddEstoque`)

### 3. Componente Card Reutilizável (Desafio 1)
Criar `Components/Card.razor`:
```razor
@* RenderFragment para máxima flexibilidade *@
<div class="card">
    <div class="card-header">
        @if (!string.IsNullOrEmpty(Icone))
        {
            <i class="@Icone"></i>
        }
        <h5>@Titulo</h5>
    </div>
    <div class="card-body">
        @ChildContent
    </div>
</div>

@code {
    [Parameter]
    public string Titulo { get; set; } = string.Empty;
    
    [Parameter]
    public string Icone { get; set; } = string.Empty; // Desafio 1: ícone dinâmico
    
    [Parameter]
    public RenderFragment ChildContent { get; set; }
}
```

### 4. ProdutoFormAvancado com Validação Customizada (Desafio 2)
Criar `Components/ProdutoFormAvancado.razor`:
- `EditContext` manual (não atributo `Model`)
- `ValidationMessageStore` para regras além de DataAnnotations
- Validação customizada: Estoque ≥ 0, preço mínimo por categoria (ex: Eletrônicos min R$ 100)
- `OnValidationRequested` event para validações customizadas
- Dispara `EventCallback<Produto> OnSalvar` ao sucesso

### 5. TemaService (CascadingParameter, Desafio 4)
Criar `Services/TemaService.cs`:
```csharp
public class TemaService
{
    private string _tema = "light"; // "light" | "dark"
    public string Tema => _tema;
    public Action OnTemaChanged { get; set; }
    
    public void AlterarTema(string tema)
    {
        _tema = tema;
        OnTemaChanged?.Invoke();
    }
}
```
Registrar no `Program.cs`: `builder.Services.AddScoped<TemaService>()`

### 6. Dashboard Interativo (Desafio 3)
Criar `Pages/Dashboard.razor`:
- Injeta `ProdutoService` + `TemaService`
- Exibe em cards (usando componente Card):
  - Total de Produtos
  - Preço Médio
  - Total em Estoque
  - Categoria com mais produtos
- Usa `OnInitializedAsync()` para carregar dados
- Implementa `IDisposable` e inscreve-se em `ProdutoService` para atualizações em tempo real (publish-subscribe pattern)

### 7. Página Configurações (Desafio 4)
Criar `Pages/Configuracoes.razor`:
- Botão "Dark Mode" / "Light Mode"
- Chama `TemaService.AlterarTema()`
- Usa `CascadingValue Value="TemaService.Tema"` para propagar tema

### 8. Página Produtos (Integração)
Atualizar `Pages/Produtos.razor`:
- Reutilizar `Card` component com diferentes títulos/ícones
- Substituir formulário anterior por `ProdutoFormAvancado`
- Após cadastro bem-sucedido, publicar evento para dashboard (Desafio 3)

### 9. MainLayout com Tema (Desafio 4)
Atualizar `Layouts/MainLayout.razor`:
- `@inject TemaService TemaService`
- `<CascadingValue Value="TemaService.Tema">` envolvendo `@Body`
- Aplicar classe CSS dinamicamente: `class="@(TemaService.Tema == "dark" ? "dark-theme" : "light-theme")"`

### 10. Estilos para Temas
Atualizar `wwwroot/app.css`:
- `.light-theme { background: white; color: black; }`
- `.dark-theme { background: #333; color: #fff; }`

### 11. Migrations e Seed
- Gerar: `dotnet ef migrations add InitialCreate --project Aula09.Web`
- Seed: 2 categorias, 4 produtos (com estoque variado)

### 12. PLANO.md
Documentar conforme padrão (Aula07/Aula08):
- Status, Fontes analisadas, Contexto/gaps, Passo a passo, Testes, Desafios

---

## Verificação

1. **Compilação**: `dotnet build`
2. **Migrations**: `dotnet ef database update --project Aula09.Web` (as migrations ficam em `Hipermidia.Data`; schema `hipermidia`)
3. **Execução**: `dotnet run --project Aula09.Web`, acessar `http://localhost:5238`
4. **Testes Manuais**:
   - Dashboard carrega com totais corretos
   - Card component reutiliza com ícones diferentes
   - ProdutoFormAvancado valida estoque (não negativo)
   - Após cadastro, dashboard atualiza em tempo real
   - Trocar tema (dark/light) afeta toda a aplicação
   - Formulário rejeita preço < mínimo por categoria

---

## 13. Conformidade com o PDF (Resultado Esperado e Atividade Avaliativa)

Revisão feita contra `Aula_09.pdf`. Resultado esperado: componentes reutilizáveis, validação customizada, formulários complexos, dashboard interativo e base sólida para Blazor WASM.

| # | Atividade avaliativa | Onde está | Situação |
|---|----------------------|-----------|----------|
| 1 | `Card` com ícone dinâmico | `Components/Card.razor` (parâmetro `Icone`, classes Bootstrap Icons; CSS `bootstrap-icons` carregado em `App.razor`) | Atendido |
| 2 | Validação de preço mínimo por categoria | `ProdutoFormAvancado.razor` → `ValidacaoCustomizada` (Eletrônicos R$ 100, demais R$ 10) com `ValidationMessageStore` | Atendido |
| 3 | Dashboard atualizado em tempo real após cadastro | `ProdutoEventos` (singleton) é notificado por `ProdutoService` em Adicionar/Atualizar/Remover; `DashboardResumo.razor` assina o evento e recarrega. Usado em `/dashboard` e no topo de `/produtos` | Atendido |
| 4 | `CascadingParameter` para o tema (dark/light) | `MainLayout.razor` publica `<CascadingValue Name="Tema">` e aplica `data-bs-theme`; `Configuracoes.razor` recebe com `[CascadingParameter(Name = "Tema")]`. O tema é salvo em `hipermidia."Configuracoes"` e sobrevive ao recarregar | Atendido |

Ajustes adicionais feitos nesta revisão:
- `@rendermode InteractiveServer` em `App.razor` (sem ele as páginas eram SSR estático e o POST do formulário falhava).
- Edição de produto (botão **Editar**, `ProdutoFormAvancado` em modo edição) e validação da categoria obrigatória (`[Range(1, int.MaxValue)]`).
- `/produtos` passou a usar `ProdutoFormAvancado` (com Estoque) em vez de `ProdutoForm`.
- Rodapé corrigido para "Aula 09".

## 14. Schema único `hipermidia`

Todas as aulas (04 a 10) usam o schema `hipermidia`, com tabelas compartilhadas:
`Categorias`, `Produtos` (Nome, Preco, Ativo, Estoque, Imagem, CategoriaId), `Pedidos` + `PedidoProduto` (Aula04), tabelas do Identity `AspNet*` (Aula07) e `Configuracoes` (chave/valor; guarda o tema).

- O modelo e as migrations ficam em `Hipermidia.Data` (`HipermidiaDbContext`, `HipermidiaMigrator.Aplicar`). Cada aula chama `HipermidiaMigrator.Aplicar(connectionString)` no startup e só mapeia as tabelas que usa; as pastas `Migrations` das aulas foram removidas.
- Colunas que uma aula não conhece têm default no banco (`Ativo = true`, `Estoque = 0`, `Imagem` nula), então todas inserem normalmente.
- O histórico de migrations fica em `hipermidia."__EFMigrationsHistory"`.
- Nova migration: `dotnet ef migrations add <Nome> --context HipermidiaDbContext --project Hipermidia.Data --startup-project Aula09.Web` (definir antes a variável `HIPERMIDIA_CONNECTION`).
- Os dados do antigo schema `aula08` (2 categorias, 8 produtos) foram copiados para `hipermidia` com os mesmos Ids.
