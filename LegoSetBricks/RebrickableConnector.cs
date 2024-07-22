using Rebrickable.Api;

namespace LegoSetBricks;

public class RebrickableConnector
{
  readonly HttpClient _httpClient;
  readonly RebrickableClient _rebrickableClient;
  readonly JsonSerializerOptions _serializerOptions;

  private string _userToken = null;

  private readonly RebrickableCredentials _credentials;

  private const string _cacheSetPath = "./cache/sets";
  private const string _cacheSetMinifigPath = "./cache/setminifigs";
  private const string _cacheMinifigPath = "./cache/minifigs";

  private delegate Task<string> BrickGetter(string set_num, int? page = null, int? pageSize = null, CancellationToken cancellationToken = default);

  public RebrickableConnector(string credentialJsonFilename)
  {
    _serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower, WriteIndented = true };

    _credentials = JsonSerializer.Deserialize<RebrickableCredentials>(
      File.ReadAllText("./rebrickable.json"),
      _serializerOptions
    );

    _httpClient = new HttpClient();
    _httpClient.DefaultRequestHeaders.Add("Authorization", $"key {_credentials.Apikey}");

    _rebrickableClient = new(_httpClient);

    Directory.CreateDirectory(_cacheSetPath);
    Directory.CreateDirectory(_cacheSetMinifigPath);
    Directory.CreateDirectory(_cacheMinifigPath);

    // CHEATING
    _userToken = _credentials.UserToken;
  }

  public string GetToken()
  {
    if (_userToken == null)
    {
      Console.WriteLine("No user token found, getting one...");
      _userToken = _rebrickableClient.Users_tokenCreateAsync(
        _credentials.Username,
        _credentials.Password
      ).Result; // Hopefully this gets fixed soon. Errors out b/c http response is 200 instead of expected 201
    }
    return _userToken;
  }

  private List<T> GetListCached<T>(string setNumber, Func<string, List<T>> getter, string cachePath, bool overrideCache = false)
  {
    string cacheFilePath = Path.Combine(cachePath, $"{setNumber}.json");

    if (File.Exists(cacheFilePath) && overrideCache == false)
    {
      Console.WriteLine($" Minifig {setNumber} found in cache");
      return JsonSerializer.Deserialize<List<T>>(
        File.ReadAllText(cacheFilePath),
        _serializerOptions
      );
    }

    List<T> finalResults = getter(setNumber);

    File.WriteAllText(
      cacheFilePath,
      JsonSerializer.Serialize(finalResults, _serializerOptions)
    );

    return finalResults;
  }

  private List<T> GetResponse<T>(string identifier, BrickGetter brickGetMethod)
  {
    List<T> inventoryParts = [];
    int page = 0;

    RebrickListResponse<T> results;

    do
    {
      page++;

      Console.WriteLine($" Getting p{page}");
      string jsonResults = brickGetMethod(identifier, page: page).Result;

      results = JsonSerializer.Deserialize<RebrickListResponse<T>>(jsonResults, _serializerOptions);

      inventoryParts.AddRange(results.Results);

    } while (results?.Next != null);
    return inventoryParts;
  }


  public List<MinifigResponse> GetMinifigsOfSet(string setNumber)
  {
    List<MinifigResponse> setInventoryMinifigs = [];
    int page = 0;

    LegoSetsMinifigsListAsyncResponse inventory;

    do
    {
      page++;

      Console.WriteLine($"Getting set {setNumber} p{page}");
      inventory = _rebrickableClient.LegoSetsMinifigsListAsync(setNumber, page: page).Result;

      foreach (var minifig in inventory.Results)
      {
        setInventoryMinifigs.Add(new()
        {
          Name = minifig.SetName,
          Quantity = minifig.Quantity,
          SetNumber = minifig.SetNum,
          ImageUrl = new Uri(minifig.SetImgUrl)
        });
      }

    } while (inventory?.Next != null);

    return setInventoryMinifigs;
  }

  public List<MinifigResponse> GetMinifigsOfSetCached(string setNumber, bool overrideCache = false) => GetListCached<MinifigResponse>(setNumber,
      GetMinifigsOfSet,
      _cacheSetMinifigPath,
      overrideCache
    );

  public List<PartResponse> GetPartsOfMinifig(string setNumber, bool overrideCache = false) =>
    GetListCached<PartResponse>(setNumber,
      (string setNumber) => GetResponse<PartResponse>(setNumber, _rebrickableClient.LegoMinifigsPartsListAsync),
      _cacheMinifigPath,
      overrideCache);

  public List<PartResponse> GetPartsOfSet(string setNumber, bool overrideCache = false) =>
    GetListCached<PartResponse>(setNumber,
      (string setNumber) => GetResponse<PartResponse>(setNumber, _rebrickableClient.LegoSetsPartsListAsync),
      _cacheSetPath,
      overrideCache);

  public List<SetListResponse> GetUserSetlists() => GetResponse<SetListResponse>(_userToken,
    _rebrickableClient.UsersSetlistsListAsync);

  public List<SetResponse> GetSetsOfSetlist(string listId) => GetResponse<SetResponse>(listId,
      async (string listId, int? page, int? pageSize, CancellationToken cancellationToken) =>
      {
        string x = await _rebrickableClient.UsersSetlistsSetsListAsync(
          _userToken, listId,
          page: page,
          cancellationToken: cancellationToken);
        return x;
      }
    );
}
