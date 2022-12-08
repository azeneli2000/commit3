function fwebGetTranslatedString(key) {
   var translations = gdicTransaltion || [];
   //Il problema dell'api è la chiave che è escapata
   //translations['search'] = '<%=SXLFormsJS("Search...") %>';

   if (!translations[key]) {
      
      //se c'è il learing mode' +
      //   'allora api'
      //key
      ////throw('Translation for ' + key + ' does not exists');
      ////chiamata api
      //altrimenti aggiungo 
      ////agggiungo 
      return key;
   }
   return translations[key];
}



//override custom filters for all kendo grids 

kendo.ui.FilterMenu.prototype.options.operators =
    $.extend(kendo.ui.FilterMenu.prototype.options.operators, {

        /* FILTER MENU OPERATORS (for each supported data type) 
         ****************************************************************************/
        string: {
            contains: "Contains",
            startswith: "Starts with",
            eq: "Is equal to",
            neq: "Is not equal to",
            doesnotcontain: "Does not contain",
            endswith: "Ends with"
        },
        number: {
            eq: "Is equal to",
            neq: "Is not equal to",
            gte: "Is greater than or equal to",
            gt: "Is greater than",
            lte: "Is less than or equal to",
            lt: "Is less than"
        },
        date: {
            eq: "Is equal to",
            neq: "Is not equal to",
            gte: "Is after or equal to",
            gt: "Is after",
            lte: "Is before or equal to",
            lt: "Is before"
        },
        enums: {
            eq: "Is equal to",
            neq: "Is not equal to"
        }
        /***************************************************************************/
    });

function umsDistinct(value, index, self) {
    return self.indexOf(value) === index;
}

function buildValidationMessage(errors) {
    var errorsMsg = "<div class='row h-100'>";
    var numitems = 0;
    //var _err = Array.from(new Set(errors));
    var _err = errors.filter(umsDistinct);
    $(_err).each(function () {
        if ($.trim(this) !== "") {
            if (numitems % 2 === 0) {
                if (numitems !== 0) 
                    errorsMsg += "</div>";
                errorsMsg += "<div class='col-auto border-right border-danger'>";
            }
            //    errorsMsg = errorsMsg + "<span class=\"error-icon glyphicon glyphicon-exclamation-sign\"></span><span>" + this + "</span>";
            //}
            //else {
                //errorsMsg = errorsMsg + "<div class='mr-auto px-2 border-right border-danger'><span class=\" mdi mdi-alert-outline\"></span>" + this + "</div>";
                errorsMsg = errorsMsg + "<li class='text-truncate'>" + this + "</li>";
            //}
            
            numitems = numitems + 1;
        }
    });
    errorsMsg = errorsMsg + "</div></div>";
    return errorsMsg;
}

function buildWarningMessage(warnings) {
   var errorsMsg = "<div class='row h-100'>";
   var numitems = 0;
   var _err = warnings.filter(umsDistinct);
   $(_err).each(function () {
      if ($.trim(this) !== "") {
         if (numitems % 2 === 0) {
            if (numitems !== 0) 
               errorsMsg += "</div>";
            errorsMsg += "<div class='col-auto border-right border-warning'>";
         }
         errorsMsg = errorsMsg + "<li class='text-truncate text-warning'>" + this + "</li>";
            
         numitems = numitems + 1;
      }
   });
   errorsMsg = errorsMsg + "</div></div>";
   return errorsMsg;
}


function getGridRowIndex(gridSelctor, dataItem) {
   var data = $(gridSelctor).data("kendoGrid").dataSource.data();
   return data.indexOf(dataItem);
}

function umsConfirm(title, msg, onOk, onCancel) {
    //debugger;
    if ($('#umsConfirmDialogDIV').length === 0) {
        var dialog = $('<div id="umsConfirmDialogDIV"></div>');
        $('body').append(dialog);
    }
    else {
        dialog = $('#umsConfirmDialogDIV');
    }
    if (dialog.data('kendoDialog')) {
        dialog.data('kendoDialog').destroy();
    } 
    dialog.kendoDialog({
        width: "400px",
        minWidth: "400px",
        title: title,
        closable: false,
        modal: true,
        content: msg,
        actions: [
            { text: '<span id=\'btnConfDialogOK\'>OK</span>', action: onOk },
            { text: '<span id=\'btnConfDialogCancel\'>Cancel</span>', action: onCancel }
        ],
        close: function (event, ui) {
            //$(this).dialog("close");
            //$(this).remove();
        }
    });
    
    
    dialog.parent().find('.k-button').addClass('k-buttonLarge'); //.css("width", "50%", "important");
    if (/MSIE (\d+\.\d+);/.test(navigator.userAgent) || navigator.userAgent.indexOf("Trident/") > -1) {
        dialog.parent().find('.k-button').attr('style', function (i, s) { return s + 'width: 50% !important;'; });
    }
    dialog.data('kendoDialog').open();
}
 
