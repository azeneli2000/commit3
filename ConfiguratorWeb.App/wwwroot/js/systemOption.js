//script for _SystemOption

function removeUserFromSysOpt() {
    $('#UserName').val('');
    $('#UserFullName').val('');
}

function changeToEditMode() {
    actualmode = 2;
    $(".modify-mode").show();
    $(".view-mode").hide();
    toolBarAllEditSaveButton();
    toolBarShowButtonByClass("modify-mode");
    var $input = $('#' + strDialogID +'   :input');
    $input.attr('disabled', false);
    $input.each(function () {
        var cb = $(this).data("kendoDropDownList");
        if (cb) {
            cb.enable(true);
        }
    });
    $input.each(function () {
        var cb = $(this).data("kendoNumericTextBox");
        if (cb) {
            cb.enable(true);
        }
    });
    $input.each(function () {
        var cb = $(this).data("kendoTimePicker");
        if (cb) {
            cb.enable(true);
        }
    });
    if (editor) {
        editor.setOptions({ readOnly: false });
    }
    if ($('#GUID').val() == '') {
       $('#btnDelete').hide();
    }

}

function changeToViewMode() {
    actualmode = 1;
    $(".view-mode").show();
    $(".modify-mode").hide();
    toolBarAllEditSaveButton();
    toolBarShowButtonByClass("view-mode");
    var $input = $('#' + strDialogID +'   :input');
    $input.attr('disabled', true);
    $input.each(function () {
        var cb = $(this).data("kendoDropDownList");
        if (cb) {
            cb.enable(false);
        }
    });
    $input.each(function () {
        var cb = $(this).data("kendoNumericTextBox");
        if (cb) {
            cb.enable(false);
        }
    });
    $input.each(function () {
        var cb = $(this).data("kendoTimePicker");
        if (cb) {
            cb.enable(false);
        }
    });
    if (editor) {
        editor.setOptions({ readOnly: true });
        $('.ace_text-input').attr('disabled', false);
    }
}

var actualmode;
function setWindowMode() {
    if ('@Model.GUID' !== '') {
        actualmode = 1;
    } else {
        actualmode = 2;
    }
    setMode();
}
function setMode() {
    if (actualmode === 2)
        changeToEditMode();
    else
        changeToViewMode();
    dialog.center().resize();
}
function toolBarShowButtonByClass(clas) {
    var tb = $("#toolbarDetail").data("kendoToolBar");
    tb.umsShowButtonByClass(clas);
}
function toolBarAllEditSaveButton() {
    var tb = $("#toolbarDetail").data("kendoToolBar");
    tb.umsHideAllEditSaveButton();
}
var dialog = $("#SystemOptionEditWindow").data("kendoWindow");
dialog.bind("activate", setMode);
dialog.bind("resize", setMode);
$(document).ready(function () {
   toolBarAllEditSaveButton();
    setWindowMode();
    $('.btnEdit').click(function () {
        changeToEditMode();
    })
    $('.btnCancel').click(function () {
        var currentID = $('#GUID').val();
        if (currentID && currentID != '') {
            openDetail({ GUID: $('#GUID').val() });
        }
        else {
            dialog.close();
        }

    })
    $('.btnClose').click(function () {
        dialog.close();
        $(this).closest(".k-state-focused").focus();
    })
    $('#btnCopy').click(function () {
        //reset guid
        $("#GUID").val("");
        changeToEditMode();
    });
    $('#btnDelete').click(function () {
        umsConfirm(confirmDelTitle, confirmDelBody, deleteSysOpt, cancelDeleteSysOpt);
    });
    $('#btnSave').off('click').on('click', function () {
       //debugger;
        if (editor && !$('#aceEditor').hasClass('d-none')) {
            var textarea = $(textareaIdValueModelGuid).hide();
            textarea.val(editor.getSession().getValue());
        }
        //if (OnValidation()) {
        $("#form_"+strDialogID).submit();
        //}
    });

});



function cancelDeleteSysOpt() {

    /* do nothing */
}


function OnSuccess(response) {
    //debugger;
    if (response.success) {
        var grid = $("#system-options-grid").data("kendoGrid");
        //var row = $("#system-options-grid .k-state-selected").closest("tr");
        grid.dataSource.read();
        //if (_guid && _guid !="") {
        //    findDataItem(grid,_guid);
        //}
        
        dialog.close();
    }
    else {
        umsErrorDialog("Error occured", response.errorMessage, 200, 200);
    }
}

function OnFailure(response) {
    //debugger;
    umsErrorDialog("Error occured", response.errorMessage, 200, 200);
}



var editor;
$(document).ready(function () {
    //debugger;
    
    if (modelType == 'Text') {
        ace.require("ace/ext/language_tools");
        editor = ace.edit('aceEditor');
        editor.setOptions({
            autoScrollEditorIntoView: true,
            copyWithEmptySelection: true,
            readOnly: true,
            displayIndentGuides: true,
            enableBasicAutocompletion: true,
            enableSnippets: true,
            enableLiveAutocompletion: false,
            printMargin: false
        });

        editor.setTheme("ace/theme/chrome");
        var textarea = $(textareaIdValueModelGuid).hide();
        editor.getSession().setValue(textarea.val());
        editor.getSession().on('change', function () {
            textarea.val(editor.getSession().getValue());
        });
        // add command to lazy-load keybinding_menu extension
        editor.commands.addCommand({
            name: "showKeyboardShortcuts",
            bindKey: { win: "Ctrl-Alt-h", mac: "Command-Alt-h" },
            exec: function (editor) {
                ace.config.loadModule("ace/ext/keybinding_menu",
                    function (module) {
                        module.init(editor);
                        editor.showKeyboardShortcuts();
                    });
            }
        })
        editor.execCommand("showKeyboardShortcuts");
        if (document.getElementById('validateXml1') != null) {

            editor.session.setMode("ace/mode/xml");
            editor.getSession().setValue(textarea.val());
            editor.getSession().on('change',
                function () {
                    textarea.val(editor.getSession().getValue());
                });
            $('#aceEditor').removeClass('d-none');
        } else {

            
            var valore = $(textareaIdValueModelGuid)["0"].value;
            if (valore.indexOf("function ") == 0 || valore.indexOf("SELECT ") == 0) {

                var codeMode = "ace/mode/vbscript";
                if (valore.indexOf("SELECT ") == 0)
                    codeMode = "ace/mode/sqlserver";


                editor.session.setMode(codeMode);


                $('#aceEditor').removeClass('d-none');
            } else {
                editor.session.setMode("ace/mode/text");
                $('#aceEditor').removeClass('d-none');
                //textarea.show();
            }
        }
        if (modelName=='SmartCentralConfig') {
            destroyEditor();
        }
    }
});

function createEditor() {
    if (modelType == 'Text') {
        var textarea = $(textareaIdValueModelGuid).hide();
        editor.getSession().setValue(textarea.val());
        $('#aceEditor').removeClass('d-none');
        $('#viewOnEditor').hide();
        $('#viewOnText').show();
    }
};
function destroyEditor(){
    if (modelType == 'Text') {
        var textarea = $(textareaIdValueModelGuid);
        textarea.val(editor.getSession().getValue());
        textarea.show();
        $('#aceEditor').addClass('d-none');
        $('#viewOnEditor').show();
        $('#viewOnText').hide();

    }
};
function validateXml() {
    var text2validate = $(textareaIdValueModelGuid)["0"].value;

    var xml = text2validate; //.replaceAll( "&amp;","&").replaceAll("&", "&amp;");
    var oParser = new DOMParser();
    var oDOM = oParser.parseFromString(xml, "text/xml");
    if (oDOM.getElementsByTagName('parsererror').length > 0) {
        
        umsAlert(xmlValidationTitle, (new XMLSerializer()).serializeToString(oDOM));
    } else {
        //debugger;
        
        $.ajax({
            url: urlActionTrytoserializexmlSystemconfiguration,
            type: 'POST',
            data: { webString: xml, specificClass: modelName },
            async: true

        }).done(function (data) {
            if (data.success) {
                
                umsAlert(xmlValidationTitle, xmlIsCorrectlyValidated);
            } else {
                umsErrorDialog("Error occured", data.errorMessage, 200, 200);
            }
        }).fail(function (response) {
            umsErrorDialog("Error occured", response.errorMessage, 200, 200);
        });

    }
}

function getDefaultXml() {
    var text2validate = $(textareaIdValueModelGuid)["0"].value;

    
    $.ajax({
        url: urlActionGetDefaultXmlSystemconfiguration,
        type: 'POST',
        data: { specificClass: modelName },
        async: true

    }).done(function (data) {
        if (data.success) {
            editor.setValue( data.value);
            text2validate = editor.getSession().getValue();
        } else {
            umsErrorDialog("Error occured", data.errorMessage, 200, 200);
        }
    }).fail(function (response) {
        umsErrorDialog("Error occured", response.errorMessage, 200, 200);
    });

}

