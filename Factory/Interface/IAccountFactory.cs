using Lorecraft_API.Data.DTO.Account;
using Lorecraft_API.Data.Models;
using static Lorecraft_API.Resources.Constants;

namespace Lorecraft_API.Factory.Interface
{
    public interface IAccountFactory
    {
        Account Create(AccountCreateRequest req); 
        AccountResponse MapToResponse(Account acc, PasswordRequestMode requestMode);
    }
}