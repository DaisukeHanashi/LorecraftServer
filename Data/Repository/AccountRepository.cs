using Lorecraft_API.Data.DTO.Account;
using Lorecraft_API.Data.Models;
using Lorecraft_API.Factory.Interface;
using Lorecraft_API.Manager;
using Lorecraft_API.Resources;
using Lorecraft_API.StaticFactory;
using static Lorecraft_API.Resources.Constants;

namespace Lorecraft_API.Data.Repository
{
    public interface IAccountRepository
    {
        Task<ResultMessage> GetAccounts();
        Task<ResultMessage> GetAccount(long id, PasswordRequestMode mode);
        Task<ResultMessage> GetAccountThroughAuthentication(IAccountSignInRequest request, AuthenticationMode mode);
        Task<ResultMessage> CreateAccount(AccountCreateRequest req);
        Task<ResultMessage> UpdateAccount(AccountUpdateRequest req);
        Task<ResultMessage> DeleteAccountSafe(AccountDeleteRequest req);
        Task<ResultMessage> DeleteAccountUnsafe(AccountDeleteRequest req);
        Task<ResultMessage> ChangePassword(ChangePasswordRequest req);
        Task<bool> DoesDuplicateEmailExist(string email);
        Task<bool> DoesDuplicateNumberExist(string contact);
        Task<bool> DoesDuplicatePenNameExist(string penName);
    }
    public class AccountRepository(SqlConnectionFactory factory,
    ILoggerFactory loggerFactory,
    IAccountFactory accountFactory,
    CryptoManager cryptoManager) : BaseRepository(factory, loggerFactory), IAccountRepository
    {
        private readonly IAccountFactory _accountFactory = accountFactory;
        private readonly CryptoManager _cryptoManager = cryptoManager;
        public async Task<ResultMessage> GetAccounts()
        {
            string sql = SQLWriter.GenerateSelectSqlWithSortByForDefaultSchema(nameof(Account), [], [nameof(Account.AccountId)], null, [nameof(Account.DatetimeCreated)], SortingMode.Descending).AsNoTracking();

            IEnumerable<Account> data = await GetAllAsync<Account>(sql);

            return ResultMessageFactory.CreateOKResult(CommonMessages.GetSuccessfulActMessage(nameof(Account)
            , CommonMessages.ReadAction, Plural), data.Select(datum => _accountFactory.MapToResponse(datum, PasswordRequestMode.NoPassword)));
        }
        public async Task<ResultMessage> GetAccount(long id, PasswordRequestMode requestMode)
        {
            string sql = SQLWriter.GenerateSelectSqlWithIdentityForDefaultSchema(nameof(Account.AccountId)).AsNoTracking();

            Account? acc = await GetAsync<Account>(sql, new { AccountId = id });

            ResultMessage result = acc is not null ?
            ResultMessageFactory.CreateOKResult(CommonMessages.GetSuccessfulActMessage(nameof(Account),
            CommonMessages.CreateAction),
            _accountFactory.MapToResponse(acc, requestMode))
            : ResultMessageFactory.CreateNotFoundResult(CommonMessages.GetNotFoundMessage(nameof(Account)));


            return result;

        }
        public async Task<ResultMessage> GetAccountThroughAuthentication(IAccountSignInRequest request, AuthenticationMode mode)
        => mode == AuthenticationMode.Email ? await GetAccountThroughEmail((EmailRequest)request) : await GetAccountThroughPenName((PenNameRequest)request);

        private async Task<ResultMessage> GetAccountThroughEmail(EmailRequest req)
        {
            string sql = SQLWriter.GenerateSelectSqlWithIdentityForDefaultSchema(nameof(req.Email)).AsNoTracking();

            string Email = req.Email.Trim();

            Account? acc = await GetAsync<Account>(sql, new { Email });

            ResultMessage result = acc is not null && _cryptoManager.VerifyPassword(req.Password, acc.Password) ?
            ResultMessageFactory.CreateOKResult(CommonMessages.SuccessfulLogin, _accountFactory.MapToResponse(acc, PasswordRequestMode.NoPassword))
            : ResultMessageFactory.CreateUnauthorizedResult(CommonMessages.IncorrectAuth);

            return result;
        }
        private async Task<ResultMessage> GetAccountThroughPenName(PenNameRequest req)
        {
            string sql = SQLWriter.GenerateSelectSqlWithIdentityForDefaultSchema(nameof(req.PenName)).AsNoTracking();

            string PenName = req.PenName.Trim();

            Account? acc = await GetAsync<Account>(sql, new { PenName });

            ResultMessage result = acc is not null && _cryptoManager.VerifyPassword(req.Password, acc.Password) ?
            ResultMessageFactory.CreateOKResult(CommonMessages.SuccessfulLogin, _accountFactory.MapToResponse(acc, PasswordRequestMode.NoPassword))
            : ResultMessageFactory.CreateUnauthorizedResult(CommonMessages.IncorrectAuth);

            return result;
        }
        public async Task<ResultMessage> CreateAccount(AccountCreateRequest req)
        {
            var duplicateCheckResult = await DoesAnyImportantStuffExists(req);

            if (duplicateCheckResult.Item1 && duplicateCheckResult.Item2 is not null)
                return duplicateCheckResult.Item2;

            try
            {

                Account account = _accountFactory.Create(req);
                string sql = SQLWriter.GenerateInsertSqlForDefaultScheme(account, account.Properties);

                await ExecuteAsync(sql, account);
                return ResultMessageFactory.CreateCreatedResult(CommonMessages.GetSuccessfulActMessage(nameof(Account),
                Constants.CommonMessages.CreateAction));
            }
            catch (Exception ex)
            {
                string message = CreateMessageFromExecuteResult(ex, CommonMessages.GetFailureMessage(nameof(Account),
                Constants.CommonMessages.CreatePresentAction));

                _logger.LogError(ex, message);
                return ResultMessageFactory.CreateInternalServerErrorResult(message);

            }
        }



        public async Task<ResultMessage> UpdateAccount(AccountUpdateRequest req)
        {
            var duplicateCheckResult = await DoesAnyImportantStuffExists(req);

            if (duplicateCheckResult.Item1 && duplicateCheckResult.Item2 is not null)
                return duplicateCheckResult.Item2;

            ResultMessage accountResult = await GetAccount(req.AccountId, PasswordRequestMode.NoPassword);


            if (accountResult.Data is not Account acc)
            {
                accountResult.ClearData();
                return accountResult;
            }

            string sql = SQLWriter.GenerateUpdateSqlForDefaultSchema(acc, req);

            if (string.IsNullOrEmpty(sql))
                return ResultMessageFactory.CreateInternalServerErrorResult(Constants.CommonMessages.EmptySQL);

            try
            {
                await ExecuteAsync(sql, req);
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Account), Constants.CommonMessages.UpdateAction));
            }
            catch (Exception ex)
            {
                string message = Constants.CommonMessages.GetFailureMessage(nameof(Account), Constants.CommonMessages.UpdatePresentAction);

                _logger.LogError(ex, message);
                return ResultMessageFactory.CreateInternalServerErrorResult(message);
            }

        }

        public async Task<ResultMessage> ChangePassword(ChangePasswordRequest req)
        {
            ResultMessage accountResult = await GetAccount(req.AccountId, PasswordRequestMode.RequirePassword);

            if (accountResult.Data is not Account acc)
            {
                accountResult.ClearData();
                return accountResult;
            }

            if (!_cryptoManager.VerifyPassword(req.OldPassword, acc.Password))
                return ResultMessageFactory.CreateUnauthorizedResult(Constants.CommonMessages.IncorrectAuth);

            try
            {
                req.NewPassword = _cryptoManager.HashPassword(req.NewPassword);
                string sql = SQLWriter.GenerateUpdateSqlForDefaultSchema(acc, req);

                await ExecuteAsync(sql, req);
                return ResultMessageFactory.CreateOKResult(Constants.CommonMessages.GetSuccessfulActMessage(nameof(Account.Password), Constants.CommonMessages.UpdateAction));
            }
            catch (Exception ex)
            {
                string message = Constants.CommonMessages.GetFailureMessage(nameof(Account), Constants.CommonMessages.UpdatePresentAction);

                _logger.LogError(ex, message);
                return ResultMessageFactory.CreateInternalServerErrorResult(message);
            }
        }

        public async Task<ResultMessage> DeleteAccountSafe(AccountDeleteRequest req)
        {
            ResultMessage accountResult = await GetAccount(req.AccountId, PasswordRequestMode.RequirePassword);

            if (accountResult.Data is not Account acc)
            {
                accountResult.ClearData();
                return accountResult;
            }

            if (!_cryptoManager.VerifyPassword(req.Password, acc.Password))
                return ResultMessageFactory.CreateUnauthorizedResult("Wrong password match!");

            return await DeleteAccountUnsafe(req);

        }

        public async Task<ResultMessage> DeleteAccountUnsafe(AccountDeleteRequest req)
        {
            string sql = SQLWriter.GenerateDeleteSqlForDefaultSchema(nameof(req.AccountId));

            try
            {
                await ExecuteAsync(sql, new { req.AccountId });
                return ResultMessageFactory.CreateOKResult(
                    CommonMessages.GetSuccessfulActMessage(nameof(Account), CommonMessages.DeleteAction));
            }
            catch (Exception ex)
            {
                string message = CommonMessages.GetFailureMessage(nameof(Account), CommonMessages.DeletePresentAction);
                _logger.LogError(ex, message);
                return ResultMessageFactory.CreateInternalServerErrorResult(message);
            }
        }

        public async Task<(bool, ResultMessage?)> DoesAnyImportantStuffExists(IAccountPostOrPutRequest req)
        {
            bool doesEmailExists = await DoesDuplicateEmailExist(req.Email);
            bool doesContactNumberExists = await DoesDuplicateNumberExist(req.ContactNum);
            bool doesPenNameExists = !string.IsNullOrEmpty(req.PenName) && await DoesDuplicatePenNameExist(req.PenName);

            string? message =
            doesEmailExists ? "Email Address already exists!" :
            doesContactNumberExists ? "Contact Number already exists!" :
            doesPenNameExists ? "Pen Name already taken!" : null;

            return (doesEmailExists || doesContactNumberExists || doesPenNameExists,
            !string.IsNullOrEmpty(message) ? ResultMessageFactory.CreateBadRequestResult(message) : null);
        }


        public async Task<bool> DoesDuplicateEmailExist(string email)
        {
            string sql = SQLWriter.GenerateSelectSqlWithSpecificPropsForDefaultSchema(nameof(Account), [nameof(Account.Email)]).AsNoTracking();

            string? existingEmail = await GetStringAsync(sql, new { Email = email });

            return CheckAndCompareBetweenInputAndExisting(existingEmail, email);
        }
        public async Task<bool> DoesDuplicateNumberExist(string contact)
        {
            string sql = SQLWriter.GenerateSelectSqlWithSpecificPropsForDefaultSchema(nameof(Account), [nameof(Account.ContactNum)]).AsNoTracking();

            string? existingNumber = await GetStringAsync(sql, new { ContactNum = contact });

            return CheckAndCompareBetweenInputAndExisting(existingNumber, contact);
        }

        public async Task<bool> DoesDuplicatePenNameExist(string penName)
        {
            string sql = SQLWriter.GenerateSelectSqlWithSpecificPropsForDefaultSchema(nameof(Account), [nameof(Account.PenName)]).AsNoTracking();

            string? existingPenName = await GetStringAsync(sql, new { PenName = penName });

            return CheckAndCompareBetweenInputAndExisting(existingPenName, penName);
        }

        private static bool CheckAndCompareBetweenInputAndExisting(string? existingName, string input)
        => !string.IsNullOrEmpty(existingName) && existingName.Equals(input.Trim());


    }
}