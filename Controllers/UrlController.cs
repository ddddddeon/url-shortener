namespace Short.Controllers;

[ApiController]
[Route("")]
public class UrlController : ControllerBase
{
    private readonly IUrlService _urlService;
    public UrlController(IUrlService urlService)
    {
        _urlService = urlService;
    }

    [HttpGet("api/shorten")]
    public async Task<IActionResult> GetShortUrl(string url)
    {
        string shortenedUrl = await _urlService.ShortenUrl(HttpContext.Request, url);
        return Ok(new Dictionary<string, string>() {
            { "url", shortenedUrl }
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetLongUrl(string id)
    {
        var longUrl = await _urlService.GetLongUrl(id);
        if (longUrl != null)
        {
            return Redirect(longUrl);
        }
        else
        {
            return BadRequest(new Dictionary<string, string> {
                { "error", "No URL found with that id!"}
            });
        }
    }
}
