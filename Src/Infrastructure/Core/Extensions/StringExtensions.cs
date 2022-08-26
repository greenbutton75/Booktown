namespace Infrastructure.Core.Extensions;

using System;
 
public static class StringExtensions
{
    public static string RemovePrefix(this string s, string prefix)
    {
        return s[prefix.Length..];
    }
}