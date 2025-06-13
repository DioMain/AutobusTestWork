using autobusTestWork.Core.Repository;
using NHibernate;

namespace autobusTestWork.Persistence;

public abstract class RepositoryBase<T> : IRepository<T>
{
    protected ISession HibernateSession;

    public RepositoryBase(ISession session)
    {
        HibernateSession = session;
    }

    public abstract Task Delete(T entity);
    public abstract Task<T?> Get(int id);
    public abstract Task<List<T>> GetAll();
    public abstract Task<int> Insert(T entity);
    public abstract Task Update(T entity);
}
