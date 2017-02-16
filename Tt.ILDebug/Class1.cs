using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tt.ILDebug
{
    public class Test
    {
        public Myclass F()
        {
            Myclass obj = new Myclass(10);
            return obj;
        }
    }

    public class Myclass
    {
        public Myclass(int x)
        {
        }
        public void F()
        {

        }
    }
}
