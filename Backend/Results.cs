using System;
using System.Collections.Generic;
using System.IO;

namespace Backend
{
    
    public enum SourceType
    {
        Binary,
        Code
    }

    public class Results
    {
        public SourceType Type { get; }
        public int TimeTaken { get; private set; }
        public int NumFunctions { get; private set; }
        public List<Function> Functions { get; set; } = new List<Function>();

        public Results(string directory, SourceType type)
        {
            Type = type;

            ReadProbs(directory);
            ReadHexCode(directory);
        }

        void ReadProbs(string directory)
        {
            NumFunctions = 0;
            StreamReader probs = new StreamReader(directory + "/" + FileNames.ProbName);
            using (probs)
            {
                try
                {
                    TimeTaken = int.Parse(probs.ReadLine());
                }
                catch(ArgumentNullException ex)
                {
                    TimeTaken = -1;
                }

                while (!probs.EndOfStream)
                {
                    Function function = new Function();
                    function.Prob = double.Parse(probs.ReadLine());
                    Functions.Add(function);
                    NumFunctions += 1;
                }
            }
        }
        
        void ReadHexCode(string directory)
        {
            StreamReader functions = new StreamReader(directory + "/" + FileNames.FuncName);
            using (functions)
            {
                int i = 0;
                while (!functions.EndOfStream)
                {
                    string line = functions.ReadLine();
                    string[] values = line.Split(",");
                    foreach(string value in values)
                    {
                        Functions[i].HexCode.Add(value);
                    }
                    i++;
                }
            }
        }
       

       
    }
}
