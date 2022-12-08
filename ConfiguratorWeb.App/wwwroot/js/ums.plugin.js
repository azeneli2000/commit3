(function ( $ ) {
 
    function _umsKendowindow (_this, options ) {
 
        // This is the easiest way to have default options.
        var settings = $.extend({
            width: "80%",
            height: "90%",
            minHeight: 200,
            minWidth: 200,
            modal: true,
            resizable: false,
            scrollable:false,
            visible: false,
            title: 'title',
            draggable: false,
            animation: false
        }, options );
        var a = _this.kendoWindow(settings);
        return a;
     
    };
    $.fn.umsKendoWindow = function( options ) {
        return _umsKendowindow(this,options);
    };
    /*deprecated*/
    $.fn.usmKendoWindow = function(options, deprecated) {
        return _umsKendowindow(this,options);
    }
}( jQuery ));

(function ($, kendo) {
    var Grid = kendo.ui.Grid.extend({
        resizeGridToFitHeight:function (offSet) {
            try {
                var pg = $gridElement.data("kendoGrid");
                var shgt = window.innerHeight;
                if (typeof(offSet) == "undefined") {
                    offSet = 26; //used for standard page
                }
                var topBottomMarginHeight = $('.k-header')["0"].clientHeight +
                    $('.ums-menu')["0"].clientHeight +
                    $('.breadcrumbs')["0"].clientHeight;
                var windowHeight = shgt - topBottomMarginHeight - offSet;
                //console.log("resize");
                pg.element.height(windowHeight);
                pg.resize();
                //pg.refresh();
            } catch (e) {

            } 
        }
    });
    kendo.ui.plugin(Grid);
    var ToolBar = kendo.ui.ToolBar.extend({
        test: function() {
            try {
                alert('test');
            } catch (e) {

            }
        }
        ,umsShowButtonByClass:function (clas) {
            //debugger;
            var tb = this;
            tb.options.items.forEach(function (btn) {
                if (btn.type === "button" && btn.attributes.class.indexOf(clas) >= 0) {
                    tb.show($('#' + btn.id));
                }
            });
        },umsHideButtonByClass:function (clas) {
            var tb = this;
            tb.options.items.forEach(function (btn) {
                if (btn.type === "button"&& btn.attributes.class.indexOf(clas) >= 0 ) {
                    tb.hide($('#' + btn.id));
                }
            });
        }
        ,umsHideAllEditSaveButton:function () {
            var tb = this;
            tb.options.items.forEach(function (btn) {
                if (btn.type === "button"&& (
                    btn.attributes.class.indexOf("modify-mode") >= 0
                        ||
                        btn.attributes.class.indexOf("view-mode") >= 0
                        )
                    ) {
                    tb.hide($('#' + btn.id));
                }
            });
        }
        ,umsHideAllButton:function () {
            var tb = this;
            tb.options.items.forEach(function (btn) {
                if (btn.type === "button") {
                    tb.hide($('#' + btn.id));
                }
            });
        }
    });
    kendo.ui.plugin(ToolBar);
    var NumericTextBox  = kendo.ui.NumericTextBox.extend({
       _adjust : function(value) {
          var that = this,
             options = that.options,
             min = options.min,
             max = options.max;

          if (value === null) {
             return value;
          }
          element = this.element;
          var triggerEvent = function() {
             var evtData = { value: value, max: max, min: min };
             setTimeout(function() {
                element.trigger("outOfRange", evtData);
                console.log("outOfRange: " + evtData.value);
             }, 1);
          };
          if (min !== null && value < min) {
             triggerEvent();
             value = min;
          } else if (max !== null && value > max) {
             triggerEvent();
             value = max;
          }
          return value;
       }
    });
    kendo.ui.plugin(NumericTextBox);
    
}(window.kendo.jQuery, window.kendo));

(function ($, kendo) {
var Win = kendo.ui.Window.extend({
    umsResizeHeight: function (height) {
        //this.setOptions({ "height": height });
        //this.refresh();
    }
});
kendo.ui.plugin(Win);
    
}(window.kendo.jQuery, window.kendo));

function setGridKendoTooltip(div,filter) {
   try {
      if (typeof filter === 'undefined') {
         filter =  "td"
      }
      $("#" + div).kendoTooltip({
         filter: filter,
         show: function(e) {
            if (this.content.text() !== "") {
               $('[role="tooltip"]').css("visibility", "visible");
            }
         },
         hide: function() {
            $('[role="tooltip"]').css("visibility", "hidden");
         },
         content: function(e) {
            
            function extractContent(e)
            {
               var element = e.target[0];
               if (element.classList.contains('k-group-cell')) {
                  if (element["data-title"]) {
                     return element["data-title"];
                  }
               }
               if (element.classList.contains('k-header')) {
                  if (element.hasAttributes('data-title')) {
                     return element.dataset.title;
                  }
               }
               if (element.offsetWidth < element.scrollWidth) {
                  return e.target.text();
               } else {
                  return "";
               }
            }
            return extractContent(e).replaceAll("<","&#60;").replaceAll("<","&#62;")
         }
      });
   } catch (e) {
      console.error('setGridKendoTooltip',div, e);
   } 
}