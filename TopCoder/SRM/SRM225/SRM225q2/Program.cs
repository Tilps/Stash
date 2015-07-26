#region Using directives

using System;
using System.Collections;
using System.Text;

#endregion

public class ComboBoxKeystrokes {

    public int minimumKeystrokes(string[] elements) {
        int worst = 0;
        for (int i = 0; i < elements.Length; i++) {
            elements[i] = elements[i].ToLower();
        }
        int[,] minimums = new int[elements.Length, elements.Length];
        ArrayList todoFrom = new ArrayList();
        ArrayList todoTo = new ArrayList();
        ArrayList freshFrom = new ArrayList();
        ArrayList freshTo = new ArrayList();
        for (int i = 0; i < elements.Length; i++) {
            for (int j = 0; j < elements.Length; j++) {
                minimums[i, j] = 1000000;
                if (i == j) {
                    minimums[i, j] = 0;
                    todoFrom.Add(i);
                    todoTo.Add(j);
                }
            }
        }
        while (todoFrom.Count > 0) {
            for (int i = 0; i < todoFrom.Count; i++) {
                int from = (int)todoFrom[i];
                int to = (int)todoTo[i];
                int startVal = minimums[from, to];
                int nextVal = startVal + 1;
                for (char letter = 'a'; letter <= 'z'; letter++) {
                    int point = to;
                    while (true) {
                        point++;
                        if (point >= elements.Length)
                            point = 0;
                        if (point == to)
                            break;
                        if (elements[point][0] == letter)
                            break;
                    }
                    if (point == to)
                        continue;
                    if (minimums[from, point] > nextVal) {
                        minimums[from, point] = nextVal;
                        freshTo.Add(point);
                        freshFrom.Add(from);
                    }
                }
            }
            todoFrom = freshFrom;
            todoTo = freshTo;
            freshFrom = new ArrayList();
            freshTo = new ArrayList();
        }

        for (int i = 0; i < elements.Length; i++) {
            for (int j = 0; j < elements.Length; j++) {
                if (minimums[i, j] > worst)
                    worst = minimums[i, j];
            }
        }
        return worst;
    }

}


namespace SRM225q2 {
    class Program {
        static void Main(string[] args) {
            ComboBoxKeystrokes c = new ComboBoxKeystrokes();
            Console.Out.WriteLine(c.minimumKeystrokes(new string[] { "alpha", "beta", "gamma", "delta" }));
            Console.In.ReadLine();
        }
    }
}
