# Especificação Técnica: Esquema de Filtros e Resumo do Grid de Lançamentos

> **Nota:** Este documento descreve detalhadamente o funcionamento técnico, regras de negócio, cálculos financeiros e comportamentos do frontend/backend referentes ao **Esquema de Filtros e Grid de Lançamentos** do sistema `GestorContas.Web`. Sirva este arquivo como diretriz completa para implementação, refatoração ou auditoria por modelos de IA e desenvolvedores.

---

## 1. Visão Geral do Módulo

O grid principal da tela de Lançamentos (`/Lancamentos/Index`) oferece uma visão financeira filtrável e agrupável de todas as receitas e despesas cadastradas. Ele possui:

1. **Formulário Dinâmico de Filtros**: Permite combinar múltiplos critérios (Período, Categoria, Tipo, Conta, Transferências e Agrupamento por Data).
2. **Atualização Reativa sem Reload (AJAX)**: Atualização automática do grid ao alterar qualquer opção de filtro sem recarregar a página.
3. **Cálculo Inteligente de Totais**: Resumo financeiro (Entradas, Saídas e Saldo) que desconsidera transferências entre contas internas para não inflar os totais.
4. **Detecção e Realce de Duplicados**: Identificação automática de lançamentos suspeitos de duplicação no mesmo dia para a mesma conta.

---

## 2. Parâmetros de Filtro e Assinatura da Action

### 2.1 Assinatura no Controller (`LancamentosController.cs`)
```csharp
public async Task<IActionResult> Index(
    int? mes, 
    int? ano, 
    int? categoriaId, 
    int? contaId, 
    TipoLancamento? tipo, 
    bool? paraTransferencia, 
    bool agruparPorData = false, 
    bool todos = false)
```

### 2.2 Especificação dos Campos de Filtro

| Parâmetro | Tipo | Opções / Valores | Valor Padrão (Default) | Comportamento no SQL (EF Core) |
| :--- | :--- | :--- | :--- | :--- |
| `mes` | `int?` | `0` (Todos), `1` a `12` (Mês) | Mês Atual (`DateTime.Today.Month`) | Se `mes != 0`: `Where(l => l.Data.Month == mes)` |
| `ano` | `int?` | `0` (Todos), Anos (ex: 2021..2031) | Ano Atual (`DateTime.Today.Year`) | Se `ano != 0`: `Where(l => l.Data.Year == ano)` |
| `categoriaId` | `int?` | `null`/`""` (Todas) ou ID da Categoria | `null` | Se informado: `Where(l => l.CategoriaId == categoriaId)` |
| `contaId` | `int?` | `null`/`""` (Todas) ou ID da Conta | `null` | Se informado: `Where(l => l.ContaId == contaId)` |
| `tipo` | `TipoLancamento?` | `null` (Todos), `1` (Entrada), `2` (Saída) | `null` | Se informado: `Where(l => l.Tipo == tipo)` |
| `paraTransferencia`| `bool?` | `null` (Todas), `true` (Transferências), `false` (Receitas/Despesas) | `null` | Se informado: `Where(l => l.Categoria.ParaTransferencia == paraTransferencia)` |
| `agruparPorData` | `bool` | `true` (Agrupar), `false` (Não agrupar) | `false` | Altera a renderização HTML do grid inserindo linhas de cabeçalho por data (`group-header`) |

---

## 3. Regras de Negócio e Cálculos Financeiros

### 3.1 Filtro Temporal Combinado (Mês / Ano)
- **Mês e Ano selecionados** (`mes != 0 && ano != 0`): Retorna apenas registros pertencentes ao mês e ano especificados.
- **Apenas Mês selecionado** (`mes != 0 && ano == 0`): Retorna registros do mês especificado em qualquer ano.
- **Apenas Ano selecionado** (`mes == 0 && ano != 0`): Retorna todos os registros do ano especificado de janeiro a dezembro.
- **Mês e Ano desmarcados** (`mes == 0 && ano == 0` ou flag `todos = true`): Remove o filtro de datas, retornando todo o histórico do banco.

---

### 3.2 Regra de Exclusão de Transferências Internas nos Totais

Para refletir com precisão a saúde financeira (receitas e despesas reais), lançamentos cujas categorias sejam marcadas como **Transferência Interna** (`Categoria.ParaTransferencia == true`) **NÃO** são computados nos somatórios exibidos nos Cards de Resumo.

```csharp
// Cálculo dos Cards no Controller
ViewBag.TotalEntradas = lancamentos
    .Where(x => x.Categoria?.ParaTransferencia == false)
    .Where(l => l.Tipo == TipoLancamento.Entrada)
    .Sum(l => l.Valor);

ViewBag.TotalSaidas = lancamentos
    .Where(x => x.Categoria?.ParaTransferencia == false)
    .Where(l => l.Tipo == TipoLancamento.Saida)
    .Sum(l => l.Valor);

ViewBag.Saldo = ViewBag.TotalEntradas - ViewBag.TotalSaidas;
```

---

### 3.3 Regra de Detecção de Lançamentos Duplicados no Grid

No momento da renderização da View (`Index.cshtml`), os lançamentos filtrados são submetidos a um agrupamento em memória para detectar duplicações potenciais:

- **Critério de Duplicidade**: Dois ou mais lançamentos que compartilhem exatamente a mesma **Data (`Data.Date`)**, mesmo **Valor (`Valor`)**, mesmo **Tipo (`Tipo`)** e mesma **Conta (`ContaId`)**.
- **Identificação**:
  ```csharp
  var lancamentosDuplicados = Model
      .GroupBy(x => new { x.Data.Date, x.Valor, x.Tipo, x.ContaId })
      .Where(g => g.Count() > 1)
      .SelectMany(g => g)
      .Select(x => x.Id)
      .ToHashSet();
  ```
- **Efeito Visual no Grid**:
  - A linha do lançamento recebe a classe CSS `table-warning` (fundo amarelo destacado).
  - Um badge é exibido ao lado da descrição: `<span class="badge bg-warning text-dark"><i class="bi bi-exclamation-triangle-fill"></i> Duplicado</span>`.

---

## 4. Arquitetura Frontend e Comportamento AJAX

### 4.1 Estrutura de Contêineres HTML

```html
<form id="formCadastro" method="get" asp-action="Index">
    <!-- Selects de Mês, Ano, Categoria, Tipo, Conta, ParaTransferencia e Switch agruparPorData -->
</form>

<div id="indexFiltrada">
    <!-- Cards de Resumo Financeiro (Total Entradas, Total Saídas, Saldo) -->
    <!-- Tabela / Grid de Lançamentos -->
</div>
```

---

### 4.2 Script de Filtragem Reativa (Auto-Submit)

O script jQuery monitora eventos `change` em todos os elementos de controle do formulário `#formCadastro`. Ao alterar qualquer valor, o formulário dispara a requisição AJAX sem necessidade de o usuário clicar no botão "Filtrar".

```javascript
function Filtrar() {
    const form = $('#formCadastro');
    const url = form.attr('action') || window.location.pathname;
    const formData = form.serialize();

    $.get(url + '?' + formData, function (data) {
        // Substituição cirúrgica do elemento contêiner #indexFiltrada
        $('#indexFiltrada').html($(data).find('#indexFiltrada').html());
    }).fail(function () {
        console.error('Erro ao filtrar lançamentos');
    });
}

$(function() {
    // Evento change automático para selects e checkboxes do filtro
    $('#formCadastro select, #formCadastro input[type="checkbox"]').on('change', function() {
        Filtrar();
    });

    // Atalhos globais de teclado (Tecla '+' para Novo Lançamento)
    $(document).on('keydown', function(e) {
        if ($(e.target).is('input, select, textarea')) return;
        
        if (e.key === '+' || e.code === 'NumpadAdd') {
            e.preventDefault();
            $('.btn-modal[title="Novo Lançamento"]').first().click();
        }
    });
});
```

---

## 5. Visualização Agrupada por Data (`agruparPorData`)

Quando a opção **"Agrupar por Data"** está ativa (`agruparPorData = true`):

1. O grid intercala linhas especiais de cabeçalho (`<tr class="table-light group-header">`) para cada nova data encontrada na ordenação.
2. É exibido o texto da data por extenso (ex: `15 de Agosto de 2026`).
3. É exibido um badge informando a quantidade de lançamentos daquele dia (ex: `3 lançamentos`).
4. Na coluna de Data/ID, em vez de repetir a data formatada, exibe-se apenas a tag com o ID do registro (`#1042`).

---

## 6. Diretrizes Técnicas para Agentes e Modelos de IA

1. **Manutenção do Parâmetro de Transferência**: Qualquer novo filtro adicionado ao grid deve preservar a exclusão de `ParaTransferencia == true` nos cálculos de `TotalEntradas`, `TotalSaidas` e `Saldo`.
2. **Atualização Parcial via AJAX**: Ao implementar novas ações de inclusão, edição ou exclusão via Modal, a função JavaScript `Filtrar()` deve ser invocada após o término bem-sucedido para atualizar o grid sem recarregar a página inteira.
3. **Preservação de IDs de DOM**: O formulário deve manter a ID `#formCadastro` e o contêiner de resultados a ID `#indexFiltrada`, garantindo o funcionamento do seletor jQuery da função `Filtrar()`.
