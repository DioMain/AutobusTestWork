using NHibernate;

namespace autobusTestWork.Entity.Repositories;

public abstract class RepositoryBase<T> : IRepository<T>
{
    protected NHibernate.ISession HibernateSession;

    public RepositoryBase(NHibernate.ISession session)
    {
        HibernateSession = session;
    }

    public abstract Task Delete(T entity);
    public abstract Task<T?> Get(int id);
    public abstract Task<List<T>> GetAll();
    public abstract Task<int> Insert(T entity);
    public abstract Task Update(T entity);
}
