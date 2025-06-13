using autobusTestWork.Persistence.Repositories;
using NHibernate;

namespace autobusTestWork.Application.Services
{
    public class DataService
    {
        public LinkRepository Links { get; private set; }

        public DataService(ISession session)
        {
            Links = new(session);
        }
    }
}
