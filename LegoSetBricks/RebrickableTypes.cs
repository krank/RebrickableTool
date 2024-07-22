using System.Text.Json.Serialization;

namespace LegoSetBricks;


public class RebrickListResponse<T>
{
  public int Count { get; set; }
  public Uri Next { get; set; }
  public Uri Previous { get; set; }
  public List<T> Results { get; set; }
}

public class MinifigResponse
{
  public int Quantity { get; set; }
  public string Name { get; set; }
  public string SetNumber { get; set; }
  public Uri ImageUrl { get; set; }
}


// -----------------------------------------------------------------------------

public class PartResponse
{
  public int Quantity { get; set; }
  public PartInfo Part { get; set; }
  public LegoColor Color { get; set; }
  public bool IsSpare { get; set; }
}

public class PartInfo
{
  public string Name { get; set; }

  [JsonPropertyName("part_num")]
  public string PartNumber { get; set; }

  [JsonPropertyName("part_img_url")]
  public Uri ImageUrl { get; set; }
}

// -----------------------------------------------------------------------------

public class SetResponse
{
  public int Quantity { get; set; }
  public bool IncludesSpares { get; set; }
  public SetInfo Set { get; set; }
}

public class SetInfo
{
  public string SetNum { get; set; }
  public string Name { get; set; }
}

// 
// PartResponse: Quantity, color
// PartInfo

// SetListing: Quantity
// SetInfo



public class LegoColor
{
  public int Id { get; set; }
  public string Name { get; set; }
  public string Rgb { get; set; }
}


public class SetListResponse
{
  public int Id { get; set; }
  public bool IsBuildable { get; set; }
  public string Name { get; set; }
  public int NumSets { get; set; }
}

public record RebrickableCredentials
{
  public string Username { get; set; }
  public string Password { get; set; }
  public string Apikey { get; set; }
  public string UserToken { get; set; }
}