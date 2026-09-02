---
trigger: always_on
description: Executar quando for solicitada a finalização da tarefa com o objetivo de resumir o que foi feito.
---

## É obrigatório que os arquivos de contexto criados por esse roteiro sejam definidos como "Model Decision".
# CONTEXTO — DOCUMENTAÇÃO DE ATIVIDADES PARA CONTINUIDADE POR IA

## 1. Objetivo

Este arquivo define o padrão que um modelo de IA deve utilizar para **documentar o que foi realizado em uma atividade**, de forma que outro modelo de IA — ou o próprio modelo no futuro — consiga compreender rapidamente:

- o que foi solicitado;
- o que foi analisado;
- o que foi decidido;
- o que foi alterado;
- quais requisitos foram criados ou modificados;
- quais itens foram descartados e por quê;
- quais regras de negócio foram consideradas;
- quais arquivos ou fontes serviram de base;
- quais pontos permanecem pendentes;
- como continuar o trabalho sem repetir análises já realizadas.

O objetivo não é produzir um relatório narrativo longo. O objetivo é criar uma **memória técnica e operacional da atividade**, suficientemente precisa para permitir continuidade futura.

---

## 2. Princípio fundamental

A documentação deve registrar **o trabalho efetivamente realizado**, e não apenas repetir a descrição original da tarefa.

Deve distinguir claramente:

- **Solicitado:** o que foi pedido;
- **Analisado:** o que foi efetivamente avaliado;
- **Decidido:** conclusões tomadas durante a atividade;
- **Produzido:** resultado efetivamente gerado;
- **Descartado:** conteúdo analisado, mas não incluído;
- **Pendente:** o que ainda precisa ser feito.

Não inventar decisões, alterações ou validações que não ocorreram.

---

## 3. Estrutura obrigatória

Cada atividade documentada deve seguir esta estrutura:

# [Título da atividade]

## 1. Objetivo

Descrever em poucas linhas o objetivo da atividade e o resultado esperado.

## 2. Solicitação original

Registrar de forma resumida o que foi solicitado.

Não alterar o sentido da solicitação.

## 3. Contexto considerado

Registrar as informações necessárias para compreender a atividade, incluindo, quando aplicável:

- módulo;
- funcionalidade;
- escopo;
- documentos utilizados;
- requisitos existentes;
- contexto de negócio;
- restrições.

## 4. Fontes utilizadas

Listar os arquivos, documentos, requisitos, descrições ou outras fontes efetivamente utilizadas.

Exemplo:

- `Requisitos.csv` — requisitos já existentes;
- `Contexto.md` — descrição das funcionalidades;
- descrição fornecida pelo solicitante;
- resultado de análise anterior.

Não afirmar que uma fonte foi consultada se isso não ocorreu.

## 5. Análise realizada

Descrever de forma objetiva o raciocínio funcional realizado.

Registrar, quando aplicável:

- funcionalidades identificadas;
- regras de negócio;
- campos;
- configurações;
- permissões;
- bloqueios;
- integrações;
- dependências;
- critérios de validação;
- comparação com requisitos existentes;
- duplicidades encontradas.

Esta seção deve permitir que uma IA futura entenda **como a conclusão foi obtida**, sem precisar refazer toda a análise.

## 6. Decisões tomadas

Registrar as decisões relevantes.

Exemplos:

- requisito considerado novo;
- requisito considerado já contemplado;
- requisito existente ampliado;
- funcionalidade dividida em mais de um requisito;
- detalhe considerado apenas consequência de requisito existente;
- tecnologia específica removida da redação por se tratar de edital;
- regra de bloqueio transformada em requisito independente.

Sempre explicar brevemente o motivo quando ele for importante para continuidade.

## 7. Resultado produzido

Registrar exatamente o que foi produzido.

Para requisitos, usar:

### Requisitos criados

> O sistema deverá ...

### Requisitos alterados

**Nova redação:**
> ...

### Requisitos descartados por duplicidade

- [descrição resumida] — já contemplado por [requisito/tema existente].

Se não houver uma dessas categorias, informar explicitamente que não houve.

## 8. Regras e critérios consolidados

Registrar as regras que devem ser preservadas em trabalhos futuros.

Exemplos:

- não repetir requisitos já existentes;
- comparar semanticamente, e não apenas por texto;
- explicitar campos quando conhecidos;
- não inventar configurações;
- não especificar tecnologia de plataforma no edital;
- separar regras de bloqueio relevantes em requisitos próprios.

Esta seção é especialmente importante para continuidade por outra IA.

## 9. Pendências e próximos passos

Listar apenas aquilo que realmente ficou pendente.

Exemplo:

- validar se determinado requisito já existe;
- revisar redação;
- analisar funcionalidade complementar.

Se não houver pendências:

**Nenhuma pendência identificada.**

## 10. Observações para continuidade

Registrar informações que evitem retrabalho.

Exemplos:

- “A comparação de duplicidade deve ser feita contra o `Requisitos.csv`, e não somente contra o `Contexto.md`.”
- “Os requisitos do módulo de Simulados devem ser comparados prioritariamente com outros requisitos do mesmo escopo.”
- “A regra de bloqueio de edição foi considerada funcionalidade independente.”

---

## 4. Como resumir uma atividade

O resumo deve ser **compacto, mas semanticamente completo**.

Não registrar cada mensagem da conversa.

Registrar somente informações que tenham valor para continuidade.

### Não fazer

> “Conversamos sobre os requisitos e fizemos alguns ajustes.”

Isso não permite continuidade.

### Fazer

> “Foram analisadas as funcionalidades de geração de questões por IA. Após comparação semântica com os requisitos existentes, foram considerados inéditos apenas os recursos de seleção da matriz de referência, configuração individual de descritor/dificuldade/imagem na geração em lote, balanceamento automático e reprocessamento automático em caso de falha.”

Isso permite que outra IA continue o trabalho.

---

## 5. Preservação dos resultados

Quando a atividade gerar requisitos, preservar a **redação final**, e não apenas uma descrição do que foi feito.

A IA futura deve conseguir recuperar:

1. qual requisito foi criado;
2. qual redação foi aprovada;
3. se ele substituiu outro;
4. por que foi criado;
5. qual funcionalidade originou o requisito.

Quando houver muitos requisitos, organizá-los em lista.

---

## 6. Registro de duplicidades

Quando houver comparação com requisitos existentes, registrar o resultado da comparação.

Usar uma classificação:

### Novo
Funcionalidade não contemplada anteriormente.

### Já contemplado
Funcionalidade já coberta por requisito existente.

### Parcialmente contemplado
Existe cobertura parcial e foi necessário complementar ou criar requisito específico.

### Detalhamento
Informação considerada apenas detalhamento de funcionalidade já prevista, sem necessidade de novo requisito.

Isso é importante porque impede que uma IA futura volte a propor como novidade algo que já foi analisado.

---

## 7. Registro de alterações

Sempre que um requisito for melhorado, documentar:

- alterações realizadas.

Exemplo:

**Motivo:** o termo “permissões” era considerado genérico.

**Alteração:** foram explicitadas as permissões de visualização, inclusão, edição, exclusão, publicação e sincronização, além das regras de bloqueio.

---

## 8. Regras para documentação de atividades de requisitos

Quando a atividade estiver relacionada à elaboração de requisitos para edital:

- preservar linguagem formal;
- registrar as regras de negócio relevantes;
- registrar campos e configurações explicitamente identificados;
- registrar regras de bloqueio;
- registrar permissões;
- registrar critérios de duplicidade;
- registrar limitações do edital quanto à especificação de tecnologias;
- registrar se o requisito foi considerado novo ou já contemplado;
- preservar a redação final dos requisitos.

### Importante

Não transformar detalhes de implementação em requisitos quando eles não representam uma capacidade funcional ou técnica necessária.

Exemplo:

Implementação:
`ExportUnidadeEscolarStrategy`

Documentação:
“Foi analisado o recurso de exportação de unidades escolares por serviço de integração.”

---

## 9. Continuidade entre IAs

A documentação deve permitir que uma IA futura comece o trabalho sem precisar reconstruir todo o histórico da conversa.

Para isso, sempre responder às perguntas:

- O que estava sendo feito?
- Qual era o escopo?
- Quais fontes foram utilizadas?
- O que foi considerado novo?
- O que já existia?
- O que foi alterado?
- Qual redação ficou definida?
- Quais decisões devem ser preservadas?
- O que ainda precisa ser feito?

Se alguma dessas informações não estiver disponível, não inventar. Registrar:

**Informação não disponível na atividade documentada.**

---

## 10. Controle de versão e estado

- não haverá controle de versão


## 11. Formato recomendado para atividades concluídas

Use o seguinte modelo:

```markdown
# [Título]

## 1. Objetivo
[Objetivo]

## 2. Solicitação original
[Solicitação]

## 3. Contexto considerado
[Contexto]

## 4. Fontes utilizadas
- [Fonte]
- [Fonte]

## 5. Análise realizada
[Resumo objetivo da análise]

## 6. Decisões tomadas
- [Decisão]
- [Decisão]

## 7. Resultado produzido

### Requisitos criados
> [Requisito]

### Requisitos alterados
**Original:**
> [Original]

**Nova redação:**
> [Nova redação]

### Itens considerados já contemplados
- [Item] — [motivo]

## 8. Regras e critérios consolidados
- [Regra]
- [Regra]

## 9. Pendências e próximos passos
- [Pendência]

## 10. Observações para continuidade
- [Informação importante]
```

---

## 12. Regra final

O documento de atividade deve funcionar como uma **memória técnica de trabalho**.

Uma IA que receba apenas este arquivo deve conseguir entender:

> **o que foi feito, por que foi feito, qual foi o resultado, quais decisões já foram tomadas e o que não deve ser refeito.**

A documentação deve ser curta o suficiente para ser reutilizável, mas detalhada o suficiente para impedir perda de contexto.

**Não registrar conversas desnecessárias. Registrar decisões, resultados, critérios e informações necessárias para continuidade.**
**Executar os passos descritos em "finalizacao_de_tarefas.md" **
**Grave o conteúdo em um arquivo .md com dentro de .agent/rules/Educatec/[Área da aplicação]/[Tarefa número (se informado)]_[Breve descrição].md.**
