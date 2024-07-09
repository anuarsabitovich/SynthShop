using LanguageExt.Common;

namespace SynthShop.Tests.Extensions;

public static class ResultObjectExtension
{
    public static T UnwrapResult<T>(Result<T> result)
    {
        return result.Match(
            value => value,
            ex => throw ex
        );
    }
}