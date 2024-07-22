global using System.Globalization;
global using CsvHelper;
global using System.Text.Json;
using LegoSetBricks;

RebrickableConnector connector = new("./rebrickable.json");

BrickCollection brickCollection = new();
SetCollection setCollection = new();


// setCollection.ReadFromCsv("collection.csv");
// setCollection.ReadFromCsv("fabuland.csv");

// var x = connector.GetMinifigsOfSetCached("6260-1");

// var x = connector.GetPartsOfSet("1462-1");
// var x = connector.GetPartsOfMinifig("fig-012594");

// connector.GetUserSetlists();

// Console.WriteLine("---- GETTING SETS FROM LISTS ----");
// List<SetResponse> classicSets = connector.GetSetsOfSetlist("734233");
// List<SetResponse> fabulandSets = connector.GetSetsOfSetlist("1067473");

// setCollection.Read(classicSets);
// setCollection.Read(fabulandSets);

// Console.WriteLine("---- ADDING ALL BRICKS TO COLLECTION----");
// setCollection.AddAllBricksTo(brickCollection, connector);

// Console.WriteLine("---- WRITING UPDATED BRICKS LIST TO CSV ----");
// brickCollection.WriteToCsv("result.csv");

Console.WriteLine("DONE");
Console.ReadLine();

// DO MANUALLY FIRST, AUTOMATE LATER
// Get user token
// Get user's setlists
// Choose which setlist to inventory
// Get list of all sets in setlist, including quantity // backup to json
// -----------------------------------------------------------------------------

/* TODO:
 - SetInfo: quantity, name, set_num
 - RebrickableConnector: GetUserSetlists(token): List<SetListInfo>
 - RebrickableConnector: GetUserSetlist(token, listId)
 - SetCollection: Add(List<SetInfo> sets)

 - Write replacement for Rebrickable library
*/
