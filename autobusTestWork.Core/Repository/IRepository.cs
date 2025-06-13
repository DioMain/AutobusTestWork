namespace autobusTestWork.Core.Repository;

public interface IRepository<T>
{
    public Task<List<T>> GetAll();

    public Task<T?> Get(int id);

    public Task<int> Insert(T entity);

    public Task Delete(T entity);

    public Task Update(T entity);
}
