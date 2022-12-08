using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading.Tasks;

namespace ConfiguratorWeb.App.Models.Therapy
{
    public class DoubleModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            // Type of specification
            var type = bindingContext.ModelType;

            // Create default instance of the specification
            var model = Activator.CreateInstance(type);

            string value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;

            double result = 0;
            double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result);

            model = result;
            // Set the result to our populated model
            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
        public Task BindModelAsync2(ModelBindingContext bindingContext)
        {
            CultureInfo culture = null;

            var request = bindingContext.HttpContext.Request;
            if (request.Headers.AcceptLanguage.Count == 0)
                culture = CultureInfo.CurrentUICulture;
            else
            {
                //culture = new CultureInfo(request.Headers.ContentLanguage[0]);
                string userCulture = request.Headers.AcceptLanguage[0];
                if (userCulture.Contains(';') || userCulture.Contains(','))
                {
                    userCulture = userCulture.Split(',',';')[0];
                }
                culture = new CultureInfo(userCulture);
            }

            // Type of specification
            var type = bindingContext.ModelType;

            // Create default instance of the specification
            var model = Activator.CreateInstance(type);


            foreach (var property in type.GetProperties())
            {
                if (property is null)
                    continue;

                string formPropertyName = string.Format("{0}.{1}", property.DeclaringType.Name, property.Name);

                if (property.PropertyType == typeof(double?))
                {
                    string value = bindingContext.ValueProvider.GetValue(formPropertyName).FirstValue;

                    double dvalue = 0;
                    double.TryParse(value, NumberStyles.Any, culture, out dvalue);
                    property.SetValue(model, dvalue);
                }
                else 
                {

                    // Is there a standard way to bind all of the other properties? This will only handle simple types
                    var value = bindingContext.ValueProvider.GetValue(formPropertyName).FirstValue;
                    var converter = TypeDescriptor.GetConverter(property.PropertyType);
                    var convertedValue = converter.ConvertFrom(value!);
                    property.SetValue(model, convertedValue);
                }
            }

            // Set the result to our populated model
            bindingContext.Result = ModelBindingResult.Success(model);

            return Task.CompletedTask;
        }
    }
}
