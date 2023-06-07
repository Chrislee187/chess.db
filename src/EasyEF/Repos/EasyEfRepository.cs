using EasyEF.Entities;
using Microsoft.EntityFrameworkCore;

namespace EasyEF.Repos;

public class EasyEfRepository<TEntity> : EasyEfRepositoryBase<TEntity> where TEntity : Entity
{
    public EasyEfRepository(DbContext dbContext) : base(dbContext)
    {
    }
}