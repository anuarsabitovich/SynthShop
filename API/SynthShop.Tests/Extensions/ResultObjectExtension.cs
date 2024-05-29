using LanguageExt.Common;

namespace SynthShop.Tests.Extensions
{
    public static class ResultObjectExtension
    {
        public static T UnwrapResult<T>(Result<T> result)
        {
            return result.Match(
                Succ: value => value,
                Fail: ex => throw ex         
            );
        }
    }
}
