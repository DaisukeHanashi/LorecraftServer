using Lorecraft_API.Resources;

namespace Lorecraft_API.StaticFactory
{
    public static class ResultMessageFactory
    {

        public static ResultMessage CreateOKResult(string message)
        => CreateResult(StatusCodes.Status200OK, message);
        public static ResultMessage CreateOKResult(string message, object data)
        => CreateResult(StatusCodes.Status200OK, message, data);
        public static ResultMessage CreateCreatedResult(string message)
        => CreateResult(StatusCodes.Status201Created, message);
        public static ResultMessage CreateCreatedResult(string message, string idString)
        => CreateResult(StatusCodes.Status201Created, message, idString);
        public static ResultMessage CreateCreatedResult(string message, long Id64)
        => CreateResult(StatusCodes.Status201Created, message, Id64);
        public static ResultMessage CreateAcceptedResult(string message)
        => CreateResult(StatusCodes.Status202Accepted, message);
         public static ResultMessage CreateAcceptedResult(string message, long id64, object data)
        => CreateResult(StatusCodes.Status202Accepted, message, id64, data);
        public static ResultMessage CreateNotFoundResult(string message)
        => CreateResult(StatusCodes.Status404NotFound, message);
        public static ResultMessage CreateBadRequestResult(string message)
        => CreateResult(StatusCodes.Status400BadRequest, message);
        public static ResultMessage CreateUnauthorizedResult(string message)
        => CreateResult(StatusCodes.Status401Unauthorized, message);
        public static ResultMessage CreateInternalServerErrorResult(string message)
        => CreateResult(StatusCodes.Status500InternalServerError, message);
        public static ResultMessage CreateResult(int code, string message) => new() { Code = code, Message = message };
        public static ResultMessage CreateResult(int code, string message, string idString) => new() { Code = code, Message = message, IdString = idString };
        public static ResultMessage CreateResult(int code, string message, long id64) => new() { Code = code, Message = message, Id64 = id64 };
        public static ResultMessage CreateResult(int code, string message, object data) => new() { Code = code, Message = message, Data = data };
        public static ResultMessage CreateResult(int code, string message, long id64, object data) => new() { Code = code, Message = message, Id64 = id64, Data = data};


        public static void ModifyData(this ResultMessage result, object? freshData)
        {
            if (freshData is null)
                return;

            result.Data = freshData;
        }
    }
}