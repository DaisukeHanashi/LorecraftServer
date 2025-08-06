using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lorecraft_API.Data.DTO.Account
{
    public interface IAccountSignInRequest
    {
         string Password { get; }
    }
}