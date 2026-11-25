# Filtro de Mês, Ano "Todos", Busca Textual Insensível à Caixa e Publicação no IIS no Módulo de Prefeituras

## 1. Objetivo
1. Permitir a remoção do filtro por mês e do filtro por ano na tela de Prefeituras (`/prefeituras`), possibilitando a visualização de todos os registros ("Todos") para cada um ou ambos os filtros.
2. Garantir que o filtro textual de busca por descrição filtre em qualquer parte da palavra e seja insensível à caixa (case-insensitive).
3. Publicar a aplicação no IIS local (`http://localhost:8081/`).

## 2. Solicitação original
- Solicitação 1: "Na tela "/prefeituras" deve ser possível remover o filtro por mês (Todos) e o filtro por ano também (Todos). Faça o ajuste."
- Solicitação 2: "O filtro textual deve filtra o texto em qualquer parte da palavra e ser insensível à caixa."
- Solicitação 3: "Agora faça a publicação no IIS local em "http://localhost:8081/""

## 3. Contexto considerado
- Módulo: Prefeituras
- Funcionalidade: Filtro de listagem por vencimento, busca por descrição e publicação IIS
- Arquivos:
  - `GestorContas.Web/Controllers/PrefeiturasController.cs`
  - `GestorContas.Web/Views/Prefeituras/Index.cshtml`
  - `GestorContas.Web/Views/Prefeituras/_GridPrefeituras.cshtml`
  - `c:\Site\Controle_Financeiro` (diretório raiz do site IIS na porta 8081)

## 4. Fontes utilizadas
- `GestorContas.Web/Controllers/PrefeiturasController.cs`
- `GestorContas.Web/Views/Prefeituras/Index.cshtml`
- `GestorContas.Web/Views/Prefeituras/_GridPrefeituras.cshtml`
- `GestorContas.Web/Properties/PublishProfiles/FolderProfile.pubxml`
- `GestorContas.Web/Properties/PublishProfiles/FolderProfile.pubxml.user`

## 5. Análise realizada
- Na tela `/prefeituras`, os selects de Mês e de Ano possuíam apenas valores fixos específicos (1 a 12 para meses e intervalo de anos), sem a opção de selecionar "Todos" (valor `0`).
- No backend (`PrefeiturasController.cs`), a ação `Index` assumia o mês e o ano correntes caso não informados ou filtrava estritamente por mês e ano. Ao selecionar "Todos" (valor `0`), ocorreria falha de consulta ou tentativa de construir `DateTime` inválido com ano ou mês zero.
- Os totalizadores cumulativos foram ajustados para respeitar a ausência de corte de data quando o ano for "Todos" (`ano == 0`) e o final do ano quando apenas o mês for "Todos" (`ano > 0 && mes == 0`).
- O badge de contagem de registros foi ajustado para exibir contextualmente "registros no mês", "registros no ano" ou "registros".
- O filtro textual utilizava `p.Descricao.Contains(busca)`. No SQLite, a função LINQ `string.Contains` é traduzida para `instr()`, que é estritamente sensível à caixa (case-sensitive). A consulta foi alterada para `EF.Functions.Like(p.Descricao, $"%{termo}%")`, que no SQLite realiza busca de substring em qualquer posição da palavra de forma insensível à caixa (case-insensitive).
- A publicação no IIS local (`http://localhost:8081/`, apontando para `c:\Site\Controle_Financeiro`) foi realizada utilizando `app_offline.htm` temporário para liberar arquivos em uso pelo `w3wp.exe` / `AspNetCoreModuleV2`, executando `dotnet publish` com configuração Release e validando via requisição HTTP com retorno 200 OK.

## 6. Decisões tomadas
- **Model Decision**: Utilizar valor `0` para representar a seleção "Todos" nos campos de Mês e Ano, alinhando com o comportamento já existente no módulo de Lançamentos.
- **Model Decision**: Utilizar `EF.Functions.Like(p.Descricao, $"%{termo}%")` com `busca.Trim()` para a busca textual, assegurando filtragem por substring em qualquer parte do texto e insensibilidade à caixa (maiúsculas/minúsculas).
- **Model Decision**: Publicação no IIS utilizando `app_offline.htm` para garantir substituição atômica e segura das DLLs e executáveis da aplicação sem bloqueios de processo do IIS.
- Condicionar a filtragem de `gridQuery`:
  - Se `mesSelecionado > 0 && anoSelecionado > 0`: filtra por mês e ano.
  - Se apenas `mesSelecionado > 0`: filtra apenas por mês em todos os anos.
  - Se apenas `anoSelecionado > 0`: filtra apenas por ano em todos os meses.
  - Se ambos forem `0`: traz todos os registros sem restrição de período.
- Totalizadores cumulativos:
  - Com ano e mês selecionados: corte no final do mês.
  - Com ano selecionado e mês "Todos": corte no final do ano.
  - Com ano "Todos": considera a base cumulativa integral.

## 7. Resultado produzido

### Requisitos criados
Não houve novos requisitos funcionais criados no edital/especificação formal.

### Requisitos alterados
Não houve alteração de especificação formal.

### Requisitos descartados por duplicidade
Não houve.

### Arquivos alterados
- `GestorContas.Web/Controllers/PrefeiturasController.cs`:
  - Lógica de filtros e totalizadores suportando valor `0` ("Todos") para mês e ano.
  - Filtro textual atualizado de `p.Descricao.Contains(busca)` para `EF.Functions.Like(p.Descricao, $"%{termo}%")` com `string.IsNullOrWhiteSpace` e `Trim()`.
- `GestorContas.Web/Views/Prefeituras/Index.cshtml`: Inclusão da opção `<option value="0">Todos</option>` nos selects de Mês e Ano.
- `GestorContas.Web/Views/Prefeituras/_GridPrefeituras.cshtml`: Ajuste do badge de contagem para refletir o escopo selecionado (mês, ano ou total).
- `GestorContas.Web/Properties/PublishProfiles/FolderProfile.pubxml.user`: Atualização da connection string de publicação para refletir o ambiente local.
- Publicação realizada com sucesso em `c:\Site\Controle_Financeiro` (IIS `http://localhost:8081/`).

## 8. Regras e critérios consolidados
- Manter consistência do valor `0` para "Todos" em filtros de data compostos por mês e ano.
- Utilizar `EF.Functions.Like` para buscas textuais no SQLite quando o comportamento esperado for insensível à caixa (case-insensitive).
- Procedimento de publicação no IIS local: colocar `app_offline.htm`, executar `dotnet publish -c Release -r win-x64 --no-self-contained -o c:\Site\Controle_Financeiro`, remover `app_offline.htm` e testar endpoint HTTP.

## 9. Pendências e próximos passos
**Nenhuma pendência identificada.**

## 10. Observações para continuidade
- A aplicação no IIS (`http://localhost:8081/`) está atualizada e operacional, validada com retorno 200 OK.
