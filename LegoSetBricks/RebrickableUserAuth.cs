using System.Text.Json.Serialization;

namespace LegoSetBricks;

public class RebrickableUserAuth
{
  [JsonPropertyName("username")]
  public string Username { get; set; }

  [JsonPropertyName("password")]
  public string Password { get; set; }
}
