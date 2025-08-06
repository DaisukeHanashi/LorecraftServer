using Lorecraft_API.Resources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
namespace Lorecraft_API.Store
{
    public class CookieTicketStore(IOptionsMonitor<CookieAuthenticationOptions> options)
    {
        private readonly ISecureDataFormat<AuthenticationTicket> _ticketFormat = options.Get(Constants.AuthenticationScheme).TicketDataFormat;
        public ISecureDataFormat<AuthenticationTicket> TicketFormat => _ticketFormat;  

    }
}