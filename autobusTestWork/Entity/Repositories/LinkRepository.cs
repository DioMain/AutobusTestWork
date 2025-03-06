using autobusTestWork.Entity.Models;
using NHibernate.Linq;
using System.Security.Policy;

namespace autobusTestWork.Entity.Repositories
{
    public class LinkRepository : RepositoryBase<Link>
    {
        public LinkRepository(NHibernate.ISession session) : base(session)
        {
        }

        public override async Task Delete(Link entity)
        {
            using var transaction = HibernateSession.BeginTransaction();

            await HibernateSession.DeleteAsync(entity);

            await transaction.CommitAsync();
        }

        public override async Task<Link?> Get(int id)
        {
            var link = await HibernateSession.Query<Link>().FirstOrDefaultAsync(l => l.Id == id);

            return link;
        }

        public async Task<Link?> GetByLongUrl(string url)
        {
            var link = await HibernateSession.Query<Link>().FirstOrDefaultAsync(l => l.LongUrl == url);

            return link;
        }

        public async Task<Link?> GetByShortUrl(string url)
        {
            var link = await HibernateSession.Query<Link>().FirstOrDefaultAsync(l => l.ShortUrl == url);

            return link;
        }

        public override async Task<List<Link>> GetAll()
        {
            var links = await HibernateSession.Query<Link>().ToListAsync();

            return links;
        }

        public override async Task<int> Insert(Link entity)
        {
            using var transaction = HibernateSession.BeginTransaction();

            int id = (int)await HibernateSession.SaveAsync(entity);

            await transaction.CommitAsync();

            return id;
        }

        public override async Task Update(Link entity)
        {
            using var transaction = HibernateSession.BeginTransaction();

            await HibernateSession.UpdateAsync(entity);

            await transaction.CommitAsync();
        }

        public async Task AddRedirectCount(int id)
        {
            var link = await Get(id);

            if (link == null)
                return;

            link.RedirectCount++;

            await Update(link);
        }
    }
}
