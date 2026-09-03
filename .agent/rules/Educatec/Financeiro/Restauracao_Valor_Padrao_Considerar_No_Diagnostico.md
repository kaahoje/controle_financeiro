# Restauração do Valor Padrão de ConsiderarNoDiagnostico para True

## 1. Objetivo
Restaurar o valor padrão da propriedade `ConsiderarNoDiagnostico` para `true` no modelo `Conta` e nos formulários do sistema.

## 2. Solicitação original
"Vi que você implementou de forma inversa o que pedi. Então o padrão é 'true' mesmo"

## 3. Contexto considerado
- `GestorContas.Web/Models/Conta.cs`
- `GestorContas.Web/Program.cs`
- `GestorContas.Web/Views/Contas/Create.cshtml`

## 4. Fontes utilizadas
- `GestorContas.Web/Models/Conta.cs`

## 5. Análise realizada
- Revertida a propriedade `ConsiderarNoDiagnostico` em `Conta.cs` para `= true`.
- Revertido o script de migração no `Program.cs` para `DEFAULT 1`.
- Revertido o formulário de cadastro em `Create.cshtml` ativando a marcação `checked` por padrão.

## 6. Decisões tomadas
- Por padrão, todas as contas são incluídas no diagnóstico. Apenas contas marcadas como exceção (como "Empréstimo") serão ignoradas ao desmarcar a opção.

## 7. Resultado produzido
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Models\Conta.cs`
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Program.cs`
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Views\Contas\Create.cshtml`

## 8. Regras e critérios consolidados
- Padrão `true` para a propriedade `ConsiderarNoDiagnostico`.

## 9. Pendências e próximos passos
**Nenhuma pendência identificada.**

## 10. Observações para continuidade
- Para ignorar uma conta específica do diagnóstico, basta ir na edição de contas e desmarcar a opção "Considerar no Diagnóstico".
