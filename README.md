# NOMES

Felipe Voidela Toledo - 98595

Kayky Oliveira Schunck - 99756

Leonardo Schunck Rainha - 99902

Paulo Lopes Junior - 551137

Ricardo Augusto de Matos Filho - 95906


# Documentação do Sistema — Usuários e Instituições

**Arquivo:** `Documentacao_Sistema_Usuarios_Instituicoes.md`

---

## Visão Geral

Aplicação console em C# que gerencia Usuários e Instituições com persistência em PostgreSQL. Funcionalidades principais:

* CRUD de Usuários
* CRUD de Instituições
* Importação/Exportação em JSON e TXT
* Arquitetura simples baseada em um repositório (`Repository`) que encapsula acesso ao banco

O programa é destinado a fins de estudo/portfólio: foco em organização de camadas e operações I/O.

---

## Estrutura de Pastas (sugerida)

```
/src
  /App (Program.cs)
  /Data (AppDbContext.cs ou Repository.cs)
  /Models (User.cs, Institution.cs)
  /Migrations (se usar EF)
  /Utils (JsonHelper.cs, FileHelper.cs)
/tests
  /UnitTests
/docs
  Documentacao_Sistema_Usuarios_Instituicoes.md
```

---

## Requisitos

* .NET 8 (recomendado)
* PostgreSQL (versão compatível com Npgsql instalado)
* Pacotes NuGet: `Npgsql`, `Dapper` (ou `Microsoft.EntityFrameworkCore.*` se preferir EF Core), `Microsoft.Extensions.Configuration.Json`

---

## Como Executar (setup rápido)

1. Configure `appsettings.json` na raiz do projeto com `ConnectionStrings:Default` apontando para seu PostgreSQL.
2. Assegure que o banco `sprint3_db` exista ou crie-o.
3. Restaurar pacotes: `dotnet restore`
4. Build: `dotnet build`
5. Rodar: `dotnet run --project ./src/App`

> O programa também aceita uma `connection string` padrão embutida como fallback (Host=localhost;Port=5432;Username=postgres;Password=Felipe123;Database=sprint3\_db).

---

## Modelos (Models)

### User

* `int Id`
* `string Name`
* `string Email`
* `int? InstitutionId`

### Institution

* `int Id`
* `string Name`
* `string Address`


---

## Camada de Persistência (Repository)

O `Repository` encapsula todas as operações de DB e expõe métodos assíncronos:

* Inicialização: `InitializeAsync()` (criar tabelas se necessário)
* Usuários: `GetUsersAsync()`, `GetUserAsync(id)`, `CreateUserAsync(user)`, `UpdateUserAsync(user)`, `DeleteUserAsync(id)`
* Instituições: `GetInstitutionsAsync()`, `GetInstitutionAsync(id)`, `CreateInstitutionAsync(inst)`, `UpdateInstitutionAsync(inst)`, `DeleteInstitutionAsync(id)`


## Fluxo da Aplicação (Program.cs)

1. Carrega configuração (`appsettings.json`) via `ConfigurationBuilder`.
2. Constrói `Repository` com `connectionString`.
3. Chama `InitializeAsync()`.
4. Mostra menu principal e entra em loops para gerenciar usuários, instituições e import/export.

**Tratamento de erros:** `try/catch` ao redor do menu principal e em blocos para entradas inválidas (FormatException) e `FileNotFoundException` nas importações.

---

## Importação / Exportação

* **Exportar JSON**: serializa `{ users, institutions }` para `export/export.json` (pasta criada programaticamente).
* **Importar JSON**: lê `export.json` e desserializa `institutions` e `users`, chamando os métodos `Create*Async` para cada item.
* **Exportar TXT**: salva um arquivo `export.txt` com seções `Institutions:` e `Users:` e linhas separadas por pipe (`|`).
* **Importar TXT**: parseia linhas e cria entidades de acordo com a seção atual.


## Banco de Dados — Esquema SQL sugerido

```sql
CREATE TABLE institutions (
  id serial PRIMARY KEY,
  name varchar(255) NOT NULL,
  address varchar(512)
);

CREATE TABLE users (
  id serial PRIMARY KEY,
  name varchar(255) NOT NULL,
  email varchar(255) NOT NULL UNIQUE,
  institution_id integer REFERENCES institutions(id)
);
```

---


*Fim da documentação.*

