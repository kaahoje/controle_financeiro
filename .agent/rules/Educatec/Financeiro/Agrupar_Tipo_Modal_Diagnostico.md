# Opção de Agrupar por Tipo no Modal do Mês do Diagnóstico

## 1. Objetivo
Adicionar a funcionalidade de agrupar os lançamentos do mês por Entrada, Saída e Transferência no modal exibido ao clicar em um mês na tela de Diagnóstico.

## 2. Solicitação original
"Adicione a opção de agrupar por entrada/saída na lista que é aberta ao clicar no mês."

## 3. Contexto considerado
- `GestorContas.Web/Views/Home/Diagnostico.cshtml`

## 4. Fontes utilizadas
- `GestorContas.Web/Views/Home/Diagnostico.cshtml`

## 5. Análise realizada
- Adicionado um interruptor (switch) **"Agrupar por Tipo"** no cabeçalho de filtros do modal de detalhes do mês.
- Ao ativar a opção, a lista de lançamentos é separada dinamicamente em 3 blocos visuais distintos com subtotais destacados:
  - 🟢 **ENTRADAS OPERACIONAIS**
  - 🔴 **SAÍDAS OPERACIONAIS**
  - 🔵 **TRANSFERÊNCIAS INTERNAS**

## 6. Decisões tomadas
- Exibir os cabeçalhos de grupo coloridos com contagem de itens e valor total acumulado do grupo.

## 7. Resultado produzido
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Views\Home\Diagnostico.cshtml`

## 8. Regras e critérios consolidados
- Agrupamento em tempo real no cliente (JavaScript) respeitando os demais filtros de texto e tipo.

## 9. Pendências e próximos passos
**Nenhuma pendência identificada.**

## 10. Observações para continuidade
- Permite analisar instantaneamente o subtotal de entradas vs saídas vs transferências ao inspecionar o desvio de qualquer mês.
