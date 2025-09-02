# LibraryManagement.Api - Backend

## Visão Geral

Esta é a API REST do Sistema de Gerenciamento de Biblioteca, desenvolvida em .NET 8.0 com ASP.NET Core Web API. A API fornece endpoints para gerenciar gêneros, autores e livros, seguindo os princípios REST e as melhores práticas de desenvolvimento.

## Arquitetura

A aplicação segue uma arquitetura em camadas bem definida:

### Camadas da Aplicação

1. **Controllers**: Responsáveis por receber as requisições HTTP e retornar as respostas
2. **Services**: Contêm a lógica de negócio da aplicação
3. **Data**: Camada de acesso a dados com Entity Framework Core
4. **Models**: Entidades do domínio
5. **DTOs**: Objetos de transferência de dados

### Estrutura de Pastas

```
LibraryManagement.Api/
├── Controllers/
│   ├── AuthorsController.cs
│   ├── BooksController.cs
│   └── GenresController.cs
├── Services/
│   ├── IAuthorService.cs
│   ├── AuthorService.cs
│   ├── IBookService.cs
│   ├── BookService.cs
│   ├── IGenreService.cs
│   └── GenreService.cs
├── Models/
│   ├── Author.cs
│   ├── Book.cs
│   └── Genre.cs
├── DTOs/
│   ├── AuthorDto.cs
│   ├── BookDto.cs
│   ├── GenreDto.cs
│   └── ApiResponse.cs
├── Data/
│   └── LibraryContext.cs
├── Tests/
│   └── Services/
│       └── GenreServiceTests.cs
├── Program.cs
└── appsettings.json
```

## Tecnologias e Pacotes

### Principais Dependências

- **Microsoft.AspNetCore.App** - Framework web ASP.NET Core
- **Microsoft.EntityFrameworkCore** - ORM para acesso a dados
- **Npgsql.EntityFrameworkCore.PostgreSQL** - Provider PostgreSQL
- **Swashbuckle.AspNetCore** - Geração de documentação Swagger
- **Microsoft.AspNetCore.Cors** - Suporte a CORS

### Dependências de Teste

- **Microsoft.AspNetCore.Mvc.Testing** - Testes de integração
- **Microsoft.EntityFrameworkCore.InMemory** - Banco em memória para testes
- **xunit** - Framework de testes
- **xunit.runner.visualstudio** - Runner de testes

## Configuração

### Pré-requisitos

- .NET 8.0 SDK
- PostgreSQL 12+

### Configuração do Banco de Dados

1. **Instalar PostgreSQL**
   ```bash
   # Ubuntu/Debian
   sudo apt-get install postgresql postgresql-contrib
   
   # Windows (usando Chocolatey)
   choco install postgresql
   
   # macOS (usando Homebrew)
   brew install postgresql
   ```

2. **Criar o banco de dados**
   ```sql
   CREATE DATABASE LibraryManagementDb;
   ```

3. **Configurar a string de conexão**
   
   Edite o arquivo `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=LibraryManagementDb;Username=postgres;Password=sua_senha"
     }
   }
   ```

### Instalação e Execução

1. **Clonar o repositório**
   ```bash
   git clone <url-do-repositorio>
   cd library_management/backend/LibraryManagement.Api
   ```

2. **Restaurar dependências**
   ```bash
   dotnet restore
   ```

3. **Aplicar migrations**
   ```bash
   dotnet ef database update
   ```

4. **Executar a aplicação**
   ```bash
   dotnet run --urls "http://0.0.0.0:5001"
   ```

A API estará disponível em `http://localhost:5001`

## Documentação da API

### Swagger UI

Acesse `http://localhost:5001/swagger` para visualizar a documentação interativa da API.

### Endpoints Principais

#### Gêneros

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/v1/genres` | Lista todos os gêneros com paginação |
| GET | `/api/v1/genres/{id}` | Obtém um gênero específico |
| POST | `/api/v1/genres` | Cria um novo gênero |
| PUT | `/api/v1/genres/{id}` | Atualiza um gênero existente |
| DELETE | `/api/v1/genres/{id}` | Remove um gênero |

#### Autores

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/v1/authors` | Lista todos os autores com paginação |
| GET | `/api/v1/authors/{id}` | Obtém um autor específico |
| POST | `/api/v1/authors` | Cria um novo autor |
| PUT | `/api/v1/authors/{id}` | Atualiza um autor existente |
| DELETE | `/api/v1/authors/{id}` | Remove um autor |

#### Livros

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/v1/books` | Lista todos os livros com paginação |
| GET | `/api/v1/books/{id}` | Obtém um livro específico |
| POST | `/api/v1/books` | Cria um novo livro |
| PUT | `/api/v1/books/{id}` | Atualiza um livro existente |
| DELETE | `/api/v1/books/{id}` | Remove um livro |

### Parâmetros de Query

#### Paginação
- `pageNumber`: Número da página (padrão: 1)
- `pageSize`: Tamanho da página (padrão: 10)

#### Busca
- `searchTerm`: Termo de busca para filtrar resultados

#### Filtros (apenas para livros)
- `authorId`: ID do autor para filtrar livros
- `genreId`: ID do gênero para filtrar livros

### Exemplos de Uso

#### Criar um Gênero
```bash
curl -X POST "http://localhost:5001/api/v1/genres" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Ficção Científica",
    "description": "Livros de ficção científica e fantasia"
  }'
```

#### Listar Livros com Paginação
```bash
curl "http://localhost:5001/api/v1/books?pageNumber=1&pageSize=5"
```

#### Buscar Autores
```bash
curl "http://localhost:5001/api/v1/authors?searchTerm=Machado"
```

## Modelos de Dados

### Genre (Gênero)
```csharp
public class Genre
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<Book> Books { get; set; }
}
```

### Author (Autor)
```csharp
public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Biography { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public string? Nationality { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<Book> Books { get; set; }
}
```

### Book (Livro)
```csharp
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string? ISBN { get; set; }
    public string? Description { get; set; }
    public DateTime? PublicationDate { get; set; }
    public string? Publisher { get; set; }
    public int? Pages { get; set; }
    public decimal? Price { get; set; }
    public int AuthorId { get; set; }
    public int GenreId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Author Author { get; set; }
    public Genre Genre { get; set; }
}
```

## Testes

### Executar Testes
```bash
dotnet test
```

### Executar Testes com Cobertura
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Estrutura de Testes

Os testes estão organizados na pasta `Tests/` e seguem a convenção:
- `Services/`: Testes unitários dos serviços
- Cada classe de teste usa banco em memória para isolamento

### Exemplo de Teste
```csharp
[Fact]
public async Task GetAllAsync_ReturnsPagedResponse()
{
    // Arrange
    var genre1 = new Genre { Id = 1, Name = "Ficção" };
    var genre2 = new Genre { Id = 2, Name = "Romance" };
    
    _context.Genres.AddRange(genre1, genre2);
    await _context.SaveChangesAsync();

    // Act
    var result = await _genreService.GetAllAsync(1, 10);

    // Assert
    Assert.True(result.Success);
    Assert.Equal(2, result.TotalCount);
}
```

## Configurações de Ambiente

### Development
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=LibraryManagementDb;Username=postgres;Password=postgres"
  },
  "AllowedHosts": "*"
}
```

### Production
- Configure variáveis de ambiente para strings de conexão
- Use HTTPS
- Configure logging apropriado
- Implemente autenticação/autorização se necessário

## Deployment

### Docker
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["LibraryManagement.Api.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LibraryManagement.Api.dll"]
```

### Azure App Service
1. Publique usando Visual Studio ou CLI
2. Configure a string de conexão nas configurações do App Service
3. Configure variáveis de ambiente necessárias

## Troubleshooting

### Problemas Comuns

1. **Erro de conexão com PostgreSQL**
   - Verifique se o PostgreSQL está rodando
   - Confirme as credenciais na string de conexão
   - Teste a conectividade: `psql -h localhost -U postgres`

2. **Erro de migrations**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

3. **Porta em uso**
   ```bash
   dotnet run --urls "http://0.0.0.0:5002"
   ```

### Logs

Os logs são configurados no `Program.cs` e podem ser visualizados no console durante o desenvolvimento.

## Contribuição

1. Siga os padrões de código estabelecidos
2. Escreva testes para novas funcionalidades
3. Mantenha a documentação atualizada
4. Use commits semânticos

## Licença

Este projeto está sob a licença MIT.

