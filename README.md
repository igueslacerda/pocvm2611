# pocvm2611

Prova de conceito de uma API ASP.NET Core com PostgreSQL e autenticação JWT.

## Pré-requisitos

- .NET 10 SDK
- PostgreSQL com as tabelas `public.si_pessoa`, `public.ac_usuario` e `public.ac_log`

Configure as variáveis de ambiente antes de iniciar a API:

```powershell
$env:ConnectionStrings__DefaultConnection = "Host=localhost;Port=5432;Database=nome_do_banco;Username=usuario;Password=senha"
$env:Jwt__Key = "uma-chave-aleatoria-com-pelo-menos-32-bytes"
dotnet run --project .\pocservicedn
```

Em produção, mantenha a chave JWT e a senha do banco em um gerenciador de segredos ou variáveis de ambiente, nunca no repositório. O serviço rejeita chaves JWT menores que 32 bytes. O issuer, audience e validade do token estão configurados em `pocservicedn/appsettings.json`.

## Banco de dados e senhas

O `DbContext` mapeia as tabelas existentes e não cria nem altera o schema automaticamente. Antes de armazenar senhas com o hasher padrão do ASP.NET Core, aumente o tamanho da coluna, pois o hash não cabe em `varchar(50)`:

```sql
ALTER TABLE public.ac_usuario
    ALTER COLUMN senha TYPE character varying(255);
```

`ac_usuario.senha` deve conter hashes gerados pelo `PasswordHasher` do ASP.NET Core, e não senhas em texto puro. Senhas já existentes em texto puro precisam ser convertidas antes de serem usadas neste fluxo.
Se o login registrar que o hash armazenado não é Base64 válido, verifique se a coluna contém texto puro ou se o hash foi truncado; mantenha a coluna com pelo menos 255 caracteres.

As entidades representam somente as colunas de suas tabelas, sem propriedades de navegação. Consultas que precisam de dados relacionados fazem o `JOIN` explicitamente; o perfil do usuário é um exemplo.

## Endpoints

| Método | Rota | Acesso |
|---|---|---|
| `POST` | `/api/auth/login` | Anônimo |
| `PATCH` | `/api/auth/password` | JWT obrigatório |
| `GET` | `/api/auth/me` | JWT obrigatório |

O login recebe `login`, `senha` e `origem` (`Web` ou `Mobile`). Em caso de sucesso, grava um registro em `ac_log` usando `AcLogAcaoEnum.Login` (`1`) e os códigos de origem (`Web=1`, `Mobile=2`); a data é preenchida pelo default do banco. A troca de senha recebe `login`, `senhaAtual` e `novaSenha`; o login informado deve corresponder ao usuário do token. O endpoint `me` retorna os dados do usuário autenticado e da pessoa relacionada.

Use `pocservicedn/pocservicedn.http` como exemplos de chamadas. Em desenvolvimento, o documento OpenAPI está disponível em `/openapi/v1.json`. A interface Scalar fica disponível em `/scalar/v1` apenas quando a aplicação é executada em `Development` usando a configuração `Debug`; ela não é incluída em builds `Release`.
