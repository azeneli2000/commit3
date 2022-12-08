function reloadDriverChildrenValues(elementkey, setDefaults, requestUrl) {

   $.ajax({
      url: requestUrl,
      type: 'GET',
      dataType: "json",
      success: function (data, textStatus, jQxhr) {
         refreshDriverChildrenValues(elementkey, setDefaults, data);
      },
      error: function (jqXhr, textStatus, thrownError) {
         $("#errors").html("An error occurred while retriving driver defaults (" + targetUrl + "): " + jqXhr.status + ' ' + thrownError);
      }
   });

}

function refreshDriverChildrenValues(elementkey, setDefaults, values) {

   var driverTypeDropdownlist = $("#DriverType_" + elementkey).data("kendoDropDownList");
   var connectionTypedropdownlist = $("#ConnectionType_" + elementkey).data("kendoDropDownList");
   var alarmSystemTypeDropdownlist = $("#AlarmSystemType_" + elementkey).data("kendoDropDownList");


   var socketTypedropdownlist = $("#SocketType_" + elementkey).data("kendoDropDownList");

   var stopBitsDropdownlist = $("#SerialPort_StopBits").data("kendoDropDownList");
   var baudDropdownlist = $("#SerialPort_BitsPerSeconds").data("kendoDropDownList");
   var handshakeDropdownlist = $("#SerialPort_Handshake").data("kendoDropDownList");
   var parityDropdownlist = $("#SerialPort_Parity").data("kendoDropDownList");
   var dataModeDropdownlist = $("#SerialPort_DataModeId").data("kendoDropDownList");
   var dataBitsDropdownlist = $("#SerialPort_DataBits").data("kendoDropDownList");


   var customParamenterGrid = $("#CustomParameters").data("kendoGrid");

   driverTypeDropdownlist.setDataSource(values.SupportedDriverTypes);
   driverTypeDropdownlist.dataSource.read();
   //debugger;
   connectionTypedropdownlist.setDataSource(values.SupportedConnectionTypes);
   connectionTypedropdownlist.dataSource.read();
   //connectionTypedropdownlist.select(-1);

   alarmSystemTypeDropdownlist.setDataSource(values.SupportedAlarmSystemType);
   alarmSystemTypeDropdownlist.dataSource.read();

   //socketTypedropdownlist.setDataSource(values.SupportedSocketType);
   //socketTypedropdownlist.dataSource.read();
    
   if (setDefaults) {
      connectionTypedropdownlist.select(function (dataItem) {
         return dataItem.Value === values.DefaultConnectionType + "";
      });
      showConnectionTypeTabstrip(values.DefaultConnectionType + "");

      socketTypedropdownlist.select(function (dataItem) {
         return dataItem.Value === values.DefaultTCPCommType + "";
      });
      socketTypedropdownlist.trigger("change");
      
      alarmSystemTypeDropdownlist.select(0);;

      $("#Socket_HostName").val(values.DefaultHostname);

      $("#Socket_Port").data("kendoNumericTextBox").value(values.DefaultSocketPort);

      $('#SerialPort_SerialPort').data('kendoNumericTextBox').value(values.DefaultComPort);

      stopBitsDropdownlist.select(function (dataItem) {
         return dataItem.Value === values.DefaultStopBits + "";
      });
      stopBitsDropdownlist.trigger("change");
      if (defaultBitsPerSeconds != undefined ) {
         defaultBitsPerSeconds = values.DefaultBaud.toString();   
      }
      
      baudDropdownlist.select(function (dataItem) {
         return dataItem.Value === values.DefaultBaud + "";
      });
      
      if (defaultBitsPerSeconds != undefined && defaultBitsPerSeconds != "0" && baudDropdownlist.value()!=defaultBitsPerSeconds ) {
         if (onBitsPerSecondsDataBound != undefined) {
            onBitsPerSecondsDataBound();
         }
      }
      baudDropdownlist.trigger("change");

      handshakeDropdownlist.select(function (dataItem) {
         return dataItem.Value === values.DefaultHandShake + "";
      });
      handshakeDropdownlist.trigger("change");

      parityDropdownlist.select(function (dataItem) {
         return dataItem.Value === values.DefaultParity + "";
      });
      parityDropdownlist.trigger("change");

      dataModeDropdownlist.select(function (dataItem) {
          return dataItem.Value === values.ReceivingDataMode + "";
      });
      dataModeDropdownlist.trigger("change");

      dataBitsDropdownlist.select(function (dataItem) {
         return dataItem.Value === values.DefaultDataBits + "";
      });
      dataBitsDropdownlist.trigger("change");

      $("#SerialPort_SmartCableID").val(values.DefaultSmartCableId);

      var dataSource = new kendo.data.DataSource({
         data: values.DefaultCustomParameters
      });

      customParamenterGrid.setDataSource(dataSource);

   }
   else {
      //if (connectionTypedropdownlist.dataSource.data().length === 1) {
      //   connectionTypedropdownlist.select(0);
      //   connectionTypedropdownlist.trigger("change");
      //}

      var alarmVal = $('#storedAlarmSystemType').val();
      if (alarmSystemTypeDropdownlist.dataSource.data().length === 1) {
         alarmSystemTypeDropdownlist.select(0);
      } else {
         alarmSystemTypeDropdownlist.select(function (dataItem) {
            return dataItem.Value === alarmVal + "";
         });
      }
      
      alarmSystemTypeDropdownlist.trigger("change");
      if (socketTypedropdownlist.dataSource.data().length === 1) {
         socketTypedropdownlist.select(0);
         socketTypedropdownlist.trigger("change");
      }



      if (stopBitsDropdownlist.dataSource.data().length === 1) {
         stopBitsDropdownlist.select(0);
         stopBitsDropdownlist.trigger("change");
      }

      if (baudDropdownlist.dataSource.data().length === 1) {
         baudDropdownlist.select(0);
         baudDropdownlist.trigger("change");
      }

      if (handshakeDropdownlist.dataSource.data().length === 1) {
         handshakeDropdownlist.select(0);
         handshakeDropdownlist.trigger("change");
      }

      if (parityDropdownlist.dataSource.data().length === 1) {
         parityDropdownlist.select(0);
         parityDropdownlist.trigger("change");
      }

      //if (dataModeDropdownlist.dataSource.data().length === 1) {
      //   dataModeDropdownlist.select(0);
      //   dataModeDropdownlist.trigger("change");
      //}

      if (dataBitsDropdownlist.dataSource.data().length === 1) {
         dataBitsDropdownlist.select(0);
         dataBitsDropdownlist.trigger("change");
      }

   }

   if (driverTypeDropdownlist.dataSource.data().length === 1) {
      driverTypeDropdownlist.select(0);
   } else {
      driverTypeDropdownlist.select(function (dataItem) {
         return dataItem.Value === $('#storedDriverType').val()+ "";
      });
   }
   driverTypeDropdownlist.trigger("change");

}