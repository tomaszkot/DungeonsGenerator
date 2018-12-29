using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dungeons.Core
{
  public class GenericEventArgs<T> : EventArgs
  {
    public T EventData { get; set; }

    public GenericEventArgs(T EventData)
    {
      this.EventData = EventData;
    }

    public GenericEventArgs()
    {
    }
  }
}
