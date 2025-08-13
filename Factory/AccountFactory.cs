using Lorecraft_API.Data.DTO.Account;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Factory.Interface;
using Lorecraft_API.Manager;
using Lorecraft_API.Resources;

namespace Lorecraft_API.Factory
{
    public class AccountFactory(CryptoManager cryptoManager) : IAccountFactory
    {
        private readonly CryptoManager _cryptoManager = cryptoManager; 
        public Account Create(AccountCreateRequest req) => new(){
            AccountId = GeneratorManager.GenerateAccountId(),
            FirstName = req.FirstName, 
            MiddleName = req.MiddleName,
            LastName = req.LastName,
            Birthdate = req.Birthdate,
            Gender = req.Gender,
            IsEmailVerified = false,
            Email = req.Email,
            IsContactVerified = false,
            ContactNum = req.ContactNum,
            IsSpammer = false,
            Password = _cryptoManager.HashPassword(req.Password),
            PenName = req.PenName, 
            CountryCodeContact = req.CountryCode,
            DatetimeCreated = Constants.CurrentUtc8PlusNow,
            Role = Constants.RegularUser
        };
        public AccountResponse MapToResponse(Account acc, Constants.PasswordRequestMode requestMode) => new(){
            AccountId = acc.AccountId,
            FirstName = acc.FirstName,
            MiddleName = acc.MiddleName,
            LastName = acc.LastName,
            Birthdate = acc.Birthdate,
            Gender = acc.Gender,
            IsEmailVerified = acc.IsEmailVerified,
            Email = acc.Email,
            IsContactVerified = acc.IsContactVerified,
            ContactNum = acc.ContactNum,
            IsSpammer = acc.IsSpammer,
            PenName = acc.PenName,
            DateCreated = acc.DatetimeCreated.ToShortDateString(),
            TimeCreated = acc.DatetimeCreated.ToShortTimeString(),
            CountryCode = acc.CountryCodeContact,
            Password = requestMode == Constants.PasswordRequestMode.RequirePassword ? acc.Password : null,
            Role = acc.Role
        };
    }

}