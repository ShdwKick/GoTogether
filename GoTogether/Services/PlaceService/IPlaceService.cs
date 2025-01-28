namespace GraphQLServer.Services.PlaceService;

public interface IPlaceService
{
    Task<List<string>> TryGetSuggestion(string place);
}