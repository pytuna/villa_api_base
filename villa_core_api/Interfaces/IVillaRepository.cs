using System.Linq.Expressions;
using VillaApi.Entities;
namespace VillaApi.Interfaces;

public interface IVillaRepository : IRepository<Villa>
{
    // Task UpdateDynamicFieldAsync(Villa entity);
}
