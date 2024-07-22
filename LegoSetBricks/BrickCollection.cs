

namespace LegoSetBricks;

public class BrickCollection // Or "Brickollection" if you will (this is the main one)
{
  private readonly Dictionary<(string pNum, string cName), Part> _parts;

  public BrickCollection()
  {
    _parts = new();
  }

  public void AddBricks(List<PartResponse> newParts)
  {
    foreach (PartResponse inventoryPart in newParts)
    {
      (string pNum, string cName) partKey = (inventoryPart.Part.PartNumber, inventoryPart.Color.Name);

      if (!_parts.TryGetValue(partKey, out Part existingPart))
      {
        Part part = new Part()
        {
          PartNumber = inventoryPart.Part.PartNumber,
          Name = inventoryPart.Part.Name,
          Color = inventoryPart.Color.Name,
          Quantity = !inventoryPart.IsSpare ? inventoryPart.Quantity : 0,
          SpareQuantity = inventoryPart.IsSpare ? inventoryPart.Quantity : 0,
          ImageUrl = inventoryPart.Part.ImageUrl
        };

        _parts.Add(partKey, part);
      }
      else
      {
        if (!inventoryPart.IsSpare) existingPart.Quantity += inventoryPart.Quantity;
        else
          existingPart.Quantity += inventoryPart.Quantity;
      }
    }
  }

  public void WriteToCsv(string csvPath)
  {
    using StreamWriter writer = new StreamWriter(csvPath);
    using CsvWriter csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

    csvWriter.WriteRecords(_parts);
  }

  public void ReadFromCsv(string csvPath)
  {
    using StreamReader reader = new StreamReader(csvPath);
    using CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

    _parts.Clear();

    IEnumerable<Part> records = csvReader.GetRecords<Part>();
    foreach (Part record in records)
    {
      (string pNum, string cName) partKey = (record.PartNumber, record.Color);
      _parts.Add(partKey, record);
    }
  }

  public int GetNumberOfParts(bool includeSpares = true)
  {
    int num = 0;
    foreach (Part part in _parts.Values)
    {
      num += part.Quantity;
      if (includeSpares) num += part.SpareQuantity;
    }

    return num;
  }

  public int GetNumberOfUniqueParts()
  {
    return _parts.Values.Count;
  }

  private record Part()
  {
    public string PartNumber { get; init; }
    public string Name { get; set; }
    public string Color { get; init; }
    public int Quantity { get; set; }
    public int SpareQuantity { get; set; }
    public Uri ImageUrl { get; init; }
  }
}
