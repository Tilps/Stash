using System;
using System.Collections;
using System.Text;

public class SquareCypher
{
    public string decrypt(string cryptogram)
    {
        char[] letters = new char[cryptogram.Length];
        int squares = 0;
        for (int i = 0; i < letters.Length; i++)
        {
            if (i * i >= letters.Length)
                break;
            squares = i;
        }
        for (int i = 0; i <= squares; i++)
        {
            letters[i * i] = cryptogram[i];
        }
        int index = 0;
        for (int i = squares + 1; i < cryptogram.Length; i++)
        {
            while (isSquare(index))
                index++;
            letters[index] = cryptogram[i];
            index++;
        }
        return new string(letters);
    }

    private bool isSquare(int index)
    {
            for (int j = 0; j <= index; j++)
            {
                if (j * j == index)
                    return true;
            }
            return false;
    }
}

namespace Q3
{
    class Program
    {
        static void Main(string[] args)
        {

            SquareCypher c = new SquareCypher();
            object o = c.decrypt("0123456789");
            PrintObj(o);
            System.Console.In.ReadLine();
        }

        private static void PrintObj(object o)
        {
            if (o is IEnumerable)
            {
                foreach (object b in (IEnumerable)o)
                {
                    System.Console.Out.WriteLine(b);
                }
            }
            else
                System.Console.Out.WriteLine(o);
        }
    }
}
