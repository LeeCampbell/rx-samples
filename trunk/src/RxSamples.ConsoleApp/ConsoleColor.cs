using System;

namespace RxSamples.ConsoleApp
{
  /// <summary>
  /// Creates a scope for a console foreground color. When disposed, will return to the previous <see cref="Console.ForegroundColor"/>.
  /// </summary>
  public class ConsoleColor :IDisposable
  {
    private readonly System.ConsoleColor _previousColor;

    public ConsoleColor(System.ConsoleColor color)
    {
      _previousColor = Console.ForegroundColor;
      Console.ForegroundColor = color;
    }

    public void Dispose()
    {
      Console.ForegroundColor = _previousColor;
    }
  }
}