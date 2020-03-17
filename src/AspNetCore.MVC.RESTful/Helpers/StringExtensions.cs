using System;
using System.Linq;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public static class StringExtensions
    {

        public static string Unwrap(this string source, char wrapper)
        {
            if (string.IsNullOrEmpty(source)) return source;

            if (source.First() == wrapper && source.Last() == wrapper)
            {
                return source.AsSpan(1, source.Length - 2).ToString();
            }

            return source;
        }
    }
}