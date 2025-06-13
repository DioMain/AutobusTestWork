using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using autobusTestWork.Models;
using autobusTestWork.Application.Services;
using autobusTestWork.Persistence.Repositories;
using autobusTestWork.Core.Models;

namespace autobusTestWork.Controllers;

public class HomeController : Controller
{
    private readonly DataService _data;
    private readonly UrlHashService _urlHasher;

    public HomeController(DataService dataService, UrlHashService urlHasher)
    {
        _data = dataService;
        _urlHasher = urlHasher;
    }

    public async Task<IActionResult> Index()
    {
        List<Link> links = await _data.Links.GetAll();

        return View(links);
    }

    public IActionResult CreateLink()
    {
        return View("CreateEdit", new LinkModel() { Id = null, LongUrl = "" });
    }

    public async Task<IActionResult> EditLink([FromQuery] int id)
    {
        var link = await _data.Links.Get(id);

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

        var checkLink = await _data.Links.GetByLongUrl(model.LongUrl);

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

            await _data.Links.Insert(nLink);
        }
        else
        {
            Link? link = await _data.Links.Get(model.Id.Value);

            if (link == null)
                return RedirectToAction("Index");

            link.LongUrl = model.LongUrl;
            link.ShortUrl = hashedUrl;

            await _data.Links.Update(link);
        }

        return RedirectToAction("Index");
    }

    [HttpDelete]
    public async Task<IResult> DeleteLink([FromQuery] int id)
    {
        var link = await _data.Links.Get(id);

        if (link == null)
            return Results.Problem(statusCode: 404);

        await _data.Links.Delete(link);

        return Results.Ok();
    }

    [HttpGet("/link/{link}")]
    public async Task<IActionResult> UseLink()
    {
        string fullUrl = $"http://localhost:{HttpContext.Request.Host.Port}{HttpContext.Request.Path}";

        var link = await _data.Links.GetByShortUrl(fullUrl);

        if (link == null)
            return RedirectToAction("Index");

        await _data.Links.AddRedirectCount(link.Id);

        return Redirect(link.LongUrl);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
