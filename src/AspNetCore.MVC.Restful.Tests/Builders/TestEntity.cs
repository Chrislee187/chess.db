using System;

namespace AspNetCore.MVC.Restful.Tests.Builders
{
    public class TestEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}