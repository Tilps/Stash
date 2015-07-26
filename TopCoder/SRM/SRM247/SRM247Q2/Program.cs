using System;
using System.Collections;
using System.Text;

public class WordTrain
{
    private Hashtable lookup = new Hashtable();
    private Hashtable lookupLength = new Hashtable();

    private void AddLookup(string code, string res, int length)
    {
        lookup.Add(code, res);
        lookupLength.Add(code, length);
    }

    private int Longest(char startLetter, int start, string[] cars, out string res )
    {
        res = string.Empty;
        string code = start.ToString() + startLetter;
        if (lookup.Contains(code)) {
             res = (string)lookup[code];
             return (int)lookupLength[code];
        }
        if (start == cars.Length-1) {
            if (startLetter == '-' || startLetter == cars[start][0]) {
                res = cars[start];
                AddLookup(code, res, 1);
                return 1;
            }
            else {
                AddLookup(code, res, 0);
                return 0;
            }
        }
        int longest = 0;
        string tempRes = string.Empty;
        if (startLetter == '-' || cars[start][0] == startLetter)
        {
            longest = Longest(cars[start][cars[start].Length - 1], start + 1, cars, out tempRes);
            if (longest != 0) {
                tempRes = cars[start]+"-"+tempRes;
            }
            else
                tempRes = cars[start];
            longest++;
        }
        string nextRes = string.Empty;
        int nextLongest = Longest(startLetter, start + 1, cars, out nextRes);
        if (nextLongest > longest)
        {
            tempRes = nextRes;
            longest = nextLongest;
        }
        else if (nextLongest == longest)
        {
            if (string.Compare(nextRes, tempRes) < 0)
            {
                tempRes = nextRes;
            }
        }
        AddLookup(code, tempRes, longest);
        res = tempRes;
        return longest;
    }

    private string abr(string full)
    {
        return ""+full[0] + full[full.Length - 1];
    }

    public string hookUp(string[] cars)
    {
        for (int i = 0; i < cars.Length; i++)
        {
            char[] revCarChars = cars[i].ToCharArray();
            Array.Reverse(revCarChars);
            string revCar = new string(revCarChars);
            if (string.Compare(cars[i], revCar) > 0)
                cars[i] = revCar;
        }
        for (int i = 0; i < cars.Length; i++)
        {
            for (int j = i + 1; j < cars.Length; j++)
            {
                int comp = string.Compare(abr(cars[i]), abr(cars[j]));
                if (comp > 0 || (comp == 0 && string.Compare(cars[i], cars[j]) > 0))
                {
                    string temp = cars[i];
                    cars[i] = cars[j];
                    cars[j] = temp;
                }
            }
        }
        string res;
        Longest('-', 0, cars, out res);

        return res;
    }
}

namespace SRM247Q2
{
    class Program
    {
        static void Main(string[] args)
        {
            WordTrain blah = new WordTrain();
            Console.Out.WriteLine(blah.hookUp(new string[] { "ABA", "BBB", "COP", "COD", "BAD" }));
            Console.In.ReadLine();
        }
    }
}
