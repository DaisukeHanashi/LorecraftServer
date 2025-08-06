namespace Lorecraft_API.Resources
{
    public class ResultMessage
    {
        public required int Code { get; set; }
        public required string Message { get; set; }
        public string? IdString { get; set; }
        public long? Id64 { get; set; }

        public object? Data { get; set; }
    }

    public static class ResultMessageExtensions
    {
        public static void ClearData(this ResultMessage result) => result.Data = null;
        public static bool IsStatusCodeAllOK(this ResultMessage result) => result.Code >= StatusCodes.Status200OK && result.Code <= StatusCodes.Status226IMUsed;
        public static bool IsStatusCodeNotAllOK(this ResultMessage result) => !result.IsStatusCodeAllOK();

    }
}