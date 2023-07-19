using System;

namespace AspNetCore.MVC.RESTful.Helpers
{
    public static class NullX
    {
        public static T Throw<T>(T instance, string paramName) 
            where T : class 
            => instance ?? throw new ArgumentNullException(paramName);

        public static bool Check<T>(T instance) 
            where T : class 
            => instance == null;
    }
}