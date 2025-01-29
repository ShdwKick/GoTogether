using Microsoft.Extensions.Caching.Memory;

namespace GoTogether.Services.PlaceService;

public class PlaceService : IPlaceService
{
    private readonly IMemoryCache _cache;

    public PlaceService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public async Task<List<string>> TryGetSuggestion(string place)
    {
        if (_cache.TryGetValue("suggecstions", out Dictionary<string,List<string>> result))
        {
            if (result != null)
            {
                //if(result.Keys.FirstOrDefault())
            }
        }

        throw new NotImplementedException();

    }
}