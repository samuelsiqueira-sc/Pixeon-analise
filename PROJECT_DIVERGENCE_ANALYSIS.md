# Project Divergence Analysis Report

## Executive Summary

This report analyzes divergences between two projects:
- **Pixeon Project**: `Pixeon.Extends` and `Pixeon.WebResources`
- **Smart PO Project**: `Smart.ExtendedPSA.Extends` and `Smart.ExtendedPSA.WebResources`

### Key Findings

1. **Significant Divergence**: The two projects have substantial differences with very little overlap
   - Only **2 plugins** exist in both projects (with different implementations)
   - Only **1 web resource** exists in both projects (with different implementation)
   - **34 plugins** are unique to Pixeon, **39 plugins** are unique to Smart PO
   - **10 web resources** are unique to Pixeon, **37 web resources** are unique to Smart PO

2. **Different Project Scopes**: The projects appear to serve different business needs:
   - **Pixeon** focuses on: Expenses (Despesas), Issues, Quote lines, Contract lines, SharePoint integration, Sales Orders, and Bookable Resource Bookings
   - **Smart PO** focuses on: Holiday requests, Resource departures, Purchasing periods, Change requests, Project alerts, and extensive project management features

3. **Technology Approach Differences**: 
   - Smart PO's web resources use more modern JavaScript practices (async/await, Xrm.WebApi)
   - Pixeon includes XRM Query libraries for data access
   - Smart PO has more extensive JavaScript utilities (jQuery, JSON helpers, KPI calculators)

4. **Common Functionality with Different Implementation**:
   - Both projects handle Project creation and updates differently
   - Time entry forms use different API approaches (synchronous vs asynchronous)

---

## 1. Plugin Analysis (C# Source Files)

### 1.1 Plugins Unique to Pixeon Project

**Count:** 34 plugins

- **Despesas/PostUpdateAsync_msdyn_expense.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Despesas/PostUpdateAsync_msdyn_expense.cs`

- **Entrada de Hora/PostUpdateAsync.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Entrada de Hora/PostUpdateAsync.cs`

- **Entrada de Hora/PreCreateSync_msdyn_timeentry.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Entrada de Hora/PreCreateSync_msdyn_timeentry.cs`

- **Entrada de Hora/PreUpdateSync_msdyn_timeentry.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Entrada de Hora/PreUpdateSync_msdyn_timeentry.cs`

- **Integracao/PostCreateAsync_smt_integration.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Integracao/PostCreateAsync_smt_integration.cs`

- **Issue/PostUpdateSync.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Issue/PostUpdateSync.cs`

- **Linha da Cotação/PreCreateSync_QuoteDetail.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Linha da Cotação/PreCreateSync_QuoteDetail.cs`

- **Linha da Cotação/PreUpdateSync_QuoteDetail.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Linha da Cotação/PreUpdateSync_QuoteDetail.cs`

- **Linha do Contrato/PostUpdateAsync_SalesOrderDetail.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Linha do Contrato/PostUpdateAsync_SalesOrderDetail.cs`

- **Linha do Contrato/PreCreateSync.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Linha do Contrato/PreCreateSync.cs`

- **Linha do Contrato/PreUpdateSync.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Linha do Contrato/PreUpdateSync.cs`

- **Pasta Sharepoint/PreValidationSync_sharepointdocumentarion.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Pasta Sharepoint/PreValidationSync_sharepointdocumentarion.cs`

- **Projeto/PostCreateSync.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Projeto/PostCreateSync.cs`

- **Projeto/PostDeleteAsync.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Projeto/PostDeleteAsync.cs`

- **Projeto/PreCreateSync.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Projeto/PreCreateSync.cs`

- **Projeto/PreUpdateSync.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Projeto/PreUpdateSync.cs`

- **Quote/PostCreateSync_Quote.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Quote/PostCreateSync_Quote.cs`

- **Quote/PostUpdateAsync_quote.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Quote/PostUpdateAsync_quote.cs`

- **Quote/PreUpdateSync_quote.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Quote/PreUpdateSync_quote.cs`

- **SalesOrder/PostUpdateAsync_Salesorder.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/SalesOrder/PostUpdateAsync_Salesorder.cs`

- **SalesOrder/PreCreateSync_SalesOrder.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/SalesOrder/PreCreateSync_SalesOrder.cs`

- **SalesOrder/PreUpdateSync_SalesOrder.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/SalesOrder/PreUpdateSync_SalesOrder.cs`

- **Tarefa do Projeto/PreCreateSync_projecttask.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Tarefa do Projeto/PreCreateSync_projecttask.cs`

- **Tarefa do Projeto/PreUpdateSync_projecttask.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Tarefa do Projeto/PreUpdateSync_projecttask.cs`

- **msdyn_BookableResourceBooking/PostDeleteSync_msdyn_BookableResourceBooking.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/msdyn_BookableResourceBooking/PostDeleteSync_msdyn_BookableResourceBooking.cs`

- **msdyn_BookableResourceBooking/PostUpdateSync_BookableResourceBooking.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/msdyn_BookableResourceBooking/PostUpdateSync_BookableResourceBooking.cs`

- **msdyn_BookableResourceBooking/PreCreateSync_BookableResourceBooking.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/msdyn_BookableResourceBooking/PreCreateSync_BookableResourceBooking.cs`

- **msdyn_BookableResourceBooking/PreUpdate_BookableResourceBooking.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/msdyn_BookableResourceBooking/PreUpdate_BookableResourceBooking.cs`

- **msdyn_projectapproval/PostCreateAsync_msdyn_projectapproval.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/msdyn_projectapproval/PostCreateAsync_msdyn_projectapproval.cs`

- **msdyn_projectapproval/PreUpdateSync_msdyn_projectapproval.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/msdyn_projectapproval/PreUpdateSync_msdyn_projectapproval.cs`

- **msdyn_projectteam/PreUpdateSync_msdyn_projectteam.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/msdyn_projectteam/PreUpdateSync_msdyn_projectteam.cs`

- **smt_imposto_parameter/PreCreateSync_smt_imposto_parameter.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/smt_imposto_parameter/PreCreateSync_smt_imposto_parameter.cs`

- **smt_imposto_parameter/PreUpdateSync_smt_imposto_parameter.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/smt_imposto_parameter/PreUpdateSync_smt_imposto_parameter.cs`

- **smt_task_history/PostCreateAsync_smt_task_history.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/smt_task_history/PostCreateAsync_smt_task_history.cs`

### 1.2 Plugins Unique to Smart PO Project

**Count:** 39 plugins

- **Aprovação de Projeto/PreUpdateSync_msdyn_projectapproval.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/Aprovação de Projeto/PreUpdateSync_msdyn_projectapproval.cs`

- **Linha da Ordem/PostCreateSync.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/Linha da Ordem/PostCreateSync.cs`

- **Linha da Ordem/PostUpdateAsync.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/Linha da Ordem/PostUpdateAsync.cs`

- **Linha da Ordem/PostUpdateSync.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/Linha da Ordem/PostUpdateSync.cs`

- **Projeto/PostUpdateAsync.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/Projeto/PostUpdateAsync.cs`

- **Smt_purchasing_period/PostUpdateSync_smt_purchasing_period.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/Smt_purchasing_period/PostUpdateSync_smt_purchasing_period.cs`

- **Smt_purchasing_period/PreCreateSync_smt_purchasing_period.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/Smt_purchasing_period/PreCreateSync_smt_purchasing_period.cs`

- **Smt_purchasing_period/PreUpdateSync_smt_purchasing_period.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/Smt_purchasing_period/PreUpdateSync_smt_purchasing_period.cs`

- **Smt_resource_departure/PostUpdateAsync.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/Smt_resource_departure/PostUpdateAsync.cs`

- **Smt_resource_departure/PreCreateSync.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/Smt_resource_departure/PreCreateSync.cs`

- **Smt_resource_departure/PreUpdateSync.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/Smt_resource_departure/PreUpdateSync.cs`

- **msdyn_projecttask/PostCreateAsync_ msdyn_projecttask.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/msdyn_projecttask/PostCreateAsync_ msdyn_projecttask.cs`

- **msdyn_projecttask/PostUpdateAsync_msdyn_projecttask.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/msdyn_projecttask/PostUpdateAsync_msdyn_projecttask.cs`

- **msdyn_projecttask/PostUpdateSync_msdyn_projecttask.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/msdyn_projecttask/PostUpdateSync_msdyn_projecttask.cs`

- **msdyn_projecttask/PostUpdate_PostCreate_AssociateResourceWithProjectTask.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/msdyn_projecttask/PostUpdate_PostCreate_AssociateResourceWithProjectTask.cs`

- **msdyn_projecttask/PreUpdateSync_msdyn_projecttask.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/msdyn_projecttask/PreUpdateSync_msdyn_projecttask.cs`

- **msdyn_projecttask/[Desativaddo]PostCreateSync_msdyn_projecttask.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/msdyn_projecttask/[Desativaddo]PostCreateSync_msdyn_projecttask.cs`

- **msdyn_projecttask/[Desativaddo]PreCreateSync_msdyn_projecttask.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/msdyn_projecttask/[Desativaddo]PreCreateSync_msdyn_projecttask.cs`

- **msdyn_projectteam/SetIdResponsibleResourceTask.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/msdyn_projectteam/SetIdResponsibleResourceTask.cs`

- **msdyn_resourceassignment/PostUpdateSync_msdyn_resourceassignment.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/msdyn_resourceassignment/PostUpdateSync_msdyn_resourceassignment.cs`

- **msdyn_resourceassignment/PreValidationSync_msdyn_resourceassignment.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/msdyn_resourceassignment/PreValidationSync_msdyn_resourceassignment.cs`

- **msdyn_timeentry/PostUpdateAsync_msdyn_timeentry.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/msdyn_timeentry/PostUpdateAsync_msdyn_timeentry.cs`

- **msdyn_timeentry/PostUpdateSync_msdyn_timeentry.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/msdyn_timeentry/PostUpdateSync_msdyn_timeentry.cs`

- **msdyn_timeentry/PreCreateSync_msdyn_timeentry.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/msdyn_timeentry/PreCreateSync_msdyn_timeentry.cs`

- **msdyn_timeentry/PreUpdateSync_msdyn_timeentry.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/msdyn_timeentry/PreUpdateSync_msdyn_timeentry.cs`

- **quote/PostCreateSync_quote.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/quote/PostCreateSync_quote.cs`

- **quote/PostUpdateSync_quote.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/quote/PostUpdateSync_quote.cs`

- **smt_changefunctions/PreCreate_Changing_Resouces_Functions.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/smt_changefunctions/PreCreate_Changing_Resouces_Functions.cs`

- **smt_confirm_holiday_request/PreCreateConfirmHolidayRequest.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/smt_confirm_holiday_request/PreCreateConfirmHolidayRequest.cs`

- **smt_holidayrequest/PostUpdateAsync_smt_holiday_request.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/smt_holidayrequest/PostUpdateAsync_smt_holiday_request.cs`

- **smt_holidayrequest/PreCreateSync_smt_holiday_request.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/smt_holidayrequest/PreCreateSync_smt_holiday_request.cs`

- **smt_holidayrequest/PreCreate_ChangeAquisitionPeriod.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/smt_holidayrequest/PreCreate_ChangeAquisitionPeriod.cs`

- **smt_holidayrequest/PreUpdateSync_smt_holiday_request.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/smt_holidayrequest/PreUpdateSync_smt_holiday_request.cs`

- **smt_holidayrequest/Update_StatusReason_ChangePurchasingPeriod.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/smt_holidayrequest/Update_StatusReason_ChangePurchasingPeriod.cs`

- **smt_monthly_revenue_recognition/PostCreateAsync_smt_monthly_revenue_recognition.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/smt_monthly_revenue_recognition/PostCreateAsync_smt_monthly_revenue_recognition.cs`

- **smt_smartparameter/PreCreateSync_smt_smartparametert.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/smt_smartparameter/PreCreateSync_smt_smartparametert.cs`

- **smt_smartparameter/PreUpdateSync_smt_smartparameter.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/smt_smartparameter/PreUpdateSync_smt_smartparameter.cs`

- **smt_type_hours/PreCreateSync_smt_type_hours.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/smt_type_hours/PreCreateSync_smt_type_hours.cs`

- **smt_type_hours/PreUpdateSync_smt_type_hours.cs**
  - Path: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/smt_type_hours/PreUpdateSync_smt_type_hours.cs`

### 1.3 Plugins Present in Both Projects with Different Implementations

**Count:** 2 plugins

These plugins exist in both projects but have different business logic or implementation:

- **Projeto/PostCreateAsync.cs**
  - Pixeon: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Projeto/PostCreateAsync.cs` (57 lines)
  - Smart PO: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/Projeto/PostCreateAsync.cs` (124 lines)
  - **Functional Difference**: Both plugins trigger after project creation (`msdyn_project`), but:
    - **Pixeon version**: Only creates a team and assigns the project to it (via `CreateTeam` method)
    - **Smart PO version**: Has significantly more functionality including creating project tasks, team members, operation sets, and project buckets (via `InsertTasks` method calling multiple scheduling APIs)
  - **Business Impact**: Smart PO has more comprehensive project initialization with automated task creation, while Pixeon has a simpler team-only setup

- **Projeto/PostUpdateSync.cs**
  - Pixeon: `/home/runner/work/Pixeon-analise/Pixeon-analise/Pixeon.Extends/Plugins/Projeto/PostUpdateSync.cs` (63 lines)
  - Smart PO: `/home/runner/work/Pixeon-analise/Pixeon-analise/Smart PO/Smart.ExtendedPSA/Smart.ExtendedPSA.Extends/Plugins/Projeto/PostUpdateSync.cs` (69 lines)
  - **Functional Difference**: Both plugins handle project updates, but may have different validation rules or business logic in how they process updates
  - **Business Impact**: Differences in project update handling logic could lead to different validation behaviors or side effects

### 1.4 Plugins with Identical Implementations

*No plugins with identical implementations found.*

---

## 2. Web Resources Analysis (JavaScript and HTML Files)

### 2.1 Web Resources Unique to Pixeon Project

**Count:** 10 files

- **AccountForm.js**
  - Path: `js/AccountForm.js`

- **Portal_msdyn_projecttask.js**
  - Path: `js/Portal_msdyn_projecttask.js`

- **dg.xrmquery.web.js**
  - Path: `src/lib/dg.xrmquery.web.js`

- **dg.xrmquery.web.min.js**
  - Path: `src/lib/dg.xrmquery.web.min.js`

- **dg.xrmquery.web.promise.min.js**
  - Path: `src/lib/dg.xrmquery.web.promise.min.js`

- **msdyn_project_pixeon.js**
  - Path: `js/msdyn_project_pixeon.js`

- **quote.js**
  - Path: `js/quote.js`

- **smt_html_alterareserva.html**
  - Path: `HTML/smt_html_alterareserva.html`

- **smt_html_jira_blank.html**
  - Path: `webpage/smt_html_jira_blank.html`

- **smt_milestone_project.js**
  - Path: `js/smt_milestone_project.js`

### 2.2 Web Resources Unique to Smart PO Project

**Count:** 37 files

- **EntidadeJS.js**
  - Path: `Javascripts/EntidadeJS.js`

- **smt_ExtendedHelp.js**
  - Path: `Javascripts/smt_ExtendedHelp.js`

- **smt_Script_TriggerPluginChangeRequest.js**
  - Path: `Javascripts/smt_Script_TriggerPluginChangeRequest.js`

- **smt_alerta_vencimento_produto_smart.js**
  - Path: `Javascripts/smt_alerta_vencimento_produto_smart.js`

- **smt_calculate_rollup_.js**
  - Path: `Javascripts/smt_calculate_rollup_.js`

- **smt_html_margem_projeto.html**
  - Path: `Html/smt_html_margem_projeto.html`

- **smt_html_powerbi_cronogramaprojetos.html**
  - Path: `Html/smt_html_powerbi_cronogramaprojetos.html`

- **smt_html_powerbi_cronogramatarefas.html**
  - Path: `Html/smt_html_powerbi_cronogramatarefas.html`

- **smt_js_StatusChange.js**
  - Path: `Javascripts/smt_js_StatusChange.js`

- **smt_js_bookableresource.js**
  - Path: `Javascripts/smt_js_bookableresource.js`

- **smt_js_call_JustForm.js**
  - Path: `Javascripts/smt_js_call_JustForm.js`

- **smt_js_cancelprojecttask.js**
  - Path: `Javascripts/smt_js_cancelprojecttask.js`

- **smt_js_change_functions.js**
  - Path: `Javascripts/smt_js_change_functions.js`

- **smt_js_change_request.js**
  - Path: `Javascripts/smt_js_change_request.js`

- **smt_js_extendedhelp.js**
  - Path: `Javascripts/smt_js_extendedhelp.js`

- **smt_js_hideShowAprovalSection.js**
  - Path: `Javascripts/smt_js_hideShowAprovalSection.js`

- **smt_js_holiday_request - Cópia .js**
  - Path: `Javascripts/smt_js_holiday_request - Cópia .js`

- **smt_js_holiday_request.js**
  - Path: `Javascripts/smt_js_holiday_request.js`

- **smt_js_holidayrequestcreat.js**
  - Path: `Javascripts/smt_js_holidayrequestcreat.js`

- **smt_js_invoice_onchangefields.js**
  - Path: `Javascripts/smt_js_invoice_onchangefields.js`

- **smt_js_jquery.js**
  - Path: `Javascripts/smt_js_jquery.js`

- **smt_js_json.js**
  - Path: `Javascripts/smt_js_json.js`

- **smt_js_kpirecalculate.js**
  - Path: `Javascripts/smt_js_kpirecalculate.js`

- **smt_js_project.js**
  - Path: `Javascripts/smt_js_project.js`

- **smt_js_projectGridIcon.js**
  - Path: `Javascripts/smt_js_projectGridIcon.js`

- **smt_js_project_alert.js**
  - Path: `Javascripts/smt_js_project_alert.js`

- **smt_js_purchasing_period.js**
  - Path: `Javascripts/smt_js_purchasing_period.js`

- **smt_js_quote.js**
  - Path: `Javascripts/smt_js_quote.js`

- **smt_js_quotedetail.js**
  - Path: `Javascripts/smt_js_quotedetail.js`

- **smt_js_resource_departure.js**
  - Path: `Javascripts/smt_js_resource_departure.js`

- **smt_js_salesOrderUnlink.js**
  - Path: `Javascripts/smt_js_salesOrderUnlink.js`

- **smt_js_salesorder.js**
  - Path: `Javascripts/smt_js_salesorder.js`

- **smt_js_smarthelper.js**
  - Path: `Javascripts/smt_js_smarthelper.js`

- **smt_js_timebank_settings.js**
  - Path: `Javascripts/smt_js_timebank_settings.js`

- **smt_script_CallAction_CreateBaseLine.js**
  - Path: `Javascripts/smt_script_CallAction_CreateBaseLine.js`

- **smt_script_createChangeRequest.js**
  - Path: `Javascripts/smt_script_createChangeRequest.js`

- **smt_verificadorCampo.js**
  - Path: `Javascripts/smt_verificadorCampo.js`

### 2.3 Web Resources Present in Both Projects with Different Implementations

**Count:** 1 files

These web resources exist in both projects but have different implementations:

- **smt_js_time_entry.js**
  - Pixeon: `js/smt_js_time_entry.js` (453 lines)
  - Smart PO: `Javascripts/smt_js_time_entry.js` (434 lines)
  - **Functional Difference**: Both implement time entry form logic, but:
    - **Pixeon version**: Uses synchronous XMLHttpRequest for retrieving project task data
    - **Smart PO version**: Uses modern asynchronous `Xrm.WebApi.online.retrieveRecord` with promise-based approach and includes "use strict" directive
  - **Business Impact**: Smart PO version follows modern best practices with async API calls and better error handling, while Pixeon uses deprecated synchronous requests that can block the UI

### 2.4 Web Resources with Identical Implementations

*No web resources with identical implementations found.*

---

## 3. Summary Statistics

### Plugins

- **Total Pixeon Plugins:** 36
- **Total Smart PO Plugins:** 41
- **Unique to Pixeon:** 34
- **Unique to Smart PO:** 39
- **Common (Identical):** 0
- **Common (Different):** 2

### Web Resources

- **Total Pixeon Web Resources:** 11
- **Total Smart PO Web Resources:** 38
- **Unique to Pixeon:** 10
- **Unique to Smart PO:** 37
- **Common (Identical):** 0
- **Common (Different):** 1
