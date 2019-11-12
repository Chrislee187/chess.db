using System;
using AspNetCore.MVC.RESTful.Controllers;
using chess.games.db.Entities;

namespace AspNetCore.MVC.Restful.Tests.Builders
{
    public class TestEntity : DbEntity<Guid>
    {
    }
    public class TestDto : Resource<Guid>
    {
    }
}