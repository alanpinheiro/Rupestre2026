## Estrutura de Solução (Projetos)
1. **Rupestre.Domain**: Entidades, Value Objects, Interfaces de Repositório.
2. **Rupestre.Application**: DTOs, Mappers, Services/Use Cases.
3. **Rupestre.Infrastructure**: Data Context, Repositórios, Migrations.
4. **Rupestre.Web**: Controllers, Views, ViewModels, Program.cs.

## Detalhes de Implementação (Padrões)

### Camada Application
- **Interfaces de Serviço:** Definir `IService` com os métodos de CRUD.
- **Serviços:** Implementar `Service` que contém a lógica de orquestração.
- **DTOs:** Criar `DTO` para transferir dados entre a Web e a Application.

### Camada Domain
- **Interfaces de Repositório:** Definir `IRepository` (abstração de dados).

### Camada Infrastructure
- **Implementação:** `Repository` (implementação real usando Entity Framework).
- **Injeção de Dependência:** Criar uma classe `DependencyInjection.cs` para registrar os serviços.


## Fluxo de Trabalho
1. Criar a estrutura de pastas e arquivos `.csproj`.
2. Configurar dependências entre projetos.
3. Implementar a camada de Domain primeiro.

## Padrões de Frontend

### Componentes de Interface
- **Listagens:** Todas as tabelas de dados do sistema DEVEM utilizar o plugin [DataTables.net](https://datatables.net/).
- **Configuração Padrão:**
  - Devem suportar ordenação (sorting) e busca rápida (search) por padrão.
  - A paginação deve ser definida para 10, 25 e 50 registros.
  - O idioma deve ser configurado para Português-Brasil.
  
### Integração
- As tabelas devem ser inicializadas via jQuery no carregamento da view.
- utilizar o processamento Server-side para garantir performance.