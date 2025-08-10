using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;
using Lorecraft_API.Data.DTO.Account;

namespace Lorecraft_API.Resources.Handler
{
    public class AccountSignInBinder : IModelBinder
    {
        public async Task BindModelAsync(ModelBindingContext bindingContext){
            var request = bindingContext.HttpContext.Request;
            IAccountSignInRequest? model = null;

            if (request.HasFormContentType)
            {
                var form = await request.ReadFormAsync();

                var type = form["type"].ToString();

                Console.WriteLine(type);

                model = type == "Email" ? new EmailRequest { Email = form["email"], Password = form["password"] }
                : type == "Contact" ? new ContactRequest { ContactNum = form["contact_num"], Password = form["password"] }
                : type == "Pen" ? new PenNameRequest { PenName = form["pen_name"], Password = form["password"] }
                : null;
            }
            else
            {
                var json = await new StreamReader(request.Body).ReadToEndAsync();
                var jObject = JObject.Parse(json);

                if (jObject.ToObject<EmailRequest>() is null && jObject.ToObject<ContactRequest>() is null && jObject.ToObject<PenNameRequest>() is null)
                {
                    bindingContext.Result = ModelBindingResult.Failed();
                    return;
                }

                model = jObject["type"]?.ToString() == "Email" ? jObject.ToObject<EmailRequest>()
                : jObject["type"]?.ToString() == "Contact" ? jObject.ToObject<ContactRequest>()
                : jObject["type"]?.ToString() == "Pen" ? jObject.ToObject<PenNameRequest>()
                : null;
            }

            bindingContext.Result = model is not null ? ModelBindingResult.Success(model) : ModelBindingResult.Failed();
        }
    }
}