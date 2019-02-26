using Dungeons.Core;
using Dungeons.Tiles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Dungeons
{
  class AsciiPrinter : INodePrinter
  {
    DungeonNode node;

    public AsciiPrinter(DungeonNode node)
    {
      this.node = node;
    }

    public void PrintNewLine()
    {
      Console.Write(Environment.NewLine);
    }

    public void Print(Tile tile, PrintInfo pi)
    {
      var color = ConsoleColor.White;
      var symbol = ' ';
      if (tile != null)
      {
        color = tile.color;
        if (pi.PrintNodeIndexes)
        {
          Console.ForegroundColor = color;
          Console.Write(tile.dungeonNodeIndex);
          return;
        }
        if (tile.Revealed)
        {
          symbol = tile.Symbol;

        }
      }
      Console.ForegroundColor = color;
      Console.Write(symbol);
    }

    public void Print(INodePrinter p, PrintInfo pi)
    {
      for (int row = 0; row < node.Height; row++)
      {
        p.PrintNewLine();
        for (int col = 0; col < node.Width; col++)
        {
          if (col == 2 && row == 1)
          {
            int kk = 0;
          }

          var tile = node.GetTile(new Point(col, row));
          p.Print(tile, pi);
        }
      }
    }

    public virtual void Print(PrintInfo pi)
    {
      Print(this, pi);
    }
  }
}
