using Dungeons.Core;
using Dungeons.Tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Dungeons.ASCIIPresenters
{
  public class DungeonPresenter
  {
    readonly int top;
    readonly int left;
    IDrawingEngine presenter;

    public DungeonPresenter(IDrawingEngine presenter, int left = 0, int top = 0)
    {
      this.presenter = presenter;
      this.top = top;
      this.left = left;
      presenter.CursorVisible = false;
    }

    public void PrintNewLine()
    {
      presenter.Write(Environment.NewLine);
    }

    public void Print(Tile tile, PrintInfo pi)
    {
      var color = ConsoleColor.White;
      var symbol = ' ';
      if (tile != null)
      {
        color = tile.Color;
        if (pi.PrintNodeIndexes)
        {
          presenter.ForegroundColor = color;
          presenter.Write(tile.DungeonNodeIndex);
          return;
        }
        if (tile.Revealed)
        {
          symbol = tile.Symbol;

        }
      }
      presenter.ForegroundColor = color;
      presenter.Write(symbol);
    }

    public virtual void RefreshPosition(DungeonNode Node, PrintInfo pi, int x, int y)
    {
      if (pi == null)
        pi = new PrintInfo();
      presenter.SetCursorPosition(left + x, top + y);
      var tile = Node.GetTile(new Point(x, y));
      Print(tile, pi);
    }

    public virtual void Redraw(DungeonNode node, PrintInfo pi)
    {
      if (pi == null)
        pi = new PrintInfo();
      
      presenter.SetCursorPosition(left, top);
      for (int row = 0; row < node.Height; row++)
      {
        PrintNewLine();
        presenter.SetCursorPosition(left, top+row);
        for (int col = 0; col < node.Width; col++)
        {
          var tile = node.GetTile(new Point(col, row));
          if (tile != null && tile.Symbol == '@')
          {
            int kk = 0;
            kk++;
          }
          Print(tile, pi);
        }
      }
    }

  }
}
