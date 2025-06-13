namespace autobusTestWork.Core.Models;

public class Link
{
    public virtual int Id { get; set; }
    
    public virtual string LongUrl { get; set; }

    public virtual string ShortUrl { get; set; }

    public virtual DateTime Created { get; set; }

    public virtual int RedirectCount { get; set; }
}
