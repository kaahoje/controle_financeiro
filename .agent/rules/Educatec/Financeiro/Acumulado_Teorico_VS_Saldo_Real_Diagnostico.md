# Adição de Coluna de Acumulado Teórico vs Saldo Real no Diagnóstico

## 1. Objetivo
Adicionar na tabela de evolução mensal a coluna de **Acumulado Teórico** (soma pura dos resultados operacionais mensais partindo do saldo inicial) para comparar lado a lado com o **Saldo Real (Extrato)** de encerramento do mês.

## 2. Solicitação original
"A coluna saldo no final do mês está comparando a evolução prevista no diagnóstico ou está buscando o saldo geral? Para diagnosticar melhor preciso de uma coluna que faça o acumulado com base nos resultados mensais para comparar com o saldo real mês a mês com base no extrato lançado."

## 3. Contexto considerado
- `GestorContas.Web/Services/Diagnostico/Dtos/DiagnosticoFinanceiroDto.cs`
- `GestorContas.Web/Services/Diagnostico/DiagnosticoFinanceiroService.cs`
- `GestorContas.Web/Views/Home/Diagnostico.cshtml`

## 4. Fontes utilizadas
- `GestorContas.Web/Views/Home/Diagnostico.cshtml`

## 5. Análise realizada
- A antiga coluna "Saldo Fim do Mês" trazia o saldo real bancário acumulado (com base nas entradas, saídas e transferências).
- Criada a nova propriedade `SaldoAcumuladoResultadoMes` que faz o acumulado matemático puro: `Saldo Inicial + Sum(Resultado Mês de cada mês)`.
- Renderizadas as duas colunas lado a lado na tabela da Seção 2:
  - **Acumulado Teórico**: O saldo previsto unicamente pelas sobras/déficits dos meses.
  - **Saldo Real (Extrato)**: O saldo bancário apurado no fechamento de cada mês.

## 6. Decisões tomadas
- Apresentar ambas as colunas na tabela da Seção 2 para permitir comparação visual direta de onde o saldo teórico diverge do saldo real.

## 7. Resultado produzido
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Services\Diagnostico\Dtos\DiagnosticoFinanceiroDto.cs`
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Services\Diagnostico\DiagnosticoFinanceiroService.cs`
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Views\Home\Diagnostico.cshtml`

## 8. Regras e criteria consolidados
- Acumulado Teórico = Saldo Inicial + Soma acumulada de (Entradas Operacionais - Saídas Operacionais).

## 9. Pendências e próximos passos
**Nenhuma pendência identificada.**

## 10. Observações para continuidade
- A comparação entre as duas colunas permite identificar instantaneamente em qual mês o caixa real desviou do resultado teórico projetado.
