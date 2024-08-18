using Microsoft.EntityFrameworkCore;

namespace Library.API.Helpers
{
    public static class SqlServerDbFunctionsExtensions
    {
        [DbFunction("LIKE", Schema = "")]
        public static bool Like(string matchExpression, string pattern)
        {
            throw new NotSupportedException();
        }
    }
}