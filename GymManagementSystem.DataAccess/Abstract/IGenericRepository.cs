using System.Linq.Expressions;

namespace GymManagementSystem.DataAccess.Abstract;

public interface IGenericRepository<T> where T : class
{
    void Insert(T t);
    void Delete(T t);
    void Update(T t);
    List<T> GetList();
    List<T> GetListByFilter(Expression<Func<T, bool>> filter);
    T GetByID(int id);
}
