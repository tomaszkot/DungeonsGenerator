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
  public class NodePresenter : INodePrinter
  {
    DungeonNode node;
    int top;
    int left;
    IPresenter presenter;

    public NodePresenter(DungeonNode node, IPresenter presenter, int left = -1, int top = -1)
    {
      this.presenter = presenter;
      this.node = node;
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
          presenter.Write(tile.dungeonNodeIndex);
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

    public virtual void RefreshPosition(PrintInfo pi, int x, int y)
    {
      if (pi == null)
        pi = new PrintInfo();
      Debug.Assert(PositionBasedPrinting());
      presenter.SetCursorPosition(left + x, top + y);
      var tile = node.GetTile(new Point(x, y));
      Print(tile, pi);
    }

    private bool PositionBasedPrinting()
    {
      return top >= 0 && left >= 0;
    }

    public virtual void Print(PrintInfo pi)
    {
      if (pi == null)
        pi = new PrintInfo();

      if (PositionBasedPrinting())
        presenter.SetCursorPosition(left, top);
      for (int row = 0; row < node.Height; row++)
      {
        PrintNewLine();
        if (PositionBasedPrinting())
          presenter.SetCursorPosition(left, top+row);
        for (int col = 0; col < node.Width; col++)
        {
          if (col == 2 && row == 1)
          {
            int kk = 0;
          }

          var tile = node.GetTile(new Point(col, row));
          Print(tile, pi);
        }
      }
    }

  }
}
