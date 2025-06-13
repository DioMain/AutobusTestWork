using autobusTestWork;
using autobusTestWork.Core.Models;
using FluentNHibernate.Mapping;

namespace autobusTestWork.Persistence.Mappings;

public class LinkMap : ClassMap<Link>
{
    public LinkMap()
    {
        Table("Links");
        Id(l => l.Id).GeneratedBy.Identity();
        Map(l => l.LongUrl).Not.Nullable().Unique();
        Map(l => l.ShortUrl).Not.Nullable();
        Map(l => l.Created).Not.Nullable();
        Map(l => l.RedirectCount).Default("0");
    }
}
