class SmartHelper {
    constructor(_executionContext) {
        if (_executionContext) {
            this.ExecutionContext = _executionContext;

            this._data = new _Data(this);
            this._form = new _Form(this);
            this._userSettings = new _UserSettings(this);
            this._organizationSettings = new _OrganizationSettings(this);

            this._process = new _Process(this);
            this._tabs = new _Tabs(this);
            this._iframeWebResource = new _IframeWebResource(this);
            this._subGrid = new _SubGrid(this);

            this._controls = new _Controls(this);
            this._attributes = new _Attributes(this);

            this._lookup = new _Lookup(this);
            this._dateTime = new _DateTime(this);
            this._optionSet = new _OptionSet(this);

            this._common = new _Common(this);

            this._utilities = new _Utilities(this);
        }
        else
            Xrm.Navigation.openAlertDialog("ExecutionContext não fornecido!");
    }
}
class _Data {
    constructor(_SmartHelper) {
        this.SmartHelper = _SmartHelper;
    }
    GetEntityName() {
        return this.SmartHelper.ExecutionContext.getFormContext().data.entity.getEntityName().toLowerCase();
    }
}
class _Form {
    constructor(_SmartHelper) {
        this.SmartHelper = _SmartHelper;
    }
    GetUrl() {
        return this.SmartHelper.ExecutionContext.getFormContext().getUrl();
    }
    GetFormType() {
        //0 = Undefined
        //1 = Create
        //2 = Update
        //3 = Read Only
        //4 = Disabled
        //5 = Quick Create (Deprecated)
        //6 = Bulk Edit
        //11 = Read Optimized (Deprecated)
        return this.SmartHelper.ExecutionContext.getFormContext().ui.getFormType();
    }
    GetFormTitle() {
        return this.SmartHelper.ExecutionContext.getFormContext().ui.get_formTitle();
    }
    SetFormTitle(_value) {
        return this.SmartHelper.ExecutionContext.getFormContext().ui.set_formTitle(_value);
    }
    SetFormNotification(_type, _message, _id) {
        var typeOK = false;
        switch (_type) {
            case "ERROR":
            case "WARNING":
            case "INFORMATION":
                typeOK = true;
                break;
            default:
                typeOK = false;
                break;
        }
        if (typeOK) {
            this.SmartHelper.ExecutionContext.getFormContext().ui.setFormNotification(_message, _type, _id);
        }
    }
    SetHtmlFormNotification(_type, _htmlMessage, _id) {
        var typeOK = false;
        switch (_type) {
            case "ERROR":
            case "WARNING":
            case "INFORMATION":
                typeOK = true;
                break;
            default:
                typeOK = false;
                break;
        }
        if (typeOK) {
            this.SmartHelper.ExecutionContext.getFormContext().ui.setFormHtmlNotification(_htmlMessage, _type, _id);
        }
    }
    ClearFormNotification(_id) {
        this.SmartHelper.ExecutionContext.getFormContext().ui.clearFormNotification(_id);
    }
    Confirm(_title, _message, _buttonConfirm, _buttonCancel) {
        var return_ = false;
        var confirmStrings = { title: _title, text: _message, confirmButtonLabel: _buttonConfirm, cancelButtonLabel: _buttonCancel };
        var confirmOptions = { height: 200, width: 400 };
        Xrm.Navigation.openConfirmDialog(confirmStrings, confirmOptions).then(
            return_ = function (success) {
                return success.confirmed;
            });
        return return_;
    }
    Alert(_message, _buttonConfirm) {
        var alertStrings = { text: _message, confirmButtonLabel: _buttonConfirm };
        var alertOptions = { height: 200, width: 400 };
        Xrm.Navigation.openAlertDialog(alertStrings, alertOptions).then(
            function success(result) {
                //console.log("Alert dialog closed");
            },
            function (error) {
                //console.log(error.message);
            }
        );
    }
    RefreshRibbon() {
        this.SmartHelper.ExecutionContext.getFormContext().ui.refreshRibbon();
    }
}
class _UserSettings {
    constructor(_SmartHelper) {
        this.SmartHelper = _SmartHelper;
        this.LanguageId = this.SmartHelper.ExecutionContext.getFormContext().context.userSettings.languageId;
        this.SecurityRoles = this.SmartHelper.ExecutionContext.getFormContext().context.userSettings.securityRoles;
        this.UserId = this.SmartHelper.ExecutionContext.getFormContext().context.userSettings.userId;
        this.UserName = this.SmartHelper.ExecutionContext.getFormContext().context.userSettings.userName;
        this.TransactionCurrencyId = this.SmartHelper.ExecutionContext.getFormContext().context.userSettings.transactionCurrencyId;
    }
}
class _OrganizationSettings {
    constructor(_SmartHelper) {
        this.SmartHelper = _SmartHelper;
        this.Attributes = _SmartHelper.ExecutionContext.getFormContext().context.organizationSettings.attributes;
        this.BaseCurrencyId = _SmartHelper.ExecutionContext.getFormContext().context.organizationSettings.baseCurrencyId;
        this.IsAutoSaveEnabled = _SmartHelper.ExecutionContext.getFormContext().context.organizationSettings.isAutoSaveEnabled;
        this.LanguageId = _SmartHelper.ExecutionContext.getFormContext().context.organizationSettings.languageId;
        this.OrganizationId = _SmartHelper.ExecutionContext.getFormContext().context.organizationSettings.organizationId;
        this.UniqueName = _SmartHelper.ExecutionContext.getFormContext().context.organizationSettings.uniqueName;
        this.UseSkypeProtocol = _SmartHelper.ExecutionContext.getFormContext().context.organizationSettings.useSkypeProtocol;
        //this.GetClientUrl = Xrm.Utility.getGlobalContext().getClientUrl();
    }
}
class _Process {
    constructor(_SmartHelper) {
        this.SmartHelper = _SmartHelper;
        this.Process = this.SmartHelper.ExecutionContext.getFormContext().data.process;
    }
    AbandonProcess() { }
    AddOnProcessStatusChange() { }
    AddOnStateChange() { }
    AddOnStageSelected() { }
    CanSetActiveStage() { }
    CompleteProcess() { }
    GetActivePath() { }
    GetActiveProcess() { }
    GetEnabledProcesses() { }
    GetInstanceId() { }
    GetInstanceName() { }
    GetProcessInstances() { }
    GetSelectedStage() { }
    GetStatus() { }
    IsLastStage() { }
    MoveNext() { }
    MovePrevious() { }
    ReactivateProcess() { }
    RemoveOnProcessStatusChange() { }
    RemoveOnStageChange() { }
    RemoveOnStageSelected() { }
    SetActiveProcess() { }
    SetActiveProcessInstance() { }
    SetActiveStage() { }
    SetStatus() { }
    SwitchProcess() { }
}
class _Tabs {
    constructor(_SmartHelper) {
        this.SmartHelper = _SmartHelper;
        this.Process = this.SmartHelper.ExecutionContext.getFormContext().ui.tabs;
    }
    GetAll() { }
    GetByName() { }
}
class _Controls {
    constructor(_SmartHelper) {
        this.SmartHelper = _SmartHelper;
    }
    GetControlType(_controlName) {
        //standard =  Um controle padrão.
        //iframe =  Um controle do IFRAME.
        //lookup =  Um controle de pesquisa.
        //optionset =  Um controle conjunto de opções.
        //subgrid =  Um controle de subgrade.
        //webresource =  Um controle de recurso da Web.
        //notes =  Um controle de anotações.
        //timercontrol =  Um controle de timer.
        //kbsearch =  Um controle de pesquisa da base de dados de conhecimento.
        //customcontrol: <namespace>.<name> =  Um controle personalizado para clientes móveis (telefones e tablets) do Dynamics 365.
        //customsubgrid:<namespace>.<name> =  Um conjunto de dados personalizado para clientes móveis (telefones e tablets) do Dynamics 365
        if (this.InForm(_controlName))
            return this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).getControlType();
        else
            return null;
    }
    InForm(_controlName) {
        if (this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName) !== null)
            return true;
        else
            return false;
    }
    GetVisible(_controlName) {
        if (this.InForm(_controlName))
            return this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).getVisible();
        else
            return null;
    }
    SetVisible(_controlName, _visibility) {
        if (this.InForm(_controlName))
            this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).setVisible(_visibility);
    }
    GetDisabled(_controlName) {
        if (this.InForm(_controlName))
            return this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).getDisabled();
        else
            return null;
    }
    SetDisabled(_controlName, _enabled) {
        if (this.InForm(_controlName))
            this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).setDisabled(_enabled);
    }
    GetLabel(_controlName) {
        if (this.InForm(_controlName))
            this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).getLabel();
    }
    SetLabel(_controlName, _label) {
        if (this.InForm(_controlName))
            this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).setLabel(_label);
    }
    GetFocus(_controlName) {
        if (this.InForm(_controlName))
            this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).getFocus();
    }
    SetFocus(_controlName) {
        if (this.InForm(_controlName))
            this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).setFocus();
    }
}
class _Attributes {
    constructor(_SmartHelper) {
        this.SmartHelper = _SmartHelper;
        const REQUIRED = "required";
        const RECOMMENDED = "recommended";
        const NONE = "none";

        const ERROR = "ERROR";
        const WARNING = "WARNING";
        const INFORMATION = "INFORMATION";
    }
    GetAttributeType(_fieldName) {
        //boolean
        //datetime
        //decimal
        //double
        //integer
        //lookup
        //memo
        //money
        //optionset
        //string
        if (this.SmartHelper._controls.InForm(_fieldName)) {
            var controlType = this.SmartHelper._controls.GetControlType(_fieldName);
            if (controlType === "standard" || controlType === "lookup" || controlType === "optionset")
                return this.SmartHelper.ExecutionContext.getFormContext().getAttribute(_fieldName).getAttributeType();
            else
                return null;
        }
        else
            return null;
    }
    GetUserPrivilege(_fieldName) {
        //canCreate
        //canRead
        //canUpdate
        if (this.SmartHelper._controls.InForm(_fieldName))
            return this.SmartHelper.ExecutionContext.getFormContext().getAttribute(_fieldName).getUserPrivilege();
        else
            return null;
    }
    ValidationsSetValue(_fieldName, _value) {
        var ok = false;
        if (this.SmartHelper._controls.InForm(_fieldName)) {
            var attributeType = this.GetAttributeType(_fieldName);
            switch (attributeType) {
                case "string":
                case "memo":
                    if (_value === null || typeof _value === "string")
                        ok = true;
                    break;

                case "decimal":
                case "double":
                case "integer":
                case "money":
                case "optionset":
                    if (_value === null || typeof _value === "number")
                        ok = true;
                    break;

                case "boolean":
                    if (_value === null || typeof _value === "boolean")
                        ok = true;
                    break;

                case "datetime":
                    if (_value === null || typeof new Date(_value) === "object")
                        ok = true;
                    break;

                case "lookup": //melhorar a tratativa
                    if (_value === null || typeof _value === "object")
                        ok = true;
                    break;
            }

            if (ok) {
                ok = false;
                var userPriveles = this.GetUserPrivilege(_fieldName);
                if (userPriveles.canRead) {
                    var formType = this.SmartHelper._form.GetFormType();
                    if (formType === 1 && userPriveles.canCreate)
                        ok = true;
                    else if (formType === 2 && userPriveles.canUpdate)
                        ok = true;
                }
            }
        }
        return ok;
    }
    GetValue(_fieldName) {
        if (this.SmartHelper._controls.InForm(_fieldName))
            return this.SmartHelper.ExecutionContext.getFormContext().getAttribute(_fieldName).getValue();
    }
    SetValue(_fieldName, _value) {
        if (this.SmartHelper._controls.InForm(_fieldName)) {
            if (this.ValidationsSetValue(_fieldName, _value))
                this.SmartHelper.ExecutionContext.getFormContext().getAttribute(_fieldName).setValue(_value);
        }
    }
    SetSubmitMode(_fieldName, _mode) {
        var modeOK = false;
        switch (_mode) {
            case "always":
            case "never":
            case "dirty":
                modeOK = true;
                break;
            default:
                modeOK = false;
                break;
        }
        if (modeOK) {
            if (_fieldName !== null) {
                if (this.SmartHelper._controls.InForm(_fieldName))
                    this.SmartHelper.ExecutionContext.getFormContext().getAttribute(_fieldName).setSubmitMode(_mode);
            }
        }
    }
    ResetInitialValue(_fieldName, _function) {
        if (_fieldName !== null) {
            if (this.SmartHelper._controls.InForm(_fieldName)) {
                var attributeType = this.GetAttributeType(_fieldName);
                if (attributeType === "boolean" || attributeType === "optionset")
                    this.SmartHelper.ExecutionContext.getFormContext().getAttribute(_fieldName).resetInitialValue();
            }
        }
    }
    GetRequiredLevel(_fieldName) {
        if (this.SmartHelper._controls.InForm(_fieldName))
            return this.SmartHelper.ExecutionContext.getFormContext().getAttribute(_fieldName).getRequiredLevel();
        else
            return null;
    }
    SetRequiredLevel(_fieldName, _level) {
        if (this.SmartHelper._controls.InForm(_fieldName))
            this.SmartHelper.ExecutionContext.getFormContext().getAttribute(_fieldName).setRequiredLevel(_level);
    }
    GetIsDirty(_fieldName) {
        if (this.SmartHelper._controls.InForm(_fieldName))
            this.SmartHelper.ExecutionContext.getFormContext().getAttribute(_fieldName).getIsDirty();
    }
    AddNotification(_fieldName, _type, _message) {
        var typeOK = false;
        switch (_type) {
            case "ERROR":
            case "WARNING":
            case "INFORMATION":
                typeOK = true;
                break;
            default:
                typeOK = false;
                break;
        }
        if (typeOK) {
            if (_fieldName !== null) {
                if (this.SmartHelper._controls.InForm(_fieldName))
                    this.SmartHelper.ExecutionContext.getFormContext().getControl(_fieldName).setNotification(_message, _type, "notf_id_" + _fieldName);
            }
        }
    }
    ClearNotification(_fieldName, _notificationsIds) {
        if (_fieldName !== null) {
            if (this.SmartHelper._controls.InForm(_fieldName)) {
                if (_notificationsIds !== null) {
                    for (var i = 0; i < _notificationsIds.length; i++)
                        this.SmartHelper.ExecutionContext.getFormContext().getControl(_fieldName).clearNotification(_notificationsIds[i]);
                }
                else
                    this.SmartHelper.ExecutionContext.getFormContext().getControl(_fieldName).clearNotification();
            }
        }
    }
    AddOnchange(_fieldName, _function) {
        if (_fieldName !== null) {
            if (this.SmartHelper._controls.InForm(_fieldName))
                this.SmartHelper.ExecutionContext.getFormContext().getAttribute(_fieldName).addOnChange(_function);
        }
    }
    RemoveOnchange(_fieldName, _function) {
        if (_fieldName !== null) {
            if (this.SmartHelper._controls.InForm(_fieldName))
                this.SmartHelper.ExecutionContext.getFormContext().getAttribute(_fieldName).removeOnChange(_function);
        }
    }
    FireOnchange(_fieldName) {
        if (_fieldName !== null) {
            if (this.SmartHelper._controls.InForm(_fieldName))
                this.SmartHelper.ExecutionContext.getFormContext().getAttribute(_fieldName).fireOnChange();
        }
    }
}
class _Lookup {
    constructor(_SmartHelper) {
        this.SmartHelper = _SmartHelper;
    }
    AddCustomFilter(_fieldName, _logicalName, _customFilter) {
        if (this.SmartHelper._controls.InForm(_fieldName))
            this.SmartHelper.ExecutionContext.getFormContext().getControl(_fieldName).addCustomFilter(_customFilter, _logicalName);
    }
    AddPreSearch(_fieldName, _functionCustomFilter) {
        if (this.SmartHelper._controls.InForm(_fieldName))
            this.SmartHelper.ExecutionContext.getFormContext().getControl(_fieldName).addPreSearch(_function);
    }
    RemovePreSearch(_fieldName, _functionCustomFilter) {
        if (this.SmartHelper._controls.InForm(_fieldName))
            this.SmartHelper.ExecutionContext.getFormContext().getControl(_fieldName).removePreSearch(_function);
    }
    AddCustomView(_fieldName, _viewId, _logicalName, _viewDisplayName, _fetchXml, _layoutXml, _isDefault) {
        if (this.SmartHelper._controls.InForm(_fieldName))
            this.SmartHelper.ExecutionContext.getFormContext().getControl(_fieldName).addCustomView(_viewId, _logicalName, _viewDisplayName, _fetchXml, _layoutXml, _isDefault);
    }
    GetDeafultView(_fieldName) {
        if (this.SmartHelper._controls.InForm(_fieldName))
            this.SmartHelper.ExecutionContext.getFormContext().getControl(_fieldName).getDefaultView();
    }
    SetDeafultView(_fieldName, _viewId) {
        if (this.SmartHelper._controls.InForm(_fieldName))
            this.SmartHelper.ExecutionContext.getFormContext().getControl(_fieldName).setDefaultView(_viewId);
    }
}
class _DateTime {
    constructor(_SmartHelper) {
        this.SmartHelper = _SmartHelper;
    }
    /*<summary>Veja se um controle de data mostra a parte da hora da data.</summary>
    <param name="_fieldName" type="string" sample="createdon" required="true"/>
    <return type="bool"/>*/
    GetShowTime(_fieldName) {
        if (this.SmartHelper._controls.InForm(_fieldName)) {
            if (this.SmartHelper._attributes.GetAttributeType(_fieldName) === "datetime")
                this.SmartHelper.ExecutionContext.getFormContext().getControl(_fieldName).getShowTime();
        }
    }
    /*<summary>Especificar se um controle de data deve mostrar a parte de hora da data.</summary>
    <param name="_fieldName" type="string" sample="createdon" required="true"/>
    <param name="_bool" type="bool" required="true"/>*/
    SetShowTime(_fieldName, _bool) {
        if (this.SmartHelper._controls.InForm(_fieldName)) {
            if (this.SmartHelper._attributes.GetAttributeType(_fieldName) === "datetime")
                this.SmartHelper.ExecutionContext.getFormContext().getControl(_fieldName).setShowTime(_bool);
        }
    }
}
class _OptionSet {
    constructor(_SmartHelper) {
        this.SmartHelper = _SmartHelper;
    }
    /*<summary>Adiciona uma nova opção a um determinado conjunto de opções</summary>
    <param name="_fieldName" type="string" sample="statuscode" required="true"/>
    <param name="_option" type="Object" sample="var option = { value: 100000000, text: "Option 07" }" required="true"/>
    <param name="_index" type="int" sample="0" required="false" comments="caso não seja fornecido, a opção será adicionada no fim da lista"/>*/
    AddOption(_fieldName, _option, _index) {
        if (this.SmartHelper._controls.InForm(_fieldName)) {
            if (this.SmartHelper._attributes.GetAttributeType(_fieldName) === "optionset") {
                if (_index !== null && parseInt(_index) !== NaN)
                    this.SmartHelper.ExecutionContext.getFormContext().getControl(_fieldName).addOption(_option, _index);
                else
                    this.SmartHelper.ExecutionContext.getFormContext().getControl(_fieldName).addOption(_option);
            }
        }
    }

    /*<summary>Remove uma nova opção de um determinado conjunto de opções com base no index</summary>
    <param name="_fieldName" type="string" sample="statuscode" required="true"/>
    <param name="_index" type="int" sample="0" required="true"/>*/
    RemoveOption(_fieldName, _index) {
        if (this.SmartHelper._controls.InForm(_fieldName)) {
            if (this.SmartHelper._attributes.GetAttributeType(_fieldName) === "optionset") {
                if (_index !== null && parseInt(_index) !== NaN)
                    this.SmartHelper.ExecutionContext.getFormContext().getControl(_fieldName).removeOption(_index);
            }
        }
    }

    /*<summary>Limpa as opções de um determinado conjunto de opções</summary>
    <param name="_fieldName" type="string" sample="statuscode" required="true"/>*/
    ClearOptions(_fieldName) {
        if (this.SmartHelper._controls.InForm(_fieldName)) {
            if (this.SmartHelper._attributes.GetAttributeType(_fieldName) === "optionset") {
                this.SmartHelper.ExecutionContext.getFormContext().getControl(_fieldName).clearOptions();
            }
        }
    }

    /*<summary>Obtém o valor default de um determinado conjunto de opções</summary>
    <param name="_fieldName" type="string" sample="statuscode" required="true"/>
    <return type="Object" sample="{ value: 100000000, text: "Option 07"/>*/
    GetInitialValue(_fieldName) {
        if (this.SmartHelper._controls.InForm(_fieldName)) {
            var attributeType = this.SmartHelper._attributes.GetAttributeType(_fieldName);
            if (attributeType === "optionset" || attributeType === "boolean")
                return this.SmartHelper.ExecutionContext.getFormContext().getAttribute(_fieldName).getInitialValue();
        }
    }

    /*<summary>Obtém todas as opções de um determinado conjunto de opções</summary>
    <param name="_fieldName" type="string" sample="statuscode" required="true"/>
    <return type="Array(Object)" sample="{ value: 100000000, text: "Option 07"/>*/
    GetOptions(_fieldName) {
        if (this.SmartHelper._controls.InForm(_fieldName)) {
            var attributeType = this.SmartHelper._attributes.GetAttributeType(_fieldName);
            if (attributeType === "optionset" || attributeType === "boolean")
                return this.SmartHelper.ExecutionContext.getFormContext().getAttribute(_fieldName).getOptions();
        }
    }

    /*<summary>Obtém a opção selecionada de um determinado conjunto de opções</summary>
    <param name="_fieldName" type="string" sample="statuscode" required="true"/>
    <return type="Object" sample="{ value: 100000000, text: "Option 07"/>*/
    GetSelectedOption(_fieldName) {
        if (this.SmartHelper._controls.InForm(_fieldName)) {
            var attributeType = this.SmartHelper._attributes.GetAttributeType(_fieldName);
            if (attributeType === "optionset" || attributeType === "boolean")
                return this.SmartHelper.ExecutionContext.getFormContext().getAttribute(_fieldName).getSelectedOption();
        }
    }
}
class _IframeWebResource {
    constructor(_SmartHelper) {
        this.SmartHelper = _SmartHelper;
    }
    /*<summary>Retorna a URL padrão que um controle IFRAME está configurado para exibir. Este método não está disponível para recursos da Web.</summary>
    <param name="_controlName" type="string" sample="IFRMAE_FormularioExterno" required="true"/>
    <return type="string" sample="http://google.com.br"/>*/
    GetInitialUrl(_controlName) {
        if (this.SmartHelper._controls.InForm(_controlName)) {
            var controlType = this.SmartHelper._controls.GetControlType(_controlName);
            if (controlType === "iframe" || controlType === "webresource")
                return this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).getInitialUrl();
        }
    }

    /*<summary>Retorna o objeto no formulário que representa o recurso da Web ou um I-frame. Um IFRAME retorna o elemento IFrame do Modelo de Objeto de Documento (DOM).</summary>
    <param name="_controlName" type="string" sample="IFRMAE_FormularioExterno" required="true"/>
    <return type="DOM" sample="??"/>*/
    GetObject(_controlName) {
        if (this.SmartHelper._controls.InForm(_controlName)) {
            var controlType = this.SmartHelper._controls.GetControlType(_controlName);
            if (controlType === "iframe" || controlType === "webresource")
                return this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).getObject();
        }
    }

    /*<summary>Retorna a URL atual que está sendo exibida em um IFRAME ou um recurso da Web.</summary>
    <param name="_controlName" type="string" sample="IFRMAE_FormularioExterno" required="true"/>
    <return type="string" sample="http://google.com.br"/>*/
    GetSrc(_controlName) {
        if (this.SmartHelper._controls.InForm(_controlName)) {
            var controlType = this.SmartHelper._controls.GetControlType(_controlName);
            if (controlType === "iframe" || controlType === "webresource")
                return this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).getSrc();
        }
    }

    /*<summary>Retorna a URL atual que está sendo exibida em um IFRAME ou um recurso da Web.</summary>
    <param name="_controlName" type="string" sample="IFRMAE_FormularioExterno" required="true"/>
    <param name="_url" type="string" sample="http://google.com.br" required="true"/>*/
    SetSrc(_controlName, _url) {
        if (this.SmartHelper._controls.InForm(_controlName)) {
            var controlType = this.SmartHelper._controls.GetControlType(_controlName);
            if (controlType === "iframe" || controlType === "webresource")
                return this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).setSrc(_url);
        }
    }
}
class _SubGrid {
    constructor(_SmartHelper) {
        this.SmartHelper = _SmartHelper;
    }
    /*<summary>Adiciona uma função a um subgrid, sempre que o mesmo for atualizado a função será executada</summary>
    <param name="_controlName" type="string" sample="subgrid_contatos" required="true"/>
    <param name="_functionOnLoad" type="function" sample="var func = function() { alert("Hi"); }"/>*/
    AddOnLoad(_controlName, _functionOnLoad) {
        if (this.SmartHelper._controls.InForm(_controlName)) {
            if (this.SmartHelper._controls.GetControlType(_controlName) === "subgrid") {
                if (_functionOnLoad !== null)
                    this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).addOnLoad(_functionOnLoad);
                else
                    this.AddNotification(_controlName, ERROR, "A função não pode ser nula.");
            }
            else
                this.AddNotification(_controlName, ERROR, "Este campo não é um SubGrid.");
        }
    }

    /*<summary>Remove uma determinada função de um subgrid</summary>
    <param name="_controlName" type="string" sample="subgrid_contatos" required="true"/>
    <param name="_functionOnLoad" type="function" sample="var func = function() { alert("Hi"); }"/>*/
    RemoveOnLoad(_controlName, _functionOnLoad) {
        if (this.SmartHelper._controls.InForm(_controlName)) {
            if (this.SmartHelper._controls.GetControlType(_controlName) === "subgrid") {
                if (_functionOnLoad !== null)
                    this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).removeOnLoad(_functionOnLoad);
                else
                    this.AddNotification(_controlName, ERROR, "A função não pode ser nula.");
            }
            else
                this.AddNotification(_controlName, ERROR, "Este campo não é um SubGrid.");
        }
    }

    /*<summary>Adiciona uma função a um subgrid, sempre que o mesmo for atualizado a função será executada</summary>
    <param name="_controlName" type="string" sample="subgrid_contatos" required="true"/>
    <return type="string" sample="account"/>*/
    GetEntityName(_controlName) {
        if (this.SmartHelper._controls.InForm(_controlName)) {
            if (this.SmartHelper._controls.GetControlType(_controlName) === "subgrid")
                return this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).getEntityName();
        }
    }

    /*<summary>Adiciona uma função a um subgrid, sempre que o mesmo for atualizado a função será executada</summary>
    <param name="_controlName" type="string" sample="subgrid_contatos" required="true"/>
    <param name="_onlySelectedRows" type="bool"/>
    <return type="Array(Object)"/>*/
    GetRows(_controlName, _onlySelectedRows) {
        if (this.SmartHelper._controls.InForm(_controlName)) {
            if (this.SmartHelper._controls.GetControlType(_controlName) === "subgrid") {
                var rows;
                if (_onlySelectedRows)
                    rows = this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).getGrid().getSelectedRows();
                else
                    rows = this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).getGrid().getRows();

                var entityCollection = [];
                rows.forEach(function (rows, i) {

                    var entityReference = rows.getData().getEntity().getEntityReference();

                    var entity = {};
                    entity.id = entityReference.id;
                    entity.entityType = entityReference.entityType;
                    entity.name = entityReference.name;
                    entity.entityReference = entityReference;
                    entity.attributes = [];

                    var colmuns = rows.getData().getAttributes();
                    colmuns.forEach(function (colmuns, i) {
                        var attribute = {};
                        attribute.name = colmuns.getName();
                        attribute.name = colmuns.getValue();
                        entity.attributes.push(attribute);
                    });

                    entityCollection.push(entity);
                });
            }
        }
    }

    /*<summary>Use esse método para obter uma referência para a exibição atual.</summary>
    <param name="_controlName" type="string" sample="subgrid_contatos" required="true"/>
    <return type="Xrm.LookupObject { id, name, entityType}"/>*/
    GetCurrentView(_controlName) {
        if (this.SmartHelper._controls.InForm(_controlName)) {
            if (this.SmartHelper._controls.GetControlType(_controlName) === "subgrid") {
                var subgrid = this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName);
                if (subgrid.getViewSelector() !== null)
                    return this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).getViewSelector().getCurrentView();
            }
        }
    }

    /*<summary>Use esse método para definir a exibição atual.</summary>
    <param name="_controlName" type="string" sample="subgrid_contatos" required="true"/>
    <param name="_view" type="Xrm.LookupObject" sample="var view = { entityType: 1039, id: "{00000000-0000-0000-0000-000000000000}", name: "Contacts Contoso BR" required="true"/>*/
    SetCurrentView(_controlName, _view) {
        if (this.SmartHelper._controls.InForm(_controlName)) {
            if (this.SmartHelper._controls.GetControlType(_controlName) === "subgrid") {
                var subgrid = this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName);
                if (subgrid.getViewSelector() !== null) {
                    if (_view !== null)
                        this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).getViewSelector().setCurrentView(_view);
                }
            }
        }
    }

    /*<summary>Adiciona uma função a um subgrid, sempre que o mesmo for atualizado a função será executada</summary>
    <param name="_controlName" type="string" sample="subgrid_contatos" required="true"/>*/
    Refresh(_controlName) {
        if (this.SmartHelper._controls.InForm(_controlName)) {
            if (this.SmartHelper._controls.GetControlType(_controlName) === "subgrid")
                this.SmartHelper.ExecutionContext.getFormContext().getControl(_controlName).refresh();
        }
    }
}
class _Common {
    constructor(_SmartHelper) {
        this.SmartHelper = _SmartHelper;
    }
    OnlyNumbers(_string) {
        var return_ = "";
        var regex = /[0-9]/g;
        var result = _string.match(regex);
        if (result !== null) {
            for (var i = 0; i < result.length; i++) {
                return_ += result[i];
            }
        }
        return return_;
    }
    FormatCPF(_cpf) {

        var cpf = new Object();
        cpf.success = false;
        cpf.message = "";
        cpf.formatedValue = "";

        if (_cpf !== null) {

            _cpf = this.OnlyNumbers(_cpf);

            if (_cpf.length === 11) {

                var Soma;
                var Resto;
                Soma = 0;
                if (_cpf === "00000000000"
                    || _cpf === "11111111111"
                    || _cpf === "22222222222"
                    || _cpf === "33333333333"
                    || _cpf === "44444444444"
                    || _cpf === "55555555555"
                    || _cpf === "66666666666"
                    || _cpf === "77777777777"
                    || _cpf === "88888888888"
                    || _cpf === "99999999999") {
                    cpf.success = false;
                    cpf.message = "CPF Inválido";
                }
                else {
                    for (var i = 1; i <= 9; i++) Soma = Soma + parseInt(_cpf.substring(i - 1, i)) * (11 - i);
                    Resto = (Soma * 10) % 11;

                    if ((Resto === 10) || (Resto === 11)) Resto = 0;
                    if (Resto !== parseInt(_cpf.substring(9, 10))) {
                        cpf.success = false;
                        cpf.message = "CPF Inválido";
                        return cpf;
                    }

                    Soma = 0;
                    for (var i = 1; i <= 10; i++) Soma = Soma + parseInt(_cpf.substring(i - 1, i)) * (12 - i);
                    Resto = (Soma * 10) % 11;

                    if ((Resto === 10) || (Resto === 11)) Resto = 0;
                    if (Resto !== parseInt(_cpf.substring(10, 11))) {
                        cpf.success = false;
                        cpf.message = "CPF Inválido";
                        return cpf;
                    }

                    cpf.success = true;
                    cpf.formatedValue = _cpf.substring(0, 9) + "-" + _cpf.substring(9, 11);
                }
            }
            else {
                cpf.success = false;
                cpf.message = "CPF Inválido";
            }
        }
        else {
            cpf.success = true;
        }

        return cpf;
    }
    FormatCNPJ(_cnpj) {

        var cnpj = new Object();
        cnpj.success = false;
        cnpj.message = "";
        cnpj.formatedValue = "";

        if (_cnpj !== null) {

            _cnpj = this.OnlyNumbers(_cnpj);

            if (_cnpj.length === 14) {

                if (_cnpj === "00000000000000" ||
                    _cnpj === "11111111111111" ||
                    _cnpj === "22222222222222" ||
                    _cnpj === "33333333333333" ||
                    _cnpj === "44444444444444" ||
                    _cnpj === "55555555555555" ||
                    _cnpj === "66666666666666" ||
                    _cnpj === "77777777777777" ||
                    _cnpj === "88888888888888" ||
                    _cnpj === "99999999999999") {
                    cnpj.success = false;
                    cnpj.message = "CNPJ Inválido";
                }
                else {
                    // Valida DVs
                    var tamanho = _cnpj.length - 2
                    var numeros = _cnpj.substring(0, tamanho);
                    var digitos = _cnpj.substring(tamanho);
                    var soma = 0;
                    var pos = tamanho - 7;
                    for (var i = tamanho; i >= 1; i--) {
                        soma += numeros.charAt(tamanho - i) * pos--;
                        if (pos < 2)
                            pos = 9;
                    }
                    var resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                    if (resultado !== digitos.charAt(0)) {
                        cnpj.success = false;
                        cnpj.message = "CNPJ Inválido";
                        return cnpj;
                    }

                    tamanho = tamanho + 1;
                    numeros = _cnpj.substring(0, tamanho);
                    soma = 0;
                    pos = tamanho - 7;
                    for (var i = tamanho; i >= 1; i--) {
                        soma += numeros.charAt(tamanho - i) * pos--;
                        if (pos < 2)
                            pos = 9;
                    }
                    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
                    if (resultado !== digitos.charAt(1)) {
                        cnpj.success = false;
                        cnpj.message = "CNPJ Inválido";
                        return cnpj;
                    }

                    cnpj.success = true;
                    cnpj.formatedValue = _cnpj.substring(0, 12) + "-" + _cnpj.substring(12, 14);
                }
            }
            else {
                cnpj.success = false;
                cnpj.message = "CNPJ Inválido";
            }
        }
        else {
            cnpj.success = true;
        }

        return cnpj;
    }
    FormatCEP(_string) {
        var return_ = "";
        if (_string.length === 8) {
            return_ = _string.substring(0, 5) + "-" + _string.substring(5, 8);
        }
        return return_;
    }
    FormatCPFandCNPJ(_string) {
        var _return = "";
        var _string = this.OnlyNumbers(_string);

        if (_string.length === 8) {
            _return = this.FormatCPF(_string);
        }
        else if (_string.length === 14) {
            _return = this.FormatCNPJ(_string);
        }
        return _return;
    }
    FormatRG_SSP(_rg) {
        _rg = this.OnlyNumbers(_rg);
        if (_rg.length === 9) {
            _rg = _rg.replace(/[^\dX]/g, "")
            _rg = _rg.replace(/(\d{2})(\d)/, "$1.$2")
            _rg = _rg.replace(/(\d{3})(\d)/, "$1.$2")
            _rg = _rg.replace(/(\d{3})([\dX]{1,2})$/, "$1-$2")
        }
        return _rg
    }
    async GetOption(_key) {
        var results = await Xrm.WebApi.retrieveMultipleRecords("fly_opcaos", "?$select=fly_value&$filter=fly_key eq '"+ _key +"'").then(
            function success(results) {
                return results;
                
            },
            function (error) {
                return null;
            }
        );

        /*
        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Utility.getGlobalContext().getClientUrl() + "/api/data/v8.2/fly_opcaos?$select=fly_value&$filter=fly_key eq '" + _key + "'", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\",odata.maxpagesize=1");
        req.onreadystatechange = function () {
            if (this.readyState == 4) {
                req.onreadystatechange = null;
                if (this.status == 200) {
                    var results = JSON.parse(this.response);
                    for (var i = 0; i < results.value.length; i++) {
                        var fly_value = results.value[i]["fly_value"];
                        return_ = fly_value;
                    }
                }
                else
                    return_ = null;
            }
        };
        req.send();
        */
        // debugger;
        return results;
    }
    Compare(_entityref1, _entityref2) {
        if (_entityref1.constructor === Array && _entityref2.constructor === Array) {
            if (_entityref1[0].id === _entityref2[0].id) return true;
        }
        if (_entityref1 === _entityref2) return true;
        return false;
    }
    SetRequiredLevel(_level, ...theArg) {

        var formContext = this.SmartHelper.ExecutionContext.getFormContext();
        theArg.forEach(function (element) {
            if (formContext.getControl(element) !== null) {
                formContext.getAttribute(element).setRequiredLevel(_level);
            }
        });
    }
    SetDisabled(_bool, ...theArg) {

        var formContext = this.SmartHelper.ExecutionContext.getFormContext();
        theArg.forEach(function (element) {
            if (formContext.getControl(element) !== null) {
                formContext.getControl(element).setDisabled(_bool);
            }
        });
    }
    SetVisible(_bool, ...theArg) {

        var formContext = this.SmartHelper.ExecutionContext.getFormContext();
        theArg.forEach(function (element) {
            if (formContext.getControl(element) !== null) {
                formContext.getControl(element).setVisible(_bool);
            }
        });
    }
    CallAction(_entityName, _actionName, _entityId) {

        var formContext = this.SmartHelper.ExecutionContext.getFormContext();
        var result = { isFault: false, data: null, erro: null };
        _entityName.substr(entityLogicalName.length - 1 === "s") ? entityLogicalName += "es" : entityLogicalName += "s"

        var req = new XMLHttpRequest();
        req.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() + "" + _entityName + "(" + _entityId + ")/Microsoft.Dynamics.CRM." + _actionName + "", true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 204) {
                    //Success
                } else {
                    result.isFault = true;
                    result.erro = this.statusText;
                }
            }
        };
        req.send();
        return result;
    }
    CallActionWithParameters(entityName, actionName, entityId, parameters) {

        var formContext = this.SmartHelper.ExecutionContext.getFormContext();
        var result = { isFault: false, data: null, erro: null };

        _entityName.substr(entityLogicalName.length - 1 === "s") ? entityLogicalName += "es" : entityLogicalName += "s";

        
        var req = new XMLHttpRequest();
        req.open("POST", Xrm.Utility.getGlobalContext().getClientUrl() + "" + _entityName + "(" + _entityId + ")/Microsoft.Dynamics.CRM." + _actionName + "", true);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 204) {
                    //Success
                } else {
                    result.isFault = true;
                    result.erro = this.statusText;
                }
            }
        };
        req.send(JSON.stringify(parameters));
        
        return result;
    }
    UserXray(_userId, _roles, _teams, _businessUnit, _manager, _position) {
        var results = [];

        if (_roles === true) {
            results[0] = this.GetRole(_userId);
        }
        if (_businessUnit === true) {
            results[1] = this.GetBusinessUnit(_userId);
        }
        if (_teams === true) {
            results[2] = this.GetTeams(_userId);
        }
        if (_manager === true) {
            results[3] = this.GetManager(_userId);
        }
        if (_position === true) {
            results[4] = this.GetPosition(_userId)
        }

        return results;
    }
    async GetBusinessUnit(_userId) {
        var result = await Xrm.WebApi.retrieveRecord("systemuser", _userId, "?$select=_businessunitid_value").then(
            function success(result) {
                // console.log(result);
                // Columns
                var systemuserid = result["systemuserid"]; // Guid
                var businessunitid = result["_businessunitid_value"]; // Lookup
                var businessunitid_formatted = result["_businessunitid_value@OData.Community.Display.V1.FormattedValue"];
                var businessunitid_lookuplogicalname = result["_businessunitid_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
                var temp = { isFault: false, data: [], erro: null };
                temp.data = {
                    id: businessunitid,
                    name: businessunitid_formatted,
                    logicalName: businessunitid_lookuplogicalname
                };
                return temp;
            },
            function (error) {
                var temp = { isFault: false, data: [], erro: null };
                result.isFault = true;
                result.erro = error.message;
                return temp;
            }
        );
        /*
        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/systemusers(" + _userId + ")?$select=_businessunitid_value", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var response = JSON.parse(this.response);
                    result.data = {
                        id: response["_businessunitid_value"],
                        name: response["_businessunitid_value@OData.Community.Display.V1.FormattedValue"],
                        logicalName: response["_businessunitid_value@Microsoft.Dynamics.CRM.lookuplogicalname"]
                    };
                } else {
                    result.isFault = true;
                    result.erro = this.statusText;
                }
            }
        };
        req.send();
        */
        return result;
    }
    async GetRole(_idUser) {
        var context = Xrm.Utility.getGlobalContext();
        var url = context.getClientUrl();
        var originalFetchXML = `<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='true'>
	                            <entity name='role'>
		                            <attribute name='name' />
		                            <order attribute='name' descending='false' />
		                            <link-entity name='systemuserroles' from='roleid' to='roleid' visible='false' intersect='true'>
			                            <link-entity name='systemuser' from='systemuserid' to='systemuserid' alias='ab'>
				                            <filter type='and'>
					                            <condition attribute='systemuserid' operator='eq' value='{`+ _idUser +`}' />
				                            </filter>
			                            </link-entity>
		                            </link-entity>
	                            </entity>
                            </fetch>";`;
        var escapedFetchXML = encodeURIComponent(originalFetchXML);

        var result =  await Xrm.WebApi.retrieveMultipleRecords("role", "?fetchXml=" + escapedFetchXML).then(
            function success(_results) {
                var temp = { isFault: false, data: [], erro: null };
                temp.data = [];
                for (var i = 0; i < _results.length; i++) {
                    temp.data[i] = _result.entities[i]["name"];
                }
                return temp;
            },
            function (error) {
                var temp = { isFault: false, data: [], erro: null };
                temp.isFault = true;
                temp.erro = error.message;
                return temp;
            }
        );
        /*
        var urlfilter = "roles?fetchXml=<fetch%20version%3D'1.0'%20output-format%3D'xml-platform'%20mapping%3D'logical'%20distinct%3D'true'><entity%20name%3D'role'><attribute%20name%3D'name'%20%2F><order%20attribute%3D'name'%20descending%3D'false'%20%2F><link-entity%20name%3D'systemuserroles'%20from%3D'roleid'%20to%3D'roleid'%20visible%3D'false'%20intersect%3D'true'><link-entity%20name%3D'systemuser'%20from%3D'systemuserid'%20to%3D'systemuserid'%20alias%3D'ab'><filter%20type%3D'and'><condition%20attribute%3D'systemuserid'%20operator%3D'eq'%20value%3D'%7B" + _idUser + "%7D'%20%2F><%2Ffilter><%2Flink-entity><%2Flink-entity><%2Fentity><%2Ffetch>";

        var result = { isFault: false, data: [], erro: null };

        var req = new XMLHttpRequest();
        req.open("GET", url + "/api/data/v9.0/" + urlfilter, false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    var _result = JSON.parse(this.response);
                    result.data = [];
                    for (var i = 0; i < _result.value.length; i++) {
                        result.data[i] = _result.value[i]["name"];
                    }
                } else {
                    result.isFault = true;
                    result.erro = this.statusText;
                }
            }
        };
        req.send();
        */
        //Caso nao encontre pela busca direta nos direitos de acesso do usuario
        if (result.isFault === true) {
            var currentUserRoles = context.userSettings.securityRoles;
            var urlfilter = "roleid";
            urlfilter += "eq " + currentUserRoles[0];

            if (currentUserRoles.length > 1) {
                for (var i = 1; i < currentUserRoles.length; i++) {

                    if (!urlfilter.includes(currentUserRoles[i]))
                        urlfilter += " or roleid eq " + currentUserRoles[i];
                }
            }
            result = null;
            result = await Xrm.WebApi.retrieveMultipleRecords("role", "?$select=name&$filter=("+ urlfilter +")").then(
                function success(results) {
                    var temp = { isFault: false, data: [], erro: null };
                    temp.data = [];
                    for (var i = 0; i < _results.length; i++) {
                        temp.data[i] = _result.entities[i]["name"];
                    }
                    return temp;
                },
                function (error) {
                    var temp = { isFault: false, data: [], erro: null };
                    temp.isFault = true;
                    temp.erro = error.message;
                    return temp;
                }
            );
            /*
            var req = new XMLHttpRequest();
            req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.0/" + urlfilter, false);
            req.setRequestHeader("OData-MaxVersion", "4.0");
            req.setRequestHeader("OData-Version", "4.0");
            req.setRequestHeader("Accept", "application/json");
            req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
            req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
            req.onreadystatechange = function () {
                if (this.readyState === 4) {
                    req.onreadystatechange = null;
                    if (this.status === 200) {
                        var _result = JSON.parse(this.response);
                        result.data = [];
                        for (var i = 0; i < _result.value.length; i++) {
                            result.data[i] = _result.value[i]["name"];
                        }
                    } else {
                        result.isFault = true;
                        result.erro = this.statusText;
                    }
                }
            };
            req.send();
            */
        }

        return result;
    }
    async GetTeams(_userId) {

        var results = await Xrm.WebApi.retrieveMultipleRecords("teammembership", "?$select=systemuserid,teamid&$filter=systemuserid eq aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa").then(
            function success(results) {
                return results;                
            },
            function (error) {
                return null;
            }
        );

        /*
        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/teammemberships?$select=systemuserid,teamid&$filter=systemuserid eq " + _userId, false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    results = JSON.parse(this.response);
                    for (var i = 0; i < results.value.length; i++) {
                        var systemuserid = results.value[i]["systemuserid"];
                        var teamid = results.value[i]["teamid"];
                    }
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();
        */
        return results;
    }
    async GetManager(_userId) {
        var result = await Xrm.WebApi.retrieveRecord("systemuser", _userId, "?$select=_positionid_value,_parentsystemuserid_value").then(
            function success(result) {
                return result;                
            },
            function (error) {
                return null;
            }
        );
        /*
        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/systemusers(" + _userId + ")?$select=_parentsystemuserid_value", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    result = JSON.parse(this.response);
                    var _parentsystemuserid_value = result["_parentsystemuserid_value"];
                    var _parentsystemuserid_value_formatted = result["_parentsystemuserid_value@OData.Community.Display.V1.FormattedValue"];
                    var _parentsystemuserid_value_lookuplogicalname = result["_parentsystemuserid_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();
        */
        return result;
    }
    async GetPosition(_userId) {
        var result = await Xrm.WebApi.retrieveRecord("systemuser", _userId, "?$select=_positionid_value").then(
            function success(result) {
                return result;                
            },
            function (error) {
                return null;
            }
        );
        /*
        var req = new XMLHttpRequest();
        req.open("GET", Xrm.Page.context.getClientUrl() + "/api/data/v9.1/systemusers(" + _userId + ")?$select=_positionid_value", false);
        req.setRequestHeader("OData-MaxVersion", "4.0");
        req.setRequestHeader("OData-Version", "4.0");
        req.setRequestHeader("Accept", "application/json");
        req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
        req.setRequestHeader("Prefer", "odata.include-annotations=\"*\"");
        req.onreadystatechange = function () {
            if (this.readyState === 4) {
                req.onreadystatechange = null;
                if (this.status === 200) {
                    result = JSON.parse(this.response);
                    var _positionid_value = result["_positionid_value"];
                    var _positionid_value_formatted = result["_positionid_value@OData.Community.Display.V1.FormattedValue"];
                    var _positionid_value_lookuplogicalname = result["_positionid_value@Microsoft.Dynamics.CRM.lookuplogicalname"];
                } else {
                    Xrm.Utility.alertDialog(this.statusText);
                }
            }
        };
        req.send();
        */
        return result;
    }
}
class _Utilities {
    constructor() {
        this.GetClientUrl = Xrm.Utility.getGlobalContext().getClientUrl();
        this.GetAllowedStatusTransitions = Xrm.Utility.closeProgressIndicator();
        this.GetLearningPathAttributeName = Xrm.Utility.getLearningPathAttributeName();
    }

    /*<summary>Retorna uma string cadastrada no RESX conforme a linguagem do usuario logado.<summary>
      <param name="_webResource" type="string" sample="smt_messages" required="true" />
      <param name="_messageKey" type="string" sample="hello" required="true" />*/
    GetResourceString(_webResource, _messageKey) {
        return Xrm.Utility.getResourceString(_webResource, _messageKey);
    }
}