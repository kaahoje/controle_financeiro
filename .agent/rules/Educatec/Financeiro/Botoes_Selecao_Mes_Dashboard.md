# Conversão do Selecionador de Meses para Botões de Atalho no Dashboard

## 1. Objetivo
Substituir o dropdown de seleção do mês por um conjunto de botões no Dashboard (`/`), mantendo o seletor de ano à esquerda.

## 2. Solicitação original
"Na tela inicial '/' preciso que converta a seleção dos meses para botões (jan, fev, mar, etc) que ao ser clicado carrega o resumo do mês. Pode manter no momento o campo de seleção do ano que deve ficar à esquerda dos botões."

## 3. Contexto considerado
- `GestorContas.Web/Views/Home/Index.cshtml`

## 4. Fontes utilizadas
- `GestorContas.Web/Views/Home/Index.cshtml`

## 5. Análise realizada
- O elemento `<select name="mes">` foi substituído por um grupo de 12 botões (`.btn-group`), identificados de `Jan` a `Dez`.
- O valor do mês selecionado passou a ser controlado por um campo `<input type="hidden" name="mes" id="filtroMesDashboard" />`.
- O dropdown de **Ano** permanece posicionado à esquerda dos botões do mês.
- O clique em qualquer um dos botões dispara o carregamento AJAX do resumo do mês e reinicializa os gráficos instantaneamente.

## 6. Decisões tomadas
- Destacar o botão do mês ativo com a classe `btn-primary active fw-bold`.

## 7. Resultado produzido
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Views\Home\Index.cshtml`

## 8. Regras e critérios consolidados
- Navegação fluida de meses por botões sem necessidade de recarregar a página.

## 9. Pendências e próximos passos
**Nenhuma pendência identificada.**

## 10. Observações para continuidade
- Permite alternar rapidamente os meses no Dashboard com um único clique nos botões Jan..Dez.
