using System.Linq.Expressions;

namespace GymManagementSystem.Business.Abstract;

public interface IGenericService<T>
{
    void Insert(T entity);
    void Delete(T entity);
    void Update(T entity);
    List<T> GetList();
    List<T> GetListByFilter(Expression<Func<T, bool>> filter);
    T GetById(int id);
}
