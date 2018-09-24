using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using trycross.Model;

namespace trycross.Model
{
    public interface ICanvasWriter
    {
        int getCount();
        Dictionary<int, char> getAns();
        string write(Canvas canvas);
    }
}