using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using autobusTestWork.Models;
using autobusTestWork.Entity.Models;
using autobusTestWork.Entity.Repositories;
using autobusTestWork.Services;

namespace autobusTestWork.Controllers;

public class HomeController : Controller
{
    private readonly LinkRepository _linkRepository;
    private readonly UrlHashService _urlHasher;

    public HomeController(NHibernate.ISession nhibernateSession, UrlHashService urlHasher)
    {
        _linkRepository = new LinkRepository(nhibernateSession);
        _urlHasher = urlHasher;
    }

    public async Task<IActionResult> Index()
    {
        List<Link> links = await _linkRepository.GetAll();

        return View(links);
    }

    public IActionResult CreateLink()
    {
        return View("CreateEdit", new LinkModel() { Id = null, LongUrl = "" });
    }

    public async Task<IActionResult> EditLink([FromQuery] int id)
    {
        var link = await _linkRepository.Get(id);

        if (link == null)
            return RedirectToAction("Index");

        return View("CreateEdit", new LinkModel() { Id = id, LongUrl = link.LongUrl });
    }

    [HttpPost]
    public async Task<IActionResult> CreateUpdateLink([FromForm] LinkModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Error = ModelState.Values.SelectMany(v => v.Errors).First().ErrorMessage;
            return View("CreateEdit", model);
        }

        var checkLink = await _linkRepository.GetByLongUrl(model.LongUrl);

        if (checkLink != null && checkLink.Id != model.Id)
        {
            model.Error = "Link with this url alredy exists!";
            return View("CreateEdit", model);
        }

        string hashedUrl = _urlHasher.HashUrl(model.LongUrl, $"http://localhost:{HttpContext.Request.Host.Port}");

        if (!model.Id.HasValue)
        {
            Link nLink = new()
            {
                Created = DateTime.UtcNow,
                ShortUrl = hashedUrl,
                LongUrl = model.LongUrl,
                RedirectCount = 0
            };

            await _linkRepository.Insert(nLink);
        }
        else
        {
            Link? link = await _linkRepository.Get(model.Id.Value);

            if (link == null)
                return RedirectToAction("Index");

            link.LongUrl = model.LongUrl;
            link.ShortUrl = hashedUrl;

            await _linkRepository.Update(link);
        }

        return RedirectToAction("Index");
    }

    [HttpDelete]
    public async Task<IResult> DeleteLink([FromQuery] int id)
    {
        var link = await _linkRepository.Get(id);

        if (link == null)
            return Results.Problem(statusCode: 404);

        await _linkRepository.Delete(link);

        return Results.Ok();
    }

    [HttpGet("/link/{link}")]
    public async Task<IActionResult> UseLink()
    {
        string fullUrl = $"http://localhost:{HttpContext.Request.Host.Port}{HttpContext.Request.Path}";

        var link = await _linkRepository.GetByShortUrl(fullUrl);

        if (link == null)
            return RedirectToAction("Index");

        await _linkRepository.AddRedirectCount(link.Id);

        return Redirect(link.LongUrl);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
