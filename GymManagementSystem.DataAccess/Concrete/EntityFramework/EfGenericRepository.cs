using GymManagementSystem.DataAccess.Abstract;
using GymManagementSystem.DataAccess.Context;
using System.Linq.Expressions;

namespace GymManagementSystem.DataAccess.Concrete.EntityFramework;

public class EfGenericRepository<T>:IGenericRepository<T> where T : class
{
    protected readonly GymContext _context;

    public EfGenericRepository(GymContext context)
    {
        _context = context;
    }

    public void Delete(T entity)
    {
        _context.Remove(entity);
        _context.SaveChanges();
    }

    public T GetById(int id)
    {
        return _context.Set<T>().Find(id);
    }

    public List<T> GetList()
    {
        return _context.Set<T>().ToList();
    }

    public List<T> GetListByFilter(Expression<Func<T, bool>> filter)
    {
        return _context.Set<T>().Where(filter).ToList();
    }

    public void Insert(T entity)
    {
        _context.Add(entity);
        _context.SaveChanges();
    }

    public void Update(T entity)
    {
        _context.Update(entity);
        _context.SaveChanges();
    }
}
