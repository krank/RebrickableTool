using System.Reflection.Metadata;
using CsvHelper.Configuration.Attributes;

namespace LegoSetBricks;

public class SetCollection
{
  private List<Set> _sets;

  public SetCollection()
  {
    _sets = new();
  }

  /// <summary>
  /// Reads a csv downloaded from Rebrickable, loads all sets as records
  /// </summary>
  /// <param name="csvPath"></param>
  public void ReadFromCsv(string csvPath)
  {
    using StreamReader reader = new StreamReader(csvPath);
    using CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

    csvReader.Read();
    csvReader.ReadHeader();

    IEnumerable<Set> records = csvReader.GetRecords<Set>();

    foreach(Set set in records)
    {
      _sets.Add(set);
    }
  }

  public void Read(List<SetResponse> sets)
  {
    foreach (SetResponse set in sets)
    {
      _sets.Add(new(
        SetNum: set.Set.SetNum,
        Quantity: set.Quantity,
        IncludesSpares: set.IncludesSpares,
        1 // Because responses via API for some reason don't include inventory version info
      ));
    }
  }

  public void Empty() => _sets.Clear();

  public record Set(
    [Name("Set Number")] string SetNum,
    [Name("Quantity")] int Quantity,
    [Name("Includes Spares")] bool IncludesSpares,
    [Name("Inventory Ver")] int InventoryVer
  );

  public void AddAllBricksTo(BrickCollection brickCollection, RebrickableConnector rebrickableConnector)
  {
    int n = 1;
    foreach (Set set in _sets)
    {
      Console.WriteLine($"Set {set.SetNum} x {set.Quantity} [{n}/{_sets.Count}]");

      // Start by getting the base bricks
      List<PartResponse> bricks = rebrickableConnector.GetPartsOfSet(set.SetNum);

      // Then get all the minifigs
      List<MinifigResponse> minifigs = rebrickableConnector.GetMinifigsOfSetCached(set.SetNum);

      foreach(MinifigResponse minifig in minifigs)
      {
        List<PartResponse> minifigParts = rebrickableConnector.GetPartsOfMinifig(minifig.SetNumber);

        for (int i = 0; i < minifig.Quantity; i++)
        {
          bricks.AddRange(minifigParts);
        }
      }

      // Finally add as many copies as relevant
      for (int i = 0; i < set.Quantity; i++)
      {
        brickCollection.AddBricks(bricks);
      }
      n++;
    }
  }
}
