using System;

namespace AspNetCore.MVC.RESTful.Controllers
{
    public class MustSet<T>
    {
        private readonly string _name;
        public MustSet(string name)
        {
            _name = name;
        }

        private T _value;

        public T Get()
        {
            if (_value == null)
            {
                throw new NullReferenceException($"HateoasConfig.{_name} is not Set");
            }
            return _value;
        }
        public void Set(T value)
        {
            _value = value;
        }

    }
}