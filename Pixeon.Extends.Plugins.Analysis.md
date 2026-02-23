# Análise Funcional dos Plugins do Projeto Pixeon.Extends

Este documento apresenta uma análise funcional detalhada de todos os plugins implementados no projeto **Pixeon.Extends**. Para cada plugin, são descritas as ações realizadas, os eventos em que são executados, validações implementadas, regras de negócio aplicadas, efeitos colaterais e outras informações técnicas relevantes.

---

## Índice

1. [Despesas (msdyn_expense)](#1-despesas-msdyn_expense)
2. [Entrada de Hora (msdyn_timeentry)](#2-entrada-de-hora-msdyn_timeentry)
3. [Integração (smt_integration)](#3-integração-smt_integration)
4. [Issue (smt_issue)](#4-issue-smt_issue)
5. [Linha da Cotação (QuoteDetail)](#5-linha-da-cotação-quotedetail)
6. [Linha do Contrato (SalesOrderDetail)](#6-linha-do-contrato-salesorderdetail)
7. [Pasta SharePoint (SharePointDocumentLocation)](#7-pasta-sharepoint-sharepointdocumentlocation)
8. [Projeto (msdyn_project)](#8-projeto-msdyn_project)
9. [Cotação (Quote)](#9-cotação-quote)
10. [Contrato de Projeto (SalesOrder)](#10-contrato-de-projeto-salesorder)
11. [Tarefa do Projeto (msdyn_projecttask)](#11-tarefa-do-projeto-msdyn_projecttask)
12. [Reserva de Recurso (BookableResourceBooking)](#12-reserva-de-recurso-bookableresourcebooking)
13. [Aprovação de Projeto (msdyn_projectapproval)](#13-aprovação-de-projeto-msdyn_projectapproval)
14. [Equipe do Projeto (msdyn_projectteam)](#14-equipe-do-projeto-msdyn_projectteam)
15. [Parâmetro de Imposto (smt_imposto_parameter)](#15-parâmetro-de-imposto-smt_imposto_parameter)
16. [Histórico de Tarefa (smt_task_history)](#16-histórico-de-tarefa-smt_task_history)

---

## 1. Despesas (msdyn_expense)

### 1.1 PostUpdateAsync_msdyn_expense

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `msdyn_expense` |
| **Evento** | Post-Operation, Update (Assíncrono) |
| **Arquivo** | `Plugins/Despesas/PostUpdateAsync_msdyn_expense.cs` |

#### Ações Realizadas
- Atualiza o tipo de faturamento (Billing Type) na aprovação do projeto associada à despesa.

#### Condições de Execução
- O plugin é executado somente quando o status da despesa (`msdyn_ExpenseStatus`) é igual a `192350001` (Enviado para aprovação).

#### Regras de Negócio
1. Quando uma despesa é atualizada e seu status muda para "Enviado para aprovação":
   - O sistema recupera a aprovação do projeto relacionada à despesa.
   - Se a aprovação existir, atualiza seus dados com base nas informações da despesa (PreImage).

#### Efeitos Colaterais
- Atualização do registro `msdyn_projectapproval` associado à despesa.

---

## 2. Entrada de Hora (msdyn_timeentry)

### 2.1 PreCreateSync_msdyn_timeentry

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `msdyn_timeentry` |
| **Evento** | Pre-Operation, Create (Síncrono) |
| **Arquivo** | `Plugins/Entrada de Hora/PreCreateSync_msdyn_timeentry.cs` |

#### Ações Realizadas
- Verifica se a tarefa do projeto associada é uma tarefa pai.
- Valida se o recurso reservável pode criar uma entrada de hora para uma determinada tarefa de projeto.

#### Validações Implementadas
1. **Verificação de Tarefa Pai**: Se a entrada de hora estiver associada a uma tarefa de projeto, verifica se essa tarefa é pai (não permite entrada de hora em tarefas pai).

2. **Restrição de Finalização de Tarefa com Marco**:
   - Se a porcentagem da tarefa (`smt_dc_task_percentage`) for 100%, o sistema verifica se o usuário é o gerente do projeto ou gerente temporário.
   - Se o usuário não for gerente e a tarefa contiver marcos (`smt_lp_milestone`), uma exceção é lançada impedindo a criação.

#### Regras de Negócio
- Apenas gerentes de projeto ou gerentes temporários podem finalizar tarefas que contêm marcos.
- Não é permitido criar entradas de hora diretamente em tarefas pai.

#### Mensagens de Erro
- "Você não pode finalizar uma tarefa que contém marcos, por favor contate seu gerente."

---

### 2.2 PreUpdateSync_msdyn_timeentry

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `msdyn_timeentry` |
| **Evento** | Pre-Operation, Update (Síncrono) |
| **Arquivo** | `Plugins/Entrada de Hora/PreUpdateSync_msdyn_timeentry.cs` |

#### Ações Realizadas
- Verifica se a tarefa do projeto associada é uma tarefa pai.

#### Validações Implementadas
- Se a entrada de hora possuir uma tarefa de projeto associada, valida se essa tarefa é uma tarefa pai (não permite atualização se for tarefa pai).

---

### 2.3 PostUpdateAsync

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `msdyn_timeentry` |
| **Evento** | Post-Operation, Update (Assíncrono) |
| **Arquivo** | `Plugins/Entrada de Hora/PostUpdateAsync.cs` |

#### Ações Realizadas
1. **Mapeamento de Aprovação**: Atualiza a aprovação do projeto com os dados da entrada de hora quando aprovada.
2. **Atualização do Projeto**: Atualiza campos do projeto relacionados à entrada de hora aprovada.
3. **Tratamento de Recuperação**: Atualiza campos do projeto quando uma entrada de hora é devolvida após solicitação de recuperação.

#### Condições de Execução
- **SetMapping e SetProjectFields**: Executados quando o status da entrada de hora é "Aprovado" e o tipo é "Trabalho".
- **SetProjectFieldsRecuperado**: Executado quando o status anterior era "Recuperação Solicitada" e o novo status é "Devolvido", com tipo "Trabalho".

#### Efeitos Colaterais
- Atualização do registro `msdyn_projectapproval` relacionado.
- Atualização de campos no registro `msdyn_project`.

---

## 3. Integração (smt_integration)

### 3.1 PostCreateAsync_smt_integration

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `smt_integration` |
| **Evento** | Post-Operation, Create (Assíncrono) |
| **Arquivo** | `Plugins/Integracao/PostCreateAsync_smt_integration.cs` |

#### Ações Realizadas
Este plugin processa registros de integração para atualizar campos de múltiplas escolhas (OptionSetValueCollection) e picklists em diferentes entidades.

#### Entidades Processadas

**1. Account (Conta)**
- **Canais Ativos** (`smt_mc_active_channels`): P01-FLOW, P01-PACS, P02-FLOW, P02-PACS, etc.
- **Módulo PACS** (`smt_pl_module_pacs`): AuroraMaster, Distribuição Interna, WebPortal, MedReport, Worklist, VirtualWorklist, Fluens, Lumine, LTA.
- **ClickVita** (`smt_pl_clickvita`): Distribuição de Imagens e Laudos, Distribuição de Laudos.
- **Módulo SMART** (`smt_pl_module_smart`): 25 módulos incluindo Agendamento, Atendimento ao Paciente, Centro Cirúrgico, Internação Hospitalar, etc.
- **Módulo XCLINIC** (`smt_pl_module_xclinic`): 20 módulos incluindo Worklist, Integração PACS, LIS Interfaceamento, etc.
- **Moment XCLINIC** (`smt_pl_moment_xclinic`): Controle, Gestão, Produtividade, Relacionamento.

**2. Quote (Cotação)**
- **Coordenação Responsável** (`smt_pl_responsible_coordination`): CONNE, SUDESTE, SUL, SMART, FLOW, Pleres, Communis, Serviços Especiais, Customizações.
- **Produtos** (`smt_pl_products`): 16 opções incluindo PACS, RIS, LIS, Clickvita, LabLink, Flow Performance, etc.
- **Pendências para Fechamento** (`smt_pl_pending_closing`): 67 tipos de pendências categorizadas de [A] a [E].

**3. Contact (Contato)**
- Atualiza picklists genéricos com prefixo `smt_pl_`.

#### Regras de Negócio
1. Identifica a entidade alvo através do campo `smt_st_entitylogicalname`.
2. Processa o valor JSON do campo `smt_st_jsonbody` (valores separados por vírgula).
3. Converte valores de texto em códigos de OptionSetValue correspondentes.
4. Para campos não mapeados explicitamente, tenta resolver o valor através do label no metadata.

#### Tratamento de Erros
- Em caso de falha na conversão ou atualização, cria um registro de log com detalhes do erro.

#### Efeitos Colaterais
- Atualização de registros nas entidades Account, Quote e Contact.
- Criação de registros de log em caso de erros.

---

## 4. Issue (smt_issue)

### 4.1 PostUpdateSync

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `smt_issue` |
| **Evento** | Post-Operation, Update (Síncrono) |
| **Arquivo** | `Plugins/Issue/PostUpdateSync.cs` |

#### Ações Realizadas
- Gera o corpo da requisição JSON para integração com o JIRA.

#### Condições de Execução
- O plugin é executado apenas se o campo `smt_bt_existingissue` no PreImage for diferente de `true` (ou seja, não é uma issue existente no JIRA).

#### Regras de Negócio
- Para issues novas (não existentes no JIRA), gera o JSON de integração com base nos dados do target e postImage.

#### Efeitos Colaterais
- Geração de dados JSON para sincronização com sistema externo (JIRA).

---

## 5. Linha da Cotação (QuoteDetail)

### 5.1 PreCreateSync_QuoteDetail

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `QuoteDetail` |
| **Evento** | Pre-Operation, Create (Síncrono) |
| **Arquivo** | `Plugins/Linha da Cotação/PreCreateSync_QuoteDetail.cs` |

#### Ações Realizadas
- Plugin preparado para atualizar receitas na cotação (método comentado/desabilitado).

#### Funcionalidade Desabilitada
- `UpdateRevenuesQuote`: Calcularia totais de receita recorrente e eventual baseado no tipo de produto (1 = recorrente, 5 = eventual).

---

### 5.2 PreUpdateSync_QuoteDetail

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `QuoteDetail` |
| **Evento** | Pre-Operation, Update (Síncrono) |
| **Arquivo** | `Plugins/Linha da Cotação/PreUpdateSync_QuoteDetail.cs` |

#### Ações Realizadas
- Plugin preparado para atualizar receitas na cotação durante atualizações (método comentado/desabilitado).

#### Funcionalidade Desabilitada
- `UpdateRevenuesQuote`: Recalcularia totais de receita considerando as alterações na linha de produto.

---

## 6. Linha do Contrato (SalesOrderDetail)

### 6.1 PreCreateSync

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `SalesOrderDetail` |
| **Evento** | Pre-Operation, Create (Síncrono) |
| **Arquivo** | `Plugins/Linha do Contrato/PreCreateSync.cs` |

#### Ações Realizadas
- Plugin preparado para atualizar receitas no contrato (método comentado/desabilitado).

#### Funcionalidade Desabilitada
- `UpdateRevenuesQuote`: Calcularia totais de receita recorrente e eventual no SalesOrder baseado no tipo de produto.

---

### 6.2 PreUpdateSync

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `SalesOrderDetail` |
| **Evento** | Pre-Operation, Update (Síncrono) |
| **Arquivo** | `Plugins/Linha do Contrato/PreUpdateSync.cs` |

#### Ações Realizadas
- Plugin preparado para atualizar receitas no contrato durante atualizações (método comentado/desabilitado).

---

### 6.3 PostUpdateAsync_SalesOrderDetail

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `SalesOrderDetail` |
| **Evento** | Post-Operation, Update (Assíncrono) |
| **Arquivo** | `Plugins/Linha do Contrato/PostUpdateAsync_SalesOrderDetail.cs` |

#### Ações Realizadas
- Atualiza a receita estimada nas tarefas do projeto associadas.

#### Condições de Execução
- Executado quando o `PricePerUnit` (preço unitário) ou `msdyn_Project` (projeto) forem alterados.

#### Efeitos Colaterais
- Atualização de campos de receita estimada nas tarefas do projeto relacionadas à linha do contrato.

---

## 7. Pasta SharePoint (SharePointDocumentLocation)

### 7.1 PreValidationSync_sharepointdocumentarion

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `SharePointDocumentLocation` |
| **Evento** | Pre-Validation, Create (Síncrono) |
| **Arquivo** | `Plugins/Pasta Sharepoint/PreValidationSync_sharepointdocumentarion.cs` |

#### Ações Realizadas
- Valida a criação de locais de documentos do SharePoint para contas.

#### Validações Implementadas
1. Verifica se o registro relacionado (`RegardingObjectId`) é do tipo "account".
2. Verifica se o nome do documento não contém "Documentos em Site Padrão".

#### Regras de Negócio
- Se o documento está relacionado a uma conta e não é o local padrão, executa validação adicional específica para documentos de conta.

---

## 8. Projeto (msdyn_project)

### 8.1 PreCreateSync

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `msdyn_project` |
| **Evento** | Pre-Operation, Create (Síncrono) |
| **Arquivo** | `Plugins/Projeto/PreCreateSync.cs` |

#### Ações Realizadas
1. **Definição do Nome do Projeto**: Atribui automaticamente o nome do projeto baseado no contrato associado.
2. **Definição da Modalidade de Canal**: Copia a modalidade de canal do contrato para o projeto.

#### Condições de Execução
- `SetProjectName`: Executado quando o projeto não é interno (`smt_bl_projectInternal == false`) e não é um template (`msdyn_istemplate == false`).
- `SetModalidadeCanal`: Executado quando existe um contrato associado (`msdyn_salesorderid != null`).

#### Regras de Negócio
- Projetos externos (não internos e não templates) recebem nome automaticamente baseado no contrato.
- A modalidade de canal é herdada do contrato de projeto.

---

### 8.2 PostCreateSync (CreateProject)

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `msdyn_project` |
| **Evento** | Post-Operation, Create (Síncrono) |
| **Arquivo** | `Plugins/Projeto/PostCreateSync.cs` |

#### Ações Realizadas
- Verifica se existe um contrato associado ao projeto.

#### Validações Implementadas
- Se o projeto não é interno, não possui contrato associado, não é migração e não é template, uma exceção é lançada.

#### Regras de Negócio
- Projetos externos devem obrigatoriamente ter um contrato associado.

#### Mensagens de Erro
- "É necessário que tenha um contrato associado ao projeto."

---

### 8.3 PostCreateAsync

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `msdyn_project` |
| **Evento** | Post-Operation, Create (Assíncrono) |
| **Arquivo** | `Plugins/Projeto/PostCreateAsync.cs` |

#### Ações Realizadas
- Cria uma equipe (Team) para o projeto e a associa como proprietária.

#### Efeitos Colaterais
- Criação de um novo registro de Team no sistema.
- Associação da equipe criada ao projeto.

---

### 8.4 PreUpdateSync

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `msdyn_project` |
| **Evento** | Pre-Operation, Update (Síncrono) |
| **Arquivo** | `Plugins/Projeto/PreUpdateSync.cs` |

#### Ações Realizadas
1. **Validação do Término Real**: Valida o preenchimento do campo Término Real na alteração de status do projeto.
2. **Alteração de Gerente de Projeto**: Atualiza a equipe proprietária quando o gerente de projeto é alterado.
3. **Alteração de Gerente Temporário**: Gerencia permissões quando o gerente temporário é alterado.
4. **Validação de Proprietário**: Impede alteração direta do proprietário do projeto.

#### Validações Implementadas

1. **Restrição de Alteração de Proprietário**:
   - Não permite alterar o proprietário se ambos (atual e novo) são equipes diferentes.
   - Não permite atribuir um usuário (ao invés de equipe) como proprietário.

#### Regras de Negócio
- O proprietário do projeto só pode ser alterado através do campo "Gerente de Projeto".
- Gerentes temporários recebem/perdem acesso automaticamente ao projeto e suas tarefas.

#### Métodos de Gerenciamento de Acesso (implementados mas não chamados diretamente)
- `GrantAcess`: Concede direitos de acesso ao gerente temporário.
- `ModifyAccess`: Revoga direitos de acesso do gerente temporário anterior.
- `ProjectMilestoneSum`: Valida que a soma dos marcos de reconhecimento é 100%.

#### Mensagens de Erro
- "O proprietário do Projeto não pode ser alterado. Caso queira alterar o gerente, por favor alterar o campo 'Gerente de Projeto'"

---

### 8.5 PostUpdateSync

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `msdyn_project` |
| **Evento** | Post-Operation, Update (Síncrono) |
| **Arquivo** | `Plugins/Projeto/PostUpdateSync.cs` |

#### Ações Realizadas
- Realiza o reconhecimento de receita recorrente quando o projeto é concluído.

#### Condições de Execução
- Status do projeto igual a `192350002` (Concluído).
- Projeto possui um contrato associado.
- O tipo de reconhecimento recorrente (`smt_pl_type_rr`) é `180580001` e a data de acesso (`smt_dt_access`) é nula.

#### Efeitos Colaterais
- Processamento dos produtos do contrato para reconhecimento de receita.

---

### 8.6 PostDeleteAsync

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `msdyn_project` |
| **Evento** | Post-Operation, Delete (Assíncrono) |
| **Arquivo** | `Plugins/Projeto/PostDeleteAsync.cs` |

#### Ações Realizadas
- Exclui a equipe proprietária do projeto quando o projeto é excluído.

#### Condições de Execução
- O projeto possui uma equipe proprietária (`OwningTeam != null`).

#### Efeitos Colaterais
- Exclusão do registro de Team associado ao projeto deletado.

---

## 9. Cotação (Quote)

### 9.1 PostCreateSync_Quote

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `Quote` |
| **Evento** | Post-Operation, Create (Síncrono) |
| **Arquivo** | `Plugins/Quote/PostCreateSync_Quote.cs` |

#### Ações Realizadas
1. **Associação de Equipe**: Define o proprietário da cotação baseado em regras de distribuição.
2. **Histórico de Cotações**: Associa cotações que pertencem à mesma oportunidade.
3. **Definição de Datas de E-mail SLA**: Calcula e define datas de envio de e-mails para diferentes níveis gerenciais.

#### Regras de Negócio para Atribuição de Equipe

**1. Gold Partner:**
- **Modalidade Tipo 2**: Direciona para o canal ao qual pertence (baseado no campo "E-mail"/Canal).
- **Modalidade Tipo 1**: Usa regras de distribuição de GPs para encontrar a equipe.

**2. Cotações com Parâmetros Definidos** (Família, Classificação, Integrador):
- Busca equipe baseada nos parâmetros de alocação configurados.

**3. Fallback**:
- Se nenhuma regra se aplicar, atribui à equipe de serviço padrão.

#### Cálculo de Datas de E-mail SLA
- **E-mail Gerente do Proprietário**: 3 dias úteis após criação.
- **E-mail Gerente Vertical**: 4 dias úteis após criação.
- **E-mail Diretor**: 5 dias úteis após criação.

#### Efeitos Colaterais
- Atualização do proprietário da cotação.
- Associação com cotações existentes da mesma oportunidade.
- Definição de campos de data para controle de SLA.

---

### 9.2 PreUpdateSync_quote

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `Quote` |
| **Evento** | Pre-Operation, Update (Síncrono) |
| **Arquivo** | `Plugins/Quote/PreUpdateSync_quote.cs` |

#### Ações Realizadas
1. **Validação de Reprovação**: Valida preenchimento dos campos de reprovação ao fechar cotação como perdida.
2. **Definição de Unidade Organizacional**: Atualiza a unidade organizacional baseada no proprietário.
3. **Fechamento de Cotação**: Fecha automaticamente a cotação quando a fase da oportunidade é "Venda Cancelada".

#### Validações Implementadas

1. **Campos Obrigatórios para Reprovação**:
   - Ao fechar cotação como perdida (StateCode = Closed, StatusCode = 5), os campos de reprovação devem estar preenchidos.

2. **Restrição de Proprietário**:
   - Não permite atribuir cotação a um usuário individual, somente a equipes.

#### Regras de Negócio
- A data de reprovação (`smt_dt_repprove`) é automaticamente definida como a data atual ao fechar como perdida.
- A unidade organizacional é obtida da equipe proprietária quando o proprietário é alterado.

#### Mensagens de Erro
- "Não foi possível fechar a cotação, pois os campos de reprovação não foram preenchidos."
- "Não é possível atribuir cotação para um usuário. Por favor, atribuir a uma equipe do sistema."

---

### 9.3 PostUpdateAsync_quote

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `Quote` |
| **Evento** | Post-Operation, Update (Assíncrono) |
| **Arquivo** | `Plugins/Quote/PostUpdateAsync_quote.cs` |

#### Ações Realizadas
- Plugin preparado para atualização de equipe proprietária e unidade organizacional (métodos comentados/desabilitados).

#### Funcionalidade Desabilitada
- `SetOwnerTeam`: Atualizaria o proprietário baseado na unidade de negócio (Distribuidor).
- `SetOrganizationalUnit`: Atualizaria a unidade organizacional baseada no proprietário.

---

## 10. Contrato de Projeto (SalesOrder)

### 10.1 PreCreateSync_SalesOrder

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `SalesOrder` |
| **Evento** | Pre-Operation, Create (Síncrono) |
| **Arquivo** | `Plugins/SalesOrder/PreCreateSync_SalesOrder.cs` |

#### Ações Realizadas
1. **Verificação de Duplicidade**: Verifica se já existe um contrato com o mesmo número de oportunidade.
2. **Associação de Unidade Organizacional**: Herda a unidade organizacional da cotação.
3. **Definição de Modalidade de Canal**: Copia a modalidade de canal da cotação.

#### Validações Implementadas
- Verifica duplicidade de contrato baseado no número da oportunidade (`smt_st_opp_number`).

#### Regras de Negócio
- A unidade organizacional do contrato é herdada da cotação origem.
- A modalidade de canal é copiada da cotação para o contrato.

---

### 10.2 PreUpdateSync_SalesOrder

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `SalesOrder` |
| **Evento** | Pre-Operation, Update (Síncrono) |
| **Arquivo** | `Plugins/SalesOrder/PreUpdateSync_SalesOrder.cs` |

#### Ações Realizadas
1. **Fechamento de Contrato**: Fecha o contrato quando a fase da oportunidade é "Venda Cancelada".
2. **Atualização de Unidade Organizacional**: Atualiza a unidade organizacional baseada no proprietário.

#### Regras de Negócio para Unidade Organizacional
- Se o proprietário é uma equipe: obtém a unidade organizacional da equipe.
- Se o proprietário é um usuário: obtém a unidade organizacional do usuário.

---

### 10.3 PostUpdateAsync_Salesorder

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `SalesOrder` |
| **Evento** | Post-Operation, Update (Assíncrono) |
| **Arquivo** | `Plugins/SalesOrder/PostUpdateAsync_Salesorder.cs` |

#### Ações Realizadas
1. **Atualização de Receita Eventual**: Atualiza a receita estimada nas tarefas do projeto.
2. **Atualização de Receita Recorrente**: Processa o reconhecimento de receita recorrente.

#### Condições de Execução
- `UpdateEventualRevenue`: Executado quando `smt_mn_value_eventual_final` é alterado.
- `UpdateRecurrenceRevenue`: Executado quando `smt_mn_recognized_rr` é alterado e o reconhecimento recorrente não foi processado (`smt_bt_recognized_recurrence != true`).

#### Efeitos Colaterais
- Atualização de campos de receita nas tarefas do projeto associadas.

---

## 11. Tarefa do Projeto (msdyn_projecttask)

### 11.1 PreCreateSync_projecttask

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `msdyn_projecttask` |
| **Evento** | Pre-Operation, Create (Síncrono) |
| **Arquivo** | `Plugins/Tarefa do Projeto/PreCreateSync_projecttask.cs` |

#### Ações Realizadas
- Associa automaticamente o contrato do projeto à tarefa.

#### Condições de Execução
- A tarefa possui um projeto associado (`msdyn_project != null`).

#### Efeitos Colaterais
- Preenchimento do campo de referência ao contrato (SalesOrder) na tarefa do projeto.

---

### 11.2 PreUpdateSync_projecttask

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `msdyn_projecttask` |
| **Evento** | Pre-Operation, Update (Síncrono) |
| **Arquivo** | `Plugins/Tarefa do Projeto/PreUpdateSync_projecttask.cs` |

#### Ações Realizadas
- Atualiza o esforço total no projeto quando a tarefa raiz (WBSID = 1) é atualizada.

#### Condições de Execução
- O campo `msdyn_Effort` foi alterado e a tarefa é a tarefa raiz (WBSID = "1").

#### Efeitos Colaterais
- Atualização do campo `smt_dc_efforttotal` no projeto com o valor do esforço da tarefa raiz.

#### Funcionalidade Desabilitada
- `RecurrentRevenue`: Validação de reconhecimento de receita recorrente (método comentado).

---

## 12. Reserva de Recurso (BookableResourceBooking)

### 12.1 PreCreateSync_BookableResourceBooking

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `BookableResourceBooking` |
| **Evento** | Pre-Operation, Create (Síncrono) |
| **Arquivo** | `Plugins/msdyn_BookableResourceBooking/PreCreateSync_BookableResourceBooking.cs` |

#### Ações Realizadas
- Plugin preparado para validação de criação de reservas (método comentado/desabilitado).

#### Funcionalidade Desabilitada
- `ValidateIfExistBooking`: Verificaria se já existe uma reserva.

---

### 12.2 PreUpdate_BookableResourceBooking

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `BookableResourceBooking` |
| **Evento** | Pre-Operation, Update (Síncrono) |
| **Arquivo** | `Plugins/msdyn_BookableResourceBooking/PreUpdate_BookableResourceBooking.cs` |

#### Ações Realizadas
- Valida a atualização da reserva de recurso.

#### Condições de Execução
- O status da reserva (`BookingStatus`) foi alterado.

#### Validações Implementadas
- Valida se o usuário atual tem permissão para realizar a atualização do status da reserva.

---

### 12.3 PostUpdateSync_BookableResourceBooking

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `BookableResourceBooking` |
| **Evento** | Post-Operation, Update (Síncrono) |
| **Arquivo** | `Plugins/msdyn_BookableResourceBooking/PostUpdateSync_BookableResourceBooking.cs` |

#### Ações Realizadas
- Valida o cancelamento de reservas após a atualização.

#### Condições de Execução
- O status da reserva (`BookingStatus`) foi alterado.

#### Validações Implementadas
- Valida se o usuário tem permissão para cancelar a reserva através da atualização do status.

---

### 12.4 PostDeleteSync_BookableResourceBooking

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `BookableResourceBooking` |
| **Evento** | Post-Operation, Delete (Síncrono) |
| **Arquivo** | `Plugins/msdyn_BookableResourceBooking/PostDeleteSync_msdyn_BookableResourceBooking.cs` |

#### Ações Realizadas
- Valida a exclusão de reservas de recurso.

#### Validações Implementadas
- Verifica se o usuário que está excluindo tem permissão para realizar a operação.

---

## 13. Aprovação de Projeto (msdyn_projectapproval)

### 13.1 PostCreateAsync_msdyn_projectapproval

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `msdyn_projectapproval` |
| **Evento** | Post-Operation, Create (Assíncrono) |
| **Arquivo** | `Plugins/msdyn_projectapproval/PostCreateAsync_msdyn_projectapproval.cs` |

#### Ações Realizadas
- Atualiza a aprovação do projeto com informações do tipo de faturamento da entrada de hora ou despesa.

#### Condições de Execução
- Se a aprovação está relacionada a uma entrada de hora (`msdyn_TimeEntry != null`).
- Se a aprovação está relacionada a uma despesa (`msdyn_ExpenseEntry != null`).

#### Efeitos Colaterais
- Copia o tipo de faturamento (`smt_pl_billingtypee`) da entrada de hora ou despesa para a aprovação.

---

### 13.2 PreUpdateSync_msdyn_projectapproval

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `msdyn_projectapproval` |
| **Evento** | Pre-Operation, Update (Síncrono) |
| **Arquivo** | `Plugins/msdyn_projectapproval/PreUpdateSync_msdyn_projectapproval.cs` |

#### Ações Realizadas
- Sincroniza alterações no tipo de faturamento da aprovação para a entrada de hora ou despesa relacionada.

#### Condições de Execução
- O tipo de faturamento (`msdyn_BillingType`) foi alterado na aprovação.

#### Efeitos Colaterais
- Atualização do tipo de faturamento na entrada de hora ou despesa relacionada.

---

## 14. Equipe do Projeto (msdyn_projectteam)

### 14.1 PreUpdateSync_msdyn_projectteam

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `msdyn_projectteam` |
| **Evento** | Pre-Operation, Update (Síncrono) |
| **Arquivo** | `Plugins/msdyn_projectteam/PreUpdateSync_msdyn_projectteam.cs` |

#### Ações Realizadas
- Valida alterações no status de aprovador do projeto.

#### Condições de Execução
- O campo `msdyn_ProjectApprover` é alterado para `false`.

#### Validações Implementadas
- Valida se a alteração é permitida baseada nas regras de gerenciamento do projeto.

---

## 15. Parâmetro de Imposto (smt_imposto_parameter)

### 15.1 PreCreateSync_smt_imposto_parameter

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `smt_imposto_parameter` |
| **Evento** | Pre-Operation, Create (Síncrono) |
| **Arquivo** | `Plugins/smt_imposto_parameter/PreCreateSync_smt_imposto_parameter.cs` |

#### Ações Realizadas
- Verifica se já existe um parâmetro de imposto para o ano especificado.

#### Condições de Execução
- O campo de ano (`smt__year`) está preenchido.

#### Validações Implementadas
- Verifica duplicidade de parâmetro de imposto por ano.

---

### 15.2 PreUpdateSync_smt_imposto_parameter

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `smt_imposto_parameter` |
| **Evento** | Pre-Operation, Update (Síncrono) |
| **Arquivo** | `Plugins/smt_imposto_parameter/PreUpdateSync_smt_imposto_parameter.cs` |

#### Ações Realizadas
- Verifica se já existe um parâmetro de imposto para o ano especificado durante atualizações.

#### Condições de Execução
- O campo de ano (`smt__year`) foi alterado.

#### Validações Implementadas
- Verifica duplicidade de parâmetro de imposto por ano.

---

## 16. Histórico de Tarefa (smt_task_history)

> **Nota**: O nome da pasta (`smt_task_history`) difere do nome lógico da entidade (`smt_history_task`).

### 16.1 PostCreateAsync_smt_task_history

| Atributo | Descrição |
|----------|-----------|
| **Entidade** | `smt_history_task` |
| **Evento** | Post-Operation, Create (Assíncrono) |
| **Arquivo** | `Plugins/smt_task_history/PostCreateAsync_smt_task_history.cs` |

#### Ações Realizadas
- Atualiza a receita reconhecida na tarefa do projeto baseada no histórico.

#### Regras de Negócio

**Sem Valor Eventual:**
- Se `smt_dc_eventual` é nulo e o valor real (`smt_mn_value_real`) é diferente do valor incorreto (`smt_mn_wrong_value` ou 0):
  - Atualiza `smt_mn_recognized_revenue` na tarefa com o valor real.

**Com Valor Eventual:**
- Se `smt_dc_eventual` possui valor:
  - Calcula o valor total: valor do contrato × (eventual / 100).
  - Atualiza `smt_mn_recognized_revenue` e `smt_mn_estimated_revenue` na tarefa.

#### Efeitos Colaterais
- Atualização dos campos de receita reconhecida e estimada na tarefa do projeto relacionada.

---

## Resumo de Eventos por Entidade

| Entidade | PreValidation | PreCreate | PreUpdate | PostCreate (Sync) | PostCreate (Async) | PostUpdate (Sync) | PostUpdate (Async) | PostDelete |
|----------|---------------|-----------|-----------|-------------------|-------------------|-------------------|-------------------|------------|
| msdyn_expense | - | - | - | - | - | - | ✓ | - |
| msdyn_timeentry | - | ✓ | ✓ | - | - | - | ✓ | - |
| smt_integration | - | - | - | - | ✓ | - | - | - |
| smt_issue | - | - | - | - | - | ✓ | - | - |
| QuoteDetail | - | ✓ | ✓ | - | - | - | - | - |
| SalesOrderDetail | - | ✓ | ✓ | - | - | - | ✓ | - |
| SharePointDocumentLocation | ✓ | - | - | - | - | - | - | - |
| msdyn_project | - | ✓ | ✓ | ✓ | ✓ | ✓ | - | ✓ |
| Quote | - | - | ✓ | ✓ | - | - | ✓ | - |
| SalesOrder | - | ✓ | ✓ | - | - | - | ✓ | - |
| msdyn_projecttask | - | ✓ | ✓ | - | - | - | - | - |
| BookableResourceBooking | - | ✓ | ✓ | - | - | ✓ | - | ✓ |
| msdyn_projectapproval | - | - | ✓ | - | ✓ | - | - | - |
| msdyn_projectteam | - | - | ✓ | - | - | - | - | - |
| smt_imposto_parameter | - | ✓ | ✓ | - | - | - | - | - |
| smt_history_task | - | - | - | - | ✓ | - | - | - |

---

## Observações Técnicas Gerais

### Padrões de Implementação
1. Todos os plugins herdam da classe `PluginBase`.
2. O contexto de execução é acessado através do parâmetro `LocalPluginContext`.
3. Classes de negócio (Business) são utilizadas para encapsular a lógica complexa.
4. Validações de negócio utilizam `InvalidPluginExecutionException` para interromper operações inválidas.

### Serviços Utilizados
- `OrganizationService`: Serviço padrão com contexto do usuário.
- `OrganizationServiceAdmin`: Serviço com privilégios administrativos.
- `TracingService`: Serviço para logging e debug.

### Imagens de Plugin (Pre/Post Image)
- **PreImage**: Estado do registro antes da operação.
- **PostImage**: Estado do registro após a operação (apenas em Post-Operation).
- **Target**: Campos que estão sendo modificados na operação atual.

---

*Documento gerado para análise funcional dos plugins do projeto Pixeon.Extends.*
