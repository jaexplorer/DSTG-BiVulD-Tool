using System.Collections.Generic;
using System.IO;

namespace Backend
{
    public class Function
    {
        public double Prob { get; set; }
        public List<string> HexCode { get; set; }
        public Function()
        {
            Prob = 0;
            HexCode = new List<string>();
        }
           
    }
}
