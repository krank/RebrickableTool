using System.Collections;
using System.Security.Cryptography;

namespace LegoSetBricks;

public class SimpleMenu<T>
{
  private readonly Dictionary<T, string> _menuItems;
  private int currentSelection = 0;
  private T[] keys;
  private int offset = 2;

  public SimpleMenu(Dictionary<T, string> menuItems)
  {
    _menuItems = menuItems;
    keys = [.. _menuItems.Keys];
  }

  public T Choose()
  {
    currentSelection = 0;
    DisplayMenu("--- CHOOSE ---\n");

    Console.CursorVisible = false;
    UpdateCursor();

    ConsoleKey key = ConsoleKey.None;
    while (key != ConsoleKey.Enter)
    {
      key = Console.ReadKey(true).Key;

      if (key == ConsoleKey.DownArrow && currentSelection + 1 < _menuItems.Count)
      {
        currentSelection++;
      }
      else if (key == ConsoleKey.UpArrow && currentSelection > 0)
      {
        currentSelection--;
      }

      UpdateCursor();
    }
    
    Console.Clear();
    Console.CursorVisible = true;
    Console.CursorLeft = 0;
    Console.CursorTop = 0;

    return keys[currentSelection];
  }

  private void UpdateCursor()
  {
    Console.CursorLeft = 1;
    Console.Write(" ");
    Console.CursorTop = currentSelection + offset;
    Console.CursorLeft = 1;
    Console.Write(">");
  }

  private void DisplayMenu(string title)
  {
    Console.WriteLine(title);
    foreach (T key in keys)
    {
      Console.WriteLine($"   {_menuItems[key]}");
    }
  }
}
