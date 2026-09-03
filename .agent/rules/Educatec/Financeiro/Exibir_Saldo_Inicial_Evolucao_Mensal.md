# Inclusão do Saldo Inicial na Tabela de Evolução Mensal

## 1. Objetivo
Incluir o valor total dos saldos iniciais no topo da tabela de evolução mensal para servir de marca d'água/referência inicial dos cálculos.

## 2. Solicitação original
"Ainda assim não me convenceu.... Adicione no início da evolução do mês o total dos saldos iniciais para que possa tê-lo como referência."

## 3. Contexto considerado
- `GestorContas.Web/Views/Home/Diagnostico.cshtml`

## 4. Fontes utilizadas
- `GestorContas.Web/Views/Home/Diagnostico.cshtml`

## 5. Análise realizada
- Adicionada a linha de destaque no topo da tabela da Seção 2 (`Evolução do Saldo Acumulado Mês a Mês`).
- Exibe o valor total de `SaldoInicialTotalContas` (soma dos saldos iniciais cadastrados nas contas ativas e consideradas no diagnóstico) como linha de partida (`table-info`).

## 6. Decisões tomadas
- Destacar o valor em azul/info no topo da lista antes da primeira linha de mês (`jan./2026`).

## 7. Resultado produzido
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Views\Home\Diagnostico.cshtml`

## 8. Regras e critérios consolidados
- O cálculo de evolução de saldo mensal parte sempre da soma dos saldos iniciais cadastrados nas contas.

## 9. Pendências e próximos passos
**Nenhuma pendência identificada.**

## 10. Observações para continuidade
- A linha de topo serve como ponto zero (acumulado inicial) antes de aplicar os resultados operacionais de cada mês.
