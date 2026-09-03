# Painel de Consulta de Saldo das Contas por Data de Corte

## 1. Objetivo
Permitir a apuração do saldo acumulado de cada conta em qualquer dia histórico através de um seletor interativo de data de corte.

## 2. Solicitação original
"Adicione um painel que mostre o saldo das contas, mas que permita definir a data de corte para verificar o saldo das contas em um dia específico."

## 3. Contexto considerado
- `GestorContas.Web/Controllers/HomeController.cs`
- `GestorContas.Web/Views/Home/Diagnostico.cshtml`

## 4. Fontes utilizadas
- `GestorContas.Web/Controllers/HomeController.cs`
- `GestorContas.Web/Views/Home/Diagnostico.cshtml`

## 5. Análise realizada
- Criada a action `ObterSaldoDataCorteJson(DateTime dataCorte)` no `HomeController.cs` que recalcula as entradas, saídas e o saldo acumulado de cada conta desde o saldo inicial até a data de corte informada (`23:59:59`).
- Adicionado o painel de consulta na seção **1. Balanço das Contas & Saldo Por Data de Corte** com seletor de data (`type="date"`).
- Ao clicar em "Consultar Saldo na Data", a tabela reflete instantaneamente os saldos apurados e o total geral na data informada via AJAX.

## 6. Decisões tomadas
- Apurar o saldo até o fim do dia selecionado (`23:59:59.999`) para incluir todas as movimentações daquela data.
- Adicionar o botão "Ver Saldo Atual" para redefinir rapidamente a tabela para a data presente.

## 7. Resultado produzido
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Controllers\HomeController.cs`
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Views\Home\Diagnostico.cshtml`

## 8. Regras e critérios consolidados
- Saldo na Data = Saldo Inicial + Soma(Entradas até Data) - Soma(Saídas até Data).

## 9. Pendências e próximos passos
**Nenhuma pendência identificada.**

## 10. Observações para continuidade
- Permite verificar o saldo bancário de qualquer dia passado (ex: 31/01/2026, 15/06/2026) e bater diretamente com os extratos das contas naquela data.
