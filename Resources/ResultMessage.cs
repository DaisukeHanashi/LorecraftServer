using System.Text.Json;

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

        public static IEnumerable<T> PaginateData<T>(this ResultMessage result, int pageSize, int pageNumber) where T : class
        {
            if (result.Data is not IEnumerable<T> allData)
            {
                string data = result.Data.ToString();
                allData = JsonSerializer.Deserialize<IEnumerable<T>>(data);
            }

            int skipNumber = (pageNumber - 1) * pageSize;
            return allData.Skip(skipNumber).Take(pageSize);
        }

    }
}