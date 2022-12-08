if (!String.prototype.format) {
   String.prototype.format = function () {
      var args = arguments;
      return this.replace(/\{\{|\}\}|\{(\d+)\}/g, function (m, n) {
         if (m == "{{") { return "{"; }
         if (m == "}}") { return "}"; }
         return args[n];
      });
   };   
}

if (!String.prototype.replaceAll) {
   String.prototype.replaceAll = function(target, replacement) {
      return this.split(target).join(replacement);
   };
}
if (!Array.prototype.find) {
   Object.defineProperty(Array.prototype, 'find', {
      value: function(predicate) {
         'use strict';
         if (this == null) {
            throw new TypeError('Array.prototype.find called on null or undefined');
         }
         if (typeof predicate !== 'function') {
            throw new TypeError('predicate must be a function');
         }
         var list = Object(this);
         var length = list.length >>> 0;
         var thisArg = arguments[1];

         for (var i = 0; i !== length; i++) {
            if (predicate.call(thisArg, this[i], i, list)) {
               return this[i];
            }
         }
         return undefined;
      }
   });
}
   if (!String.prototype.noExponents) {
      String.prototype.noExponents = function(explicitNum) {
         var data, leader, mag, multiplier, num, sign, str, z;
         if (explicitNum == null) {
            explicitNum = true;
         }

         /*
          * Remove scientific notation from a number
          *
          * After
          * http://stackoverflow.com/a/18719988/1877527
          */
         data = this.split(/[eE]/);
         if (data.length === 1) {
            return data[0];
         }
         z = "";
         sign = this.slice(0, 1) === "-" ? "-" : "";
         str = data[0].replace(".", "");
         mag = Number(data[1]) + 1;
         if (mag <= 0) {
            z = sign + "0.";
            while (!(mag >= 0)) {
               z += "0";
               ++mag;
            }
            num = z + str.replace(/^\-/, "");
            if (explicitNum) {
               return parseFloat(num);
            } else {
               return num;
            }
         }
         if (str.length <= mag) {
            mag -= str.length;
            while (!(mag <= 0)) {
               z += 0;
               --mag;
            }
            num = str + z;
            if (explicitNum) {
               return parseFloat(num);
            } else {
               return num;
            }
         } else {
            leader = parseFloat(data[0]);
            multiplier = Math.pow(10, parseInt(data[1]));
            return leader * multiplier;
         }
      };
   }
   if (!Number.prototype.noExponents) {
      Number.prototype.noExponents = function() {
         var strVal;
         strVal = String(this);
         return strVal.noExponents(true);
      };
   }
   if (!Number.prototype.toFixedSpecial) {
      Number.prototype.toFixedSpecial = function(n) {
         var str = this.toFixed(n);
         if (str.indexOf('e+') === -1)
            return str;

         // if number is in scientific notation, pick (b)ase and (p)ower
         str = str.replace('.', '').split('e+').reduce(function(p, b) {
            return p + Array(b - p.length + 2).join(0);
         });

         if (n > 0)
            str += '.' + Array(n + 1).join(0);

         return str;
      };
   }
   if (!String.prototype.toBool) {
      String.prototype.toBool = function() {
         return (this.toString().toLowerCase() == "true" || this.toString().toLowerCase() == "1");
      }
   }


if (!String.format) {
   String.format = function(format) {
      var args = Array.prototype.slice.call(arguments, 1);
      return format.replace(/{(\d+)}/g, function(match, number) { 
         return typeof args[number] != 'undefined'
               ? args[number] 
               : match
            ;
      });
   };
}
if (!String.prototype.startsWith) {
   Object.defineProperty(String.prototype, 'startsWith', {
      value: function(search, rawPos) {
         var pos = rawPos > 0 ? rawPos|0 : 0;
         return this.substring(pos, pos + search.length) === search;
      }
   });
}
if (!String.prototype.includes) {
   String.prototype.includes = function(search, start) {
      'use strict';

      if (search instanceof RegExp) {
         throw TypeError('first argument must not be a RegExp');
      } 
      if (start === undefined) { start = 0; }
      return this.indexOf(search, start) !== -1;
   };
}
// Production steps of ECMA-262, Edition 6, 22.1.2.1
if (!Array.from) {
  Array.from = (function () {
    var toStr = Object.prototype.toString;
    var isCallable = function (fn) {
      return typeof fn === 'function' || toStr.call(fn) === '[object Function]';
    };
    var toInteger = function (value) {
      var number = Number(value);
      if (isNaN(number)) { return 0; }
      if (number === 0 || !isFinite(number)) { return number; }
      return (number > 0 ? 1 : -1) * Math.floor(Math.abs(number));
    };
    var maxSafeInteger = Math.pow(2, 53) - 1;
    var toLength = function (value) {
      var len = toInteger(value);
      return Math.min(Math.max(len, 0), maxSafeInteger);
    };

    // The length property of the from method is 1.
    return function from(arrayLike/*, mapFn, thisArg */) {
      // 1. Let C be the this value.
      var C = this;

      // 2. Let items be ToObject(arrayLike).
      var items = Object(arrayLike);

      // 3. ReturnIfAbrupt(items).
      if (arrayLike === null) {
        throw new TypeError('Array.from requires an array-like object - not null or undefined');
      }

      // 4. If mapfn is undefined, then let mapping be false.
      var mapFn = arguments.length > 1 ? arguments[1] : void undefined;
      var T;
      if (typeof mapFn !== 'undefined') {
        // 5. else
        // 5. a If IsCallable(mapfn) is false, throw a TypeError exception.
        if (!isCallable(mapFn)) {
          throw new TypeError('Array.from: when provided, the second argument must be a function');
        }

        // 5. b. If thisArg was supplied, let T be thisArg; else let T be undefined.
        if (arguments.length > 2) {
          T = arguments[2];
        }
      }

      // 10. Let lenValue be Get(items, "length").
      // 11. Let len be ToLength(lenValue).
      var len = toLength(items.length);

      // 13. If IsConstructor(C) is true, then
      // 13. a. Let A be the result of calling the [[Construct]] internal method 
      // of C with an argument list containing the single item len.
      // 14. a. Else, Let A be ArrayCreate(len).
      var A = isCallable(C) ? Object(new C(len)) : new Array(len);

      // 16. Let k be 0.
      var k = 0;
      // 17. Repeat, while k < len… (also steps a - h)
      var kValue;
      while (k < len) {
        kValue = items[k];
        if (mapFn) {
          A[k] = typeof T === 'undefined' ? mapFn(kValue, k) : mapFn.call(T, kValue, k);
        } else {
          A[k] = kValue;
        }
        k += 1;
      }
      // 18. Let putStatus be Put(A, "length", len, true).
      A.length = len;
      // 20. Return A.
      return A;
    };
  }());
}