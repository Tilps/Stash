using System;
using System.Text;
using System.Collections;



public class TCMinMin {

	Hashtable intTypes = new Hashtable();
	Hashtable stringTypes = new Hashtable();

	Hashtable links = new Hashtable();
	string currentContext = "";

	private string createVar(string var) {
		return createVar(currentContext,var);
	}

	private string createVar(string func, string var) {
		return func+"_"+var;
	}

	Hashtable funcs = new Hashtable();

	private void AddRealLink(string rv1, string rv2) {
		if (rv1 != rv2) {
			links[rv1+":"+rv2] = 1;
			links[rv2+":"+rv1] = 1;
		}
	}

	private void AddSimpleLink(string var1, string var2) {
		AddRealLink(createVar(var1), createVar(var2));
	}
	private void AddLink(string var1, string func, int index) {
		ArrayList funcsfunc = (ArrayList)funcs[func];
		AddRealLink(createVar(var1), createVar(func, (string)funcsfunc[index]));
	}
	private void AddRetLink(string var1, string func) {
		AddRealLink(createVar(var1), createVar(func, "return"));
	}

	private string CheckVar(string var) {
		if (intTypes.Contains(var)) {
			return ":int";
		}
		else if (stringTypes.Contains(var)) {
			return ":string";
		}
		else 
			throw new Exception("Impossible");
	}

	private bool ExistsVar(string var) {
		if (intTypes.Contains(var)) {
			return true;
		}
		else if (stringTypes.Contains(var)) {
			return true;
		}
		else 
			return false;
	}

	public string[] deduceTypes(string[] program) {
		// parse

		for (int i=0; i<program.Length;i++) {
			if (program[i].StartsWith("function ")) {
				string[] split1 = program[i].Split(' ');
				string func = ""+split1[1][0];
				string reduce = split1[1].Substring(2, split1[1].LastIndexOf(')')-2);
				if (reduce.Length > 0) {
					string[] split2 = reduce.Split(',');
					funcs.Add(func, new ArrayList());
					ArrayList list = (ArrayList)funcs[func];
					for (int j=0; j<split2.Length;j++) {
						list.Add(""+split2[j][0]);
					}
				}
				
			}
		}
		for (int i=0; i<program.Length;i++) {
			if (program[i].StartsWith("function ")) {
				string[] split1 = program[i].Split(' ');
				string func = ""+split1[1][0];
				currentContext = func;
				string[] splitret = split1[1].Split(')');
				if (splitret[1].Length > 0) {
					string retreduce = splitret[1].Substring(1);
					if (retreduce == "int") {
						intTypes[createVar("return")] = 1;
					}
					else if (retreduce == "string") {
						stringTypes[createVar("return")] = 1;
					}
				}
				string reduce = splitret[0].Substring(2);
				if (reduce.Length > 0) {
					string[] split2 = reduce.Split(',');
					for (int j=0; j<split2.Length;j++) {
						string[] splits = split2[j].Split(':');
						if (splits.Length > 1) {
							if (splits[1] == "string")
								stringTypes[createVar(splits[0])] = 1;
							else if (splits[1] == "int")
								intTypes[createVar(splits[0])] = 1;
						}
					}
				}
			}
			else if (program[i].StartsWith("return ")) {
				string[] splits = program[i].Split(' ');
				if (splits[1][0] == '\"') {
					stringTypes[createVar("return")] = 1;
				}
				else if (splits[1].IndexOfAny(new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'}) != -1) {
					intTypes[createVar("return")] = 1;
				}
				else
					AddSimpleLink(splits[1], "return");
			}
			else if (program[i].IndexOf('\"') != -1) {
				stringTypes[createVar(""+program[i][0])] = 1;
			}
			else if (program[i].IndexOf(':') != -1) {
				string[] splits = program[i].Split(':');
				if (splits[1] == "string")
					stringTypes[createVar(splits[0])] = 1;
				else if (splits[1] == "int")
					intTypes[createVar(splits[0])] = 1;
			}
			else if (program[i].IndexOf('(') != -1) {
				string[] split1 = program[i].Split('=');
				string localVar = split1[0];
				string func = ""+split1[1][0];
				AddRetLink(localVar, func);
				string reduce = split1[1].Substring(2, split1[1].Length-3);
				if (reduce.Length > 0) {
					string[] split2 = reduce.Split(',');
					for (int j=0; j<split2.Length;j++) {
						AddLink(split2[j], func, j);
					}
				}
			}
			else if (program[i].IndexOfAny(new char[] {'+', '-', '/', '*'}) != -1) {
				if (program[i][3]=='/'||program[i][3]=='*') {
					intTypes[createVar(""+program[i][0])] = 1;
				}
				AddSimpleLink(""+program[i][0], ""+program[i][2]);
				AddSimpleLink(""+program[i][2], ""+program[i][4]);
				AddSimpleLink(""+program[i][0], ""+program[i][4]);
			}
			else if (program[i].IndexOfAny(new char[] {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'}) != -1) {
				intTypes[createVar(""+program[i][0])] = 1;
			}
			else {
				AddSimpleLink(""+program[i][0], ""+program[i][2]);
			}

		}

		bool changed = true;
		while (changed) {
			changed=false;
			ArrayList toRemove = new ArrayList();
			foreach (DictionaryEntry de in links) {
				string[] splits = ((string)de.Key).Split(':');
				if (intTypes.Contains(splits[0])) {
					if (!intTypes.Contains(splits[1])) {
						intTypes.Add(splits[1], 1);
						changed=true;
					}
					toRemove.Add(de.Key);
					continue;
				}
				if (stringTypes.Contains(splits[0])) {
					if (!stringTypes.Contains(splits[1])) {
						stringTypes.Add(splits[1], 1);
						changed=true;
					}
					toRemove.Add(de.Key);
					continue;
				}
			}
			if (!changed)
				break;
			foreach (object obj in toRemove) {
				links.Remove(obj);
			}
		}

		// reconstruct.
		ArrayList results = new ArrayList();
		for (int i=0; i<program.Length;i++) {
			if (program[i].StartsWith("function ")) {
				StringBuilder result = new StringBuilder();
				string[] split1 = program[i].Split(' ');
				string func = ""+split1[1][0];
				currentContext = func;
				result.Append("function ");
				result.Append(func);
				result.Append("(");
				string[] splitret = split1[1].Split(')');
				string reduce = splitret[0].Substring(2);
				string[] split2 = reduce.Split(',');
				ArrayList used = new ArrayList();
				if (reduce.Length > 0) {
					for (int j=0; j<split2.Length;j++) {
						string[] splits = split2[j].Split(':');
						if (j > 0) {
							result.Append(",");
						}
						used.Add(splits[0][0]);
						result.Append(splits[0]);
						result.Append(CheckVar(createVar(splits[0])));
					}
				}
				result.Append(")");
				string retVar = createVar("return");
				result.Append(CheckVar(retVar));
				results.Add(result.ToString());
				for (char var = 'a'; var <= 'z'; var++) {
					if (used.Contains(var))
						continue;
					StringBuilder result2 = new StringBuilder();
					if (ExistsVar(createVar(""+var))) {
						result2.Append(var);
						result2.Append(CheckVar(createVar(""+var)));
						results.Add(result2.ToString());
					}
				}
			}
		}
		return (string[])results.ToArray(typeof(string));
	}
}



	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class Class1
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			TCMinMin t = new TCMinMin();
			string[] results = t.deduceTypes(new string[] {"function f(a,b,c)", "g=a(a)", "f=b+c", "return f", "function a(a:string)", "return a", "function b()", "a=f(b,a,c)", "a=0123456789012345678901234567890", "return a"}
				);

			for (int i=0; i<results.Length;i++) {
				Console.Out.WriteLine("{0}", results[i]
					);
				}
			Console.In.ReadLine();
		}
	}
