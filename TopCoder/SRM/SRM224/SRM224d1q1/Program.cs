#region Using directives

using System;
using System.Collections;
using System.Text;

#endregion

public class TwoTurtledoves {

    public int presentType(int n) {
        int present = 0;
        int maxType = 1;
        while (true) {
            for (int i = maxType; i >= 1; i--) {
                present += i;
                if (present >= n)
                    return i;
            }
            maxType++;
        }
        return -1;
    }

}



namespace SRM224d1q1 {
    class Program {
        static void Main(string[] args) {

//#define REAL_DEBUG
#if REAL_DEBUG
            a c = new a();
            Console.Out.WriteLine(a.b());
            Console.In.ReadLine();
#endif

        }
    }
}
