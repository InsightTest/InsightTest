using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Test
{
    class Program
    {
        static Dictionary<int, Dictionary<string, decimal>> actualValues;
        static Dictionary<int, Dictionary<string, decimal>> predictedValues;
        static Dictionary<int, Dictionary<decimal, int>> errorValues = new Dictionary<int, Dictionary<decimal, int>>();
        static void Main(string[] args)
        {
            var path = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName;
            var actualFilePath = Path.Combine(path, "input\\actual.txt");
            var predictedFilePath = Path.Combine(path, "input\\predicted.txt");
            var windowFilePath = Path.Combine(path, "input\\window.txt"); 

           var windowHour = CalculateWindowHour(windowFilePath);
           actualValues = ReadStockValues(actualFilePath);
           predictedValues = ReadStockValues(predictedFilePath);
           CalculateError();
           var output=  CreateOutput(windowHour);
           using (StreamWriter outputFile = new StreamWriter(Path.Combine(path, "output\\comparision.txt")))
            {
                foreach (string line in output)
                outputFile.WriteLine(line);
            }
        }

        public static Dictionary<int, Dictionary<string, decimal>> ReadStockValues(string path)
        {
            string line;
            string[] lineValues = new string[3];
            Dictionary<int, Dictionary<string, decimal>> Values = new Dictionary<int, Dictionary<string, decimal>>();
            StreamReader file = new StreamReader(path);

            while (!String.IsNullOrEmpty(line = file.ReadLine()))
            {
                lineValues = line.Split('|');
                var hour = Int32.Parse(lineValues[0]);
                var value = decimal.Parse(lineValues[2]);
                if (Values.ContainsKey(hour))
                {
                    var shareObject = Values[hour];
                    shareObject.Add(lineValues[1], value);
                }
                else
                {
                    Values.Add(hour, new Dictionary<string, decimal>());
                    var shareObject = Values[hour];
                    shareObject.Add(lineValues[1], value);
                }
            }
            return Values;
        }

        public static void CalculateError()
        {
            List<int> predictedHours = predictedValues.Keys.ToList();
            var predictDict = new Dictionary<string, decimal>();
            var actualDict = new Dictionary<string, decimal>();
            
            foreach (var i in predictedHours)
            {
                decimal errorValue = 0;
                predictDict = predictedValues[i];
                actualDict = actualValues[i];
                var keys = predictDict.Keys;
                foreach (var j in keys)
                {
                    errorValue = errorValue + Math.Abs(predictDict[j] - actualDict[j]);                    
                }
                var dict = new Dictionary<decimal, int>();
                dict.Add(errorValue, keys.Count);
                errorValues.Add(i, dict);
            }
        }

        public static int CalculateWindowHour(string windowFilePath)
        {
            int window;
            string line;
            StreamReader file = new StreamReader(windowFilePath);
            line = file.ReadLine();
            window = Int32.Parse(line);
            return window;
        }

        public static List<string> CreateOutput(int window)
        {

            int hour = window;
            int counter = 0;
            List<string> outputList = new List<string>();
            List<int> iteratorList = errorValues.Keys.ToList();
            for (var j = 0; j <= iteratorList.Count - window; j++)
            {
                decimal errors = 0;
                decimal numOfValues = 0;

                for (var i = j; i < iteratorList.Count; i++)
                {
                    var val = iteratorList[i];                   
                    if (counter < window)
                    {
                        counter++;
                        var dict = errorValues[val];
                        errors = errors + dict.Keys.Sum();
                        numOfValues = numOfValues + dict.Values.Sum();

                        if (i == iteratorList.Count-1)
                        {
                            counter = 0;
                            var averageValue = Math.Round(errors / numOfValues, 2);
                            var outputValue = iteratorList[j] + "|" + iteratorList[i] + "|" + averageValue;
                            outputList.Add(outputValue);
                        }
                    }
                    else
                    {
                        counter = 0;
                        var averageValue = Math.Round(errors / numOfValues,2);
                        var outputValue = iteratorList[j] + "|" + iteratorList[i-1] + "|" + averageValue;
                        outputList.Add(outputValue);
                        break;
                    }
                }

            }

            return outputList;
            
        }
    }
}
