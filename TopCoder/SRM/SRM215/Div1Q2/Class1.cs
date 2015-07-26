using System;
using System.Collections;
using System.Text;


public class Thesaurus {

	public string[] edit(string[] entry) {
		ArrayList hashtables = new ArrayList();
		for (int i=0; i<entry.Length;i++) {
			string[] splits = entry[i].Split(' ');
			Hashtable hashtable = new Hashtable();
			for (int j=0; j<splits.Length;j++) {
				hashtable.Add(splits[j], splits[j]);
			}
			hashtables.Add(hashtable);
		}
		while (true){
			bool changed=false;
			for (int i=hashtables.Count-1;i>=0;i--) {
				Hashtable table = (Hashtable) hashtables[i];
				for (int j=0; j<i;j++) {
					Hashtable otherTable = (Hashtable) hashtables[j];
					int count =0;
					foreach (DictionaryEntry word in table) {
						if (otherTable.Contains((string)word.Value))
							count++;
					}
					if (count >= 2) {
						// merge
						foreach (DictionaryEntry word in table) {
							otherTable[(string)word.Value] = (string)word.Value;
						}
						hashtables.RemoveAt(i);
						changed = true;
						break;
					}
				}
			}
			if (!changed)
				break;
		}
		string[] results = new string[hashtables.Count];
		for (int i=0; i<hashtables.Count;i++) {
			Hashtable table = (Hashtable) hashtables[i];
			string[] words = new string[table.Count];
			int index=0;
			foreach (DictionaryEntry word in table) {
				words[index] = (string)word.Value;
				index++;
			}
			Array.Sort(words);
			results[i] = "";
			for (int j=0; j<words.Length;j++) {
				results[i] += words[j] + " ";
			}
			results[i] = results[i].Substring(0, results[i].Length-1);
		}
		Array.Sort(results);
		return results;
	}
}



	/// <summary>
	/// Summary description for Class1.
	/// </summary>
class Class1 {
	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main(string[] args) {
		Thesaurus a = new Thesaurus();
		string[] results = a.edit(new string[] {"a b c", "d e f", "g h i", "b g c e f"});
		for (int i=0; i< results.Length;i++) {
			Console.Out.WriteLine("{0}", 
				results[i]
				); 
		}
		Console.In.ReadLine();
	}
}
