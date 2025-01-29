namespace GoTogether.Services.PlaceService;

public interface IPlaceService
{
    Task<List<string>> TryGetSuggestion(string place);
}