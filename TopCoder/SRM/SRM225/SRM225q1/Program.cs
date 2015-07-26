#region Using directives

using System;
using System.Collections;
using System.Text;

#endregion

public class ParameterSubstitution {

    public string processParams(string code, string[] param) {
        for (int i = param.Length; i > 0; i--) {
            code = code.Replace("$" + i.ToString(), param[i-1]);
        }
        return code;
    }

}


namespace SRM225q1 {
    class Program {
        static void Main(string[] args) {
//#define REAL_DEBUG
#if REAL_DEBUG
            a c = new a();
            Console.Out.WriteLine(c.b());
            Console.In.ReadLine();
#endif

        }
    }
}
