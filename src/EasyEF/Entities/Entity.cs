using Microsoft.EntityFrameworkCore;

namespace EasyEF.Entities;

[PrimaryKey("Id")]
public abstract record Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}