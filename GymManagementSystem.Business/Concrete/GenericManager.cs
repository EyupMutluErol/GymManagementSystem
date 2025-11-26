using GymManagementSystem.Business.Abstract;
using GymManagementSystem.DataAccess.Abstract;
using System.Linq.Expressions;

namespace GymManagementSystem.Business.Concrete;

public class GenericManager<T> :IGenericService<T> where T : class
{
    protected readonly IGenericRepository<T> _repository;

    public GenericManager(IGenericRepository<T> repository)
    {
        _repository = repository;
    }

    public void Delete(T entity)
    {
        _repository.Delete(entity);
    }

    public T GetById(int id)
    {
        return _repository.GetByID(id);
    }

    public T GetByID(int id)
    {
        throw new NotImplementedException();
    }

    public List<T> GetList()
    {
        return _repository.GetList();
    }

    public List<T> GetListByFilter(Expression<Func<T, bool>> filter)
    {
        return _repository.GetListByFilter(filter);
    }

    public void Insert(T entity)
    {
        _repository.Insert(entity);
    }

    public void Update(T entity)
    {
        _repository.Update(entity);
    }
}
