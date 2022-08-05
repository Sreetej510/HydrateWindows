// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;

string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Hydrate");
string DATA_FILE = Path.Combine(path, "database.json");
string POST_FILE = Path.Combine(path, "post_request.json");


Dictionary<string, Dictionary<string, object>> dataFull = new Dictionary<string, Dictionary<string, object>> { };
try
{
	FileStream fileStream = File.Open(DATA_FILE, FileMode.OpenOrCreate);
	StreamReader streamReader = new StreamReader(fileStream, System.Text.Encoding.UTF8);
	dataFull = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(streamReader.ReadLine());
	fileStream.Close();
}
catch (Exception) { }

string id = "dailyProgress";
string tmpStr = JsonConvert.SerializeObject(dataFull);
var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(tmpStr);
Console.WriteLine(result);

