﻿using EasyEF.Entities;
using EasyEF.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EasyEF.Controllers;

/// <summary>
/// Uses a pattern of directly using serialized EF entities to interact with the WebAPI.
/// This is typically not best practice but this class is designed to specifically create simple,
/// controllers for use directly with entities.
///
/// Creates a simple, very vaguely REST-like WebAPI over EF Core "entities".
///
/// For more typical "business" controllers for a web-app I would expect to use a DTO layer to decouple
/// the WebAPI contracts from the DB Schema
/// </summary>
/// <typeparam name="TEntity"></typeparam>

[ApiController]
[Route("api/[controller]")]
public abstract class EasyEfController<TEntity> : ControllerBase where TEntity : Entity
{
    private readonly ILinqRepository<TEntity> _repository;

    protected EasyEfController(ILinqRepository<TEntity> repository)
    {
        _repository = repository;
    }

    // GET: api/<Entity>
    [HttpGet]
    public IResult Get() => Results.Ok(_repository.Get());

    // GET api/<Entity>/5
    [HttpGet("{id}")]
    public IResult Get(Guid id)
    {
        var entity = _repository.Get(id);

        return entity != null
            ? Results.Ok(entity)
            : Results.NotFound();
    }

    // POST api/<Entity>
    [HttpPost]
    public IResult Post([FromBody] TEntity value)
    {
        _repository.Add(value);
        _repository.Save();
        var entityName = typeof(TEntity).Name.Replace("Entity","");
        return Results.Created($"{entityName}/{value.Id}", value.Id);
    }

    // PUT api/<Entity>
    [HttpPut()]
    public IResult Put([FromBody] TEntity value)
    {
        _repository.Update(value);
        return SaveChanges(Results.NotFound(),
            Results.NoContent());
    }

    // DELETE api/<Entity>/5
    [HttpDelete("{id}")]
    public IResult Delete(Guid id)
    {
        var e = Activator.CreateInstance(typeof(TEntity)) as TEntity;
        e.Id = id;
        _repository.Delete(e);
        return SaveChanges(Results.NotFound());
    }

    private IResult SaveChanges(
        IResult failResult, IResult? successResult = null)
    {
        successResult ??= Results.Ok();
        try
        {
            _repository.Save();
        }
        catch (DbUpdateConcurrencyException e)
        {
            return failResult;
        }

        return successResult;
    }
}