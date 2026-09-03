# Reinicialização Dinâmica dos Gráficos Chart.js via AJAX

## 1. Objetivo
Garantir que os gráficos (Fluxo de Caixa, Top Despesas por Categoria e Evolução de Saldo) sejam reconstruídos e atualizados dinamicamente ao trocar o mês via filtro AJAX.

## 2. Solicitação original
"Os gráficos não estão sendo recarregados."

## 3. Contexto considerado
- `GestorContas.Web/Views/Home/Index.cshtml`

## 4. Fontes utilizadas
- `GestorContas.Web/Views/Home/Index.cshtml`

## 5. Análise realizada
- Quando o HTML do contêiner do Dashboard era substituído (`replaceWith`), os elementos `<canvas>` anteriores eram descartados, mas as instâncias do Chart.js associadas precisavam ser explicitamente encerradas via `destroy()` antes de rebindar os novos dados nos elementos recém-inseridos.
- Foi implementado a extração do bloco de scripts retornado na requisição parcial AJAX com a destruição e recriação limpa de `window.chartFluxoCaixaInst`, `window.chartDespesasInst` e `window.chartEvolucaoSaldoInst`.

## 6. Decisões tomadas
- Destruir com segurança os gráficos anteriores e reinicializar com os novos dados de forma isolada no manipulador AJAX.

## 7. Resultado produzido
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Views\Home\Index.cshtml`

## 8. Regras e critérios consolidados
- Destruir a instância anterior do Chart.js antes de vincular novos dados a um canvas.

## 9. Pendências e próximos passos
**Nenhuma pendência identificada.**

## 10. Observações para continuidade
- Os gráficos atualizam instantaneamente a cada clique no mês sem requisições em duplicidade.
