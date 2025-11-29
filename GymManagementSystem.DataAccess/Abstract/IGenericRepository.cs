using System.Linq.Expressions;

namespace GymManagementSystem.DataAccess.Abstract;

public interface IGenericRepository<T> where T : class
{
    void Insert(T entity);
    void Delete(T entity);
    void Update(T entity);
    List<T> GetList();
    List<T> GetListByFilter(Expression<Func<T, bool>> filter);
    T GetById(int id);
}
