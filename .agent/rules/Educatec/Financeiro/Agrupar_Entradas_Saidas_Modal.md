# Agrupamento Estrito por Entradas e Saídas no Modal de Lançamentos

## 1. Objetivo
Reorganizar a exibição agrupada do modal do mês para agrupar estritamente entre Entradas Principais e Saídas Principais (separando em subseções operacionais e de transferência).

## 2. Solicitação original
"Adicione o agrupamento para agrupar as entradas e as saídas."

## 3. Contexto considerado
- `GestorContas.Web/Views/Home/Diagnostico.cshtml`

## 4. Fontes utilizadas
- `GestorContas.Web/Views/Home/Diagnostico.cshtml`

## 5. Análise realizada
- O agrupamento no modal foi reestruturado em dois blocos principais:
  - 🟢 **TODAS AS ENTRADAS DO MÊS**: Somando tanto receitas operacionais quanto transferências recebidas com subtotal consolidado.
  - 🔴 **TODAS AS SAÍDAS DO MÊS**: Somando tanto despesas operacionais quanto transferências enviadas com subtotal consolidado.
- Cada grupo principal contém subcabeçalhos de indentação para discriminar as movimentações de transferência interna das movimentações de receita/despesa pura.

## 6. Decisões tomadas
- Apresentar subtotais destacados tanto no topo dos blocos principais (Entradas / Saídas) quanto nas subcategorias de Transferência.

## 7. Resultado produzido
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Views\Home\Diagnostico.cshtml`

## 8. Regras e critérios consolidados
- Agrupamento visual intuitivo com subtotais instantâneos de Entradas Totais e Saídas Totais.

## 9. Pendências e próximos passos
**Nenhuma pendência identificada.**

## 10. Observações para continuidade
- Ao analisar qualquer mês, ligar o switch "Agrupar por Tipo" compara imediatamente a soma bruta de todas as Entradas daquele mês contra a soma bruta de todas as Saídas.
