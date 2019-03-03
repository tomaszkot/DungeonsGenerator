namespace Dungeons.ASCIIDisplay.Presenters
{
  public abstract class Item
  {
    IDrawingEngine drawingEngine;
    public int CurrentX { get; set; }
    public int CurrentY { get; set; }

    public Item(int x, int y)
    {
      this.OriginPositionX = x;
      this.OriginPositionY = y;
      CurrentX = OriginPositionX;
      CurrentY = OriginPositionY;
    }

    public int OriginPositionX { get; set; }
    public int OriginPositionY { get; set; }
    public IDrawingEngine DrawingEngine { get => drawingEngine; set => drawingEngine = value; }

    protected void Reset()
    {
      CurrentX = OriginPositionX;
      CurrentY = OriginPositionY;
      UpdatePresenterPos();
    }

    protected void WriteLine(string line)
    {
      UpdatePresenterPos();
      var debug = "";// " (at " + CurrentX + ", " + CurrentY;
      DrawingEngine.WriteLine(line + " " + debug);
      CurrentX = OriginPositionX;
      CurrentY++;
      UpdatePresenterPos();
    }

    private void UpdatePresenterPos()
    {
      DrawingEngine.SetCursorPosition(CurrentX, CurrentY);
    }

    public abstract void Redraw(IDrawingEngine drawingEngine);

    public abstract int TotalHeight
    {
      get;
    }
  }
}
