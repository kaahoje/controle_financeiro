# Adição de Flag ConsiderarNoDiagnostico na Entidade Conta

## 1. Objetivo
Permitir que contas de empréstimo ou controle de dívidas sejam desconsideradas dos cálculos de diagnóstico financeiro e saldos acumulados reais.

## 2. Solicitação original
"A conta 'Empréstimo' é só para controle de dívidas e não deve ser levada em conta no saldo acumulado do mês já que não estou quitando a dívida no momento. Então crie uma propriedade 'Considerar no diagnóstico' na GestorContas.Web/Models/Conta.cs e ignore ela na tela de diagnóstico."

## 3. Contexto considerado
- `GestorContas.Web/Models/Conta.cs`
- `GestorContas.Web/Services/Diagnostico/DiagnosticoFinanceiroService.cs`
- `GestorContas.Web/Program.cs`
- Views de Cadastro/Edição de Contas (`Create.cshtml`, `Edit.cshtml`, `Index.cshtml`)

## 4. Fontes utilizadas
- `GestorContas.Web/Models/Conta.cs`
- `GestorContas.Web/Services/Diagnostico/DiagnosticoFinanceiroService.cs`

## 5. Análise realizada
- Foi adicionada a propriedade booleana `ConsiderarNoDiagnostico` na entidade `Conta.cs` com padrão `true`.
- No `DiagnosticoFinanceiroService.cs`, a consulta de contas e lançamentos passou a filtrar estritamente por `.Where(c => c.ConsiderarNoDiagnostico)`.
- No `Program.cs`, adicionada migração automática na inicialização da aplicação para executar `ALTER TABLE Contas ADD COLUMN ConsiderarNoDiagnostico INTEGER NOT NULL DEFAULT 1;` caso a coluna não exista no banco SQLite existente.
- Na interface de gerenciamento de contas (`Create.cshtml`, `Edit.cshtml`), foi adicionado o interruptor (switch) para habilitar/desabilitar a propriedade.
- Na listagem de contas (`Index.cshtml`), foi inserida a coluna indicadora do status no diagnóstico.

## 6. Decisões tomadas
- Padrão `true` para todas as contas existentes e novas, exigindo alteração manual apenas para contas específicas (como "Empréstimo").
- Inclusão automática de coluna no SQLite na inicialização para não exigir migrações manuais por comando.

## 7. Resultado produzido
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Models\Conta.cs`
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Program.cs`
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Services\Diagnostico\DiagnosticoFinanceiroService.cs`
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Controllers\ContasController.cs`
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Views\Contas\Create.cshtml`
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Views\Contas\Edit.cshtml`
- `c:\Projetos\Controle Financeiro\GestorContas.Web\Views\Contas\Index.cshtml`

## 8. Regras e critérios consolidados
- Lançamentos vinculados a contas com `ConsiderarNoDiagnostico == false` são ignorados nas análises do módulo de diagnóstico.

## 9. Pendências e próximos passos
**Nenhuma pendência identificada.**

## 10. Observações para continuidade
- Ao editar a conta "Empréstimo" na tela de Contas, desmarque a opção "Considerar no Diagnóstico" para que ela seja totalmente excluída dos cálculos da auditoria.
