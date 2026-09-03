# Diagnóstico e Auditoria Financeira de Saldos

## 1. Objetivo
Criar um módulo e serviço permanente de auditoria financeira (`/Home/Diagnostico`) para investigar discrepâncias entre os saldos mensais e o montante real acumulado no caixa.

## 2. Solicitação original
"Crie esse endpoint de diagnóstico, mas não será temporário. Use todas as técnicas possíveis para entender o que está acontecendo."

## 3. Contexto considerado
- Sistema: `GestorContas.Web`
- Módulo: Finanças / Conciliação / Diagnósticos
- Contexto de negócio: O usuário possuía fechamentos mensais superavitários, mas o saldo acumulado total não correspondia às expectativas. Era necessário auditá-los contra duplicidades, saldos iniciais descalibrados, erros de sinal e transferências desbalanceadas.

## 4. Fontes utilizadas
- `GestorContas.Web/Models/Lancamento.cs`
- `GestorContas.Web/Models/Conta.cs`
- `GestorContas.Web/Controllers/HomeController.cs`
- `GestorContas.Web/Data/AppDbContext.cs`

## 5. Análise realizada
Foram identificadas 5 áreas principais de auditoria:
1. **Saldos Iniciais e Balanço por Conta**: Comparação do `SaldoInicial` com o somatório histórico de Entradas e Saídas (operacionais vs transferências).
2. **Duplicidades na Base de Dados**: Agrupamento por `(Data, Valor, Tipo, ContaId)` com cálculo de redundância de saldo.
3. **Desbalanço de Transferências**: Identificação de transferências entre contas sem a respectiva contrapartida cadastrada na conta de destino.
4. **Suspeitas de Inversão de Tipo/Sinal**: Análise de descrições contendo termos de receita ("Estorno", "Rendimento") gravadas como Saída ou vice-versa.
5. **Evolução do Saldo Mês a Mês**: Tabela temporal completa que permite rastrear em qual mês exato o saldo sofreu a primeira queda/desvio relevante.

## 6. Decisões tomadas
- Criado o serviço `IDiagnosticoFinanceiroService` e a implementação `DiagnosticoFinanceiroService` sob a namespace `GestorContas.Web.Services.Diagnostico`.
- Criados os DTOs de auditoria em `DiagnosticoFinanceiroDto.cs`.
- Criado o endpoint permanente `/Home/Diagnostico` (view interativa) e `/Home/DiagnosticoJson` (exportação bruta).
- Adicionada a opção "Diagnóstico" na barra de navegação superior (`_Layout.cshtml`).

## 7. Resultado produzido
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Services\Diagnostico\Dtos\DiagnosticoFinanceiroDto.cs`
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Services\Diagnostico\IDiagnosticoFinanceiroService.cs`
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Services\Diagnostico\DiagnosticoFinanceiroService.cs`
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Views\Home\Diagnostico.cshtml`
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Controllers\HomeController.cs` (atualizado)
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Views\Shared\_Layout.cshtml` (atualizado)

## 8. Regras e critérios consolidados
- As transferências internas (`ParaTransferencia == true`) devem ser separadas do resultado operacional de receitas/despesas para não distorcer o resultado líquido real.
- O diagnóstico deve permanecer ativo e acessível na barra de navegação para uso contínuo de auditoria.

## 9. Pendências e próximos passos
**Nenhuma pendência identificada.**

## 10. Observações para continuidade
- O endpoint `/Home/Diagnostico` pode ser acessado diretamente no navegador ou via menu superior para auditoria a qualquer momento.
