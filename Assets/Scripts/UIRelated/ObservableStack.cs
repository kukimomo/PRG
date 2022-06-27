using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.PlayerLoop;

public delegate void UpdateStackEvent();

public class ObservableStack<T>:Stack<T>
{
    public event UpdateStackEvent OnPush;
    public event UpdateStackEvent OnPop;//event that is raised when we pop something
    public event UpdateStackEvent OnClear;//event that is raised when we clear the stack

    public ObservableStack(ObservableStack<T> items) : base(items)
    {
        
    }

    public ObservableStack()
    {
        
    }
    public new void Push(T item)
    {
        base.Push(item);

        if (OnPush != null)
        {
            OnPush();
        }
    }

    public new T Pop()
    {
      T  Item = base.Pop();
      if (OnPop != null)
      {
          OnPop();
      }

      return Item;
    }

    public new void Clear()
    {
        base.Clear();
        if (OnClear != null)
        {
            OnClear();
        }
    }
}