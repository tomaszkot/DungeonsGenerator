using Dungeons.Tiles;
using System;
using System.Drawing;

namespace Dungeons.ASCIIDisplay.Presenters
{
  public class DungeonPresenter
  {
    readonly int top;
    readonly int left;
    IDrawingEngine drawingEngine;

    public DungeonPresenter(IDrawingEngine drawingEngine, int left = 0, int top = 0)
    {
      this.drawingEngine = drawingEngine;
      this.top = top;
      this.left = left;
      drawingEngine.CursorVisible = false;
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
          drawingEngine.ForegroundColor = color;
          drawingEngine.Write(tile.DungeonNodeIndex);
          return;
        }
        if (tile.Revealed)
        {
          symbol = tile.Symbol;
        }
        //else
        //  Debug.WriteLine("tile.Revealed " + tile);
      }
      drawingEngine.ForegroundColor = color;
      drawingEngine.Write(symbol);
    }

    public virtual void RefreshPosition(DungeonNode Node, PrintInfo pi, int x, int y)
    {
      if (pi == null)
        pi = new PrintInfo();
      drawingEngine.SetCursorPosition(left + x, top + y);
      var tile = Node.GetTile(new Point(x, y));
      Print(tile, pi);
    }

    public virtual void Redraw(DungeonNode node, PrintInfo pi)
    {
      if (pi == null)
        pi = new PrintInfo();
      
      drawingEngine.SetCursorPosition(left, top);
      for (int row = 0; row < node.Height; row++)
      {
        drawingEngine.SetCursorPosition(left, top+row);
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
