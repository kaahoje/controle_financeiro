# Backup do Banco de Dados SQLite

## 1. Objetivo
Disponibilizar um recurso rápido de backup do banco de dados SQLite diretamente na barra de exibição do caminho do banco no layout da aplicação, gerando uma cópia com carimbo de data e hora no nome do arquivo.

## 2. Solicitação original
"Crie um recurso de backup do banco de dados da aplicação que faça uma cópia adicionando no nome do arquivo o dia, hora, minuto, segundo em que foi feito. O botão deve ficar ao lado do campo que exibe o caminho atual do banco da aplicação."

## 3. Contexto considerado
- `GestorContas.Web/Views/Shared/_Layout.cshtml`
- `GestorContas.Web/Controllers/HomeController.cs`
- `GestorContas.Web/wwwroot/css/site.css`
- `GestorContas.Web/wwwroot/js/site.js`

## 4. Fontes utilizadas
- `GestorContas.Web/Views/Shared/_Layout.cshtml`
- `GestorContas.Web/Controllers/HomeController.cs`
- `GestorContas.Web/appsettings.json`
- `GestorContas.Web/wwwroot/js/site.js`
- `GestorContas.Web/wwwroot/css/site.css`

## 5. Análise realizada
- A barra superior de informações do banco (`.db-info-bar`) em `_Layout.cshtml` lê `ConnectionStrings:DefaultConnection` e exibe o caminho em `.db-path-badge`.
- Como a aplicação utiliza SQLite (`Microsoft.EntityFrameworkCore.Sqlite` / `Microsoft.Data.Sqlite`), a cópia foi implementada com `SqliteConnection.BackupDatabase` seguida de `SqliteConnection.ClearPool(destination)` para garantir consistência mesmo com conexões abertas em pool e liberar o arquivo de backup imediatamente.
- O arquivo de backup é gravado no mesmo diretório do banco original com o sufixo `_yyyyMMdd_HHmmss` antes da extensão (ex.: `GestorContas_20261125_081732.db`).

## 6. Decisões tomadas
- Utilizar `SqliteConnection.BackupDatabase` em vez de `File.Copy` direto para evitar inconsistências caso haja transações/WAL ativos ou bloqueios no pool de conexões.
- Posicionar o botão `#btnBackupDb` dentro de `.db-info-content`, imediatamente após `.db-path-badge`, mantendo o padrão visual arredondado da barra.
- Exibir feedback ao usuário via `showToast` já existente em `site.js`, com estado de carregamento no botão durante a requisição POST para `/Home/FazerBackup`.

## 7. Resultado produzido
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Controllers\HomeController.cs` — adicionada injeção de `IConfiguration` e action `[HttpPost] FazerBackup()`.
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Views\Shared\_Layout.cshtml` — adicionado botão `#btnBackupDb` ao lado de `.db-path-badge`.
- `c:\Projetos\Controle Financeiro\GestorContas.Web\wwwroot\css\site.css` — adicionada classe `.btn-db-backup`.
- `c:\Projetos\Controle Financeiro\GestorContas.Web\wwwroot\js\site.js` — adicionado manipulador de clique para `#btnBackupDb`.

## 8. Regras e critérios consolidados
- O nome do arquivo de backup preserva o nome e a extensão originais, inserindo `_yyyyMMdd_HHmmss` antes da extensão.
- O backup é salvo no mesmo diretório do arquivo de banco configurado em `DefaultConnection`.

## 9. Pendências e próximos passos
**Nenhuma pendência identificada.**

## 10. Observações para continuidade
- Caso futuramente seja necessário escolher um diretório customizado para backups, basta ler uma chave adicional de configuração em `IConfiguration` dentro de `HomeController.FazerBackup()`.
