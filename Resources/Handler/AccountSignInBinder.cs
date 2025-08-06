using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;
using Lorecraft_API.Data.DTO.Account;

namespace Lorecraft_API.Resources.Handler
{
    public class AccountSignInBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext){
            var json = await new StreamReader(bindingContext.HttpContext.Request.Body).ReadToEndAsync();
            var jObject = JObject.Parse(json);

            IAccountSignInRequest? model;

            if (jObject.ToObject<EmailRequest>() is null && jObject.ToObject<ContactRequest>() is null && jObject.ToObject<PenNameRequest>() is null)
            {
                bindingContext.Result = ModelBindingResult.Failed();
                return;
            }

            model = jObject["type"]?.ToString() == "Email" ? jObject.ToObject<EmailRequest>()
            : jObject["type"]?.ToString() == "Contact" ? jObject.ToObject<ContactRequest>()
            : jObject["type"]?.ToString() == "Pen" ? jObject.ToObject<PenNameRequest>()
            : null;

            bindingContext.Result = model is not null ? ModelBindingResult.Success(model) : ModelBindingResult.Failed();
        }
    }
}