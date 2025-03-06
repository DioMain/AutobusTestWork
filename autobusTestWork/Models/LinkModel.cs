using System.ComponentModel.DataAnnotations;

namespace autobusTestWork.Models;

public class LinkModel
{
    public int? Id { get; set; }

    [Required, Url]
    public string LongUrl { get; set; } = "";

    public string? Error { get; set; }
}
