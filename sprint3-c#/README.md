# Sprint3 - Usuarios e Instituições

Esta aplicação console em .NET 8 demonstra:
- Estrutura de classes (models, repository)
- Conexão com PostgreSQL usando Npgsql e Dapper (CRUD completo)
- Import/Export JSON e TXT
- Arquitetura simples: Program -> Repository -> Database

Configuração:
- Edite appsettings.json para configurar a connection string do PostgreSQL.
- Crie o banco `sprint3_db` em sua instância PostgreSQL antes de executar, ou altere a string.

Como executar:
- dotnet build
- dotnet run

