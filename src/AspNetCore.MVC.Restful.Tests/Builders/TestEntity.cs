using System;
using System.ComponentModel.DataAnnotations;
using AspNetCore.MVC.RESTful.Controllers;
using chess.games.db.Entities;

namespace AspNetCore.MVC.Restful.Tests.Builders
{
    public class TestEntity : DbEntity<Guid>
    {
        public TestEntity()
        {
            Id = Guid.NewGuid();
        }
    }
    public class TestDto : Resource<Guid>
    {
        public string Value { get; set; }
    }
    public class TestCreationDto
    {
        [Required] public string Value { get; set; } = null;

        public string OtherValue { get; set; } = "";
    }
}