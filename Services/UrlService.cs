using System.Text;

namespace Short.Services
{
    public interface IUrlService
    {
        Task<string> ShortenUrl(HttpRequest request, string url);
        Task<string> GetLongUrl(string id);
    }

    public class UrlService : IUrlService
    {
        private readonly DataContext _context;
        public UrlService(DataContext context)
        {
            _context = context;
        }

        private string GenerateShortUrl(string longUrl)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            int maxSize = 7;
            var random = new Random();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= maxSize; i++)
            {
                sb.Append(chars[random.Next(0, chars.Length)]);
            }

            return sb.ToString();
        }

        public async Task<string> ShortenUrl(HttpRequest request, string url)
        {
            string generatedShortUrl;
            var existingUrl = await _context.Urls.Where(u => u.LongUrl == url).FirstOrDefaultAsync();
            string shortUrl;

            if (existingUrl == null)
            {
                int maxRetries = 10;
                int retryCount = 0;
                do
                {
                    shortUrl = GenerateShortUrl(url);
                    existingUrl = await _context.Urls.Where(u => u.ShortUrl == shortUrl).FirstOrDefaultAsync();
                    retryCount++;
                }
                while (existingUrl != null && retryCount <= maxRetries);

                _context.Urls.Add(new Models.Url() { LongUrl = url, ShortUrl = shortUrl });
                await _context.SaveChangesAsync();

                generatedShortUrl = "https://" + request.Host.ToString() + "/" + shortUrl;

                return generatedShortUrl;
            }

            generatedShortUrl = "https://" + request.Host.ToString() + "/" + existingUrl.ShortUrl;
            return generatedShortUrl; ;
        }

        public async Task<string> GetLongUrl(string id)
        {
            Url result = await _context.Urls.Where(u => u.ShortUrl == id).FirstOrDefaultAsync();
            if (result == null)
            {
                return null;
            }

            string longUrl = result.LongUrl;
            if (!longUrl.StartsWith("http"))
            {
                longUrl = "https://" + longUrl;
            }

            return longUrl;
        }
    }
}