   function onDocReadyEditOpt() {
      var type = $('#Type').val();
      //debugger;

      $('#Value').keydown(function(e){
         //console.log(2)
         if ($('#Type').val() == "2") {

            var keyPressed;
            if (!e) var e = window.event;
            if (e.keyCode) keyPressed = e.keyCode;
            else if (e.which) keyPressed = e.which;
            var hasDecimalPoint = true;//(($(this).val().split('.').length-1)>0);
            if ( keyPressed == 46 || keyPressed == 8 ||((keyPressed == 190||keyPressed == 110)&&(!hasDecimalPoint && !e.shiftKey)) || keyPressed == 9 || keyPressed == 27 || keyPressed == 13 ||
               // Allow: Ctrl+A
               (keyPressed == 65 && e.ctrlKey === true) ||
               // Allow: home, end, left, right
               (keyPressed >= 35 && keyPressed <= 39)) {
               // let it happen, don't do anything
               return;
            }
            else {
               
               //if(e.currentTarget.value.length === 0 && e.keyCode === 189){
               //   return true;
               //}
               // Ensure that it is a number and stop the keypress
               if (e.shiftKey || (keyPressed < 48 || keyPressed > 57) && (keyPressed < 96 || keyPressed > 105 )) {
                  //debugger;
                  // If this is the first character and it is a dash, that's okay
                  // Allow: minus
                  if (keyPressed !== 109 && keyPressed !== 189) {
                     e.preventDefault();
                  }
                  
               }
            }
         }
         

      });

      $('#ValueBool').kendoDropDownList({
         dataTextField: "text",
         dataValueField: "value",
         valuePrimitive: true,
         dataSource: [
            { text: dicsvcXlateFalse, value: "0" },
            { text: dicsvcXlateTrue, value: "1" }
         ]
      }).getKendoDropDownList().wrapper.hide();

      simpleChoise = $('#ListItems').val();
      try {
         var x = $.parseXML(simpleChoise).documentElement;
         simpleChoise = xml2json(x);
      } catch (e) {
         if (type == "1") {
            
            console.error(simpleChoise, dicsvcXlateNotAValidXml);
         }

      }
      dsList = new kendo.data.DataSource({
         //type: 'json',
         data: simpleChoise,
         batch: false,

         schema: {
            id: 'Key',
            model: {
               fields: {
                  Key:{ validation: { required: true } },
                  Label: {},
                  Value: {}
               }
            },
            parse: function(response) {
               var choices = [];
               for (var i = 0; i < response.length; i++) {
                  if (response[i].Key) {
                     var choice = {
                        Key: response[i].Key,
                        Label: response[i].Label ? response[i].Label:'',
                        Value: response[i].Value ? response[i].Value : ''
                     };
                     choices.push(choice);
                  }

               }
               return choices;
            }
         },
         change: function(e) {
            console.log(e.action);
            if (e.action === "itemchange" || e.action === "remove") {
               try {
                  var xml = json2xml(this.data().toJSON(), undefined, "Choice", "Choices");
                  if ($.parseXML(xml).documentElement) {
                     $('#ListItems').val(xml);
                  }
               } catch (e) {
                  //debugger;
               }
            }
            //if (e.action === "add") {
            //}
            //if (e.action === "remove") {
               
            //}
            //debugger;
         }
      });


      $('#ValueList').kendoDropDownList({
         dataTextField: "Label",
         dataValueField: "Key",
         dataSource: dsList
      }).getKendoDropDownList().wrapper.hide();

      $('#ListItemsGrid').kendoGrid({
         dataSource: dsList,
         selectable: true,
         editable: {
            "mode":"popup",
            "createAt": "bottom"
         },
         columns: [
            { field: "Key" },
            { field: "Label" },
            { field: "Value" },
            { command: [{ name: "edit", text: " " }], width: 40 }
         ],
         height: 150,

         save: function(e) {
            if (e.model.Key && e.model.Key !== "") {
               if (e.sender.dataSource.view().filter(x => { return x.Key === e.model.Key && x.uid != e.model.uid }).length > 0) {
                  e.preventDefault();
                  kendo.alert(msgDuplicateKey || 'duplicate key');
                  return;
               }
            }
            //debugger;

         }
         //,
         ,
         cancel: function(e) {
            //debugger;
            e.preventDefault();
            var grid = $("#ListItemsGrid").data("kendoGrid");
            grid.dataSource.read();
            e.container.data("kendoWindow").close();
         }
         //cancel: function(e) {
         //   e.preventDefault();
         //   e.container.data("kendoWindow").close();
         //   try {
         //      var grid = $("#ListItemsGrid").data("kendoGrid");
         //      grid.dataSource.read();

         //   } catch (e) {

         //   }

         //}
      });
      $('#Limits').hide();
      $('#ListEdit').hide();


      if (type === "2" || type === "3") {
         $('#Limits').show();
      }
      if (type === "1") {
         $('#ListEdit').show();
      }
      $('#Type').attr('previous', "0");

      document.getElementById('optName').readOnly = (typeOfEdit == 0);
      setAceEditorForOption();
      setValueType(type);
   }
   function onTypeChange(e) {
      var type = $('#Type').val();
      var prevType = $('#Type').attr('previous');

      if (type !== "2" && type !== "3") {
         $('#Limits').hide();
      } else {
         $('#Limits').show();
      }
      if (type !== "1") {
         $('#ListEdit').hide();
      } else {
         $('#ListEdit').show();
      }

      clearType(prevType);

      setValueType(type);

   }
   function clearType(prevType) {
      switch (prevType) {
      case "0":/*Text*/
         $('#Value').hide();
         break;
      case "1":/*List*/
         $('#Value')[0].type = "text";

         $('#ValueList').getKendoDropDownList().wrapper.hide();

         break;
      case "2":/*Integer*/
         $('#Value').hide();
         break;
      case "3":/*Decimal*/
         $('#Value').hide();
         break;
      case "4":/*XML*/
         $('#ValueXml').hide();
         break;
      case "5":/*Boolean*/
         $('#ValueBool').getKendoDropDownList().wrapper.hide();
         break;

      default:
         break;
      }
   }

   function setValueType(type) {
      //debugger;
      $('#Type').attr('previous', type);
      switch (type) {
      case "0":
         $('#Value')[0].type = "text"; 
         //$('#Value')[0].pattern = "";
         $('#Value')[0].step = ""; 
         $('#Value').show();
         break;
      case "1":

         $('#Value')[0].type = "text";
         $('#ValueList').getKendoDropDownList().wrapper.show();
         break;
      case "2":
         $('#Value')[0].type = "number";    
         $('#Value')[0].step = "1";   
         //$('#Value')[0].pattern = "\\d*";
         $('#Value').val(Math.floor($('#Value').val()));
         $('#Value').show();
         var a = $('#MinLimit').getKendoNumericTextBox();
         var v = a.value();
         a.setOptions({ decimals: 0, step: 1 , format:"n0"});
         a.value(v);
         var b = $('#MaxLimit').getKendoNumericTextBox();
         v = b.value();
         b.setOptions({ decimals: 0, step: 1 , format:"n0"});
         b.value(v);
         break;
      case "3":
         $('#Value')[0].type = "number"; 
         $('#Value')[0].step = ".1";  
         //$('#Value')[0].pattern = "";
         $('#Value').show();
         var x = $('#MinLimit').getKendoNumericTextBox();
         var v = x.value();
         x.setOptions({ decimals: 5, step: 0.1, format: "n5" });
         x.value(v);
         var y = $('#MaxLimit').getKendoNumericTextBox();
         v = y.value();
         y.setOptions({ decimals: 5, step: 0.1, format: "n5" });
         y.value(v);
         break;
      case "4":/*XML*/
         $('#Value')[0].type = "text";
         $('#ValueXml').show();
         break;
      case "5":/*Boolean*/

         var val = $('#Value').val().toBool() ? 1 : 0;
         $('#Value').val(val);

         $('#ValueBool').getKendoDropDownList().wrapper.show();
         $('#ValueBool').getKendoDropDownList().value(val);
         break;

      default:
         break;
      }
   }
   

   function onClickListItemsScriptNew() {
      var grid = $("#ListItemsGrid").data("kendoGrid");
      grid.addRow();
      //console.log($('[data-bind="value:ListItems"]').val())
      //debugger;
   }
   function onClickListItemsScriptDel() {

      var grid = $("#ListItemsGrid").data("kendoGrid");
      var row = grid.select();
      if (row) {
         var data = grid.dataItem(row);
         grid.removeRow(row);
      }
      //console.log($('[data-bind="value:ListItems"]').val())
      //debugger;

   }