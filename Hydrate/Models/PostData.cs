using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Hydrate.Models
{
    internal class PostData
    {
        private Dictionary<string, object> body;
        private string path;
        private Dictionary<string, object> result = null;
        private static bool serverDown = false;
        private static Dictionary<string, object> data;

        public PostData(Dictionary<string, object> body)
        {
            this.body = body;
            path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Hydrate");
            Directory.CreateDirectory(path);
        }

        public Dictionary<string, object> getResult()
        {
            return result;
        }

        private bool isConnectedToServer(String url, int port, int timeout)
        {
            try
            {
                TcpClient client = new TcpClient();

                var success = client.BeginConnect(url, port, null, null);
                success.AsyncWaitHandle.WaitOne(timeout);
                if (client.Connected)
                {
                    client.EndConnect(success);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string Post(string url, Dictionary<string, object> obj, bool forResult)
        {
            string Data;
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(obj);
                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                Data = streamReader.ReadToEnd();
                if (forResult)
                {
                    result = JsonConvert.DeserializeObject<Dictionary<string, object>>(Data);
                }
            }
            return Data;

        }

        public void Run()
        {
            string url;
            if (isConnectedToServer("ohio.omegarts.de", 1269, 1000) && !serverDown)
            {
                url = "http://ohio.omegarts.de:1269";
            }
            else if (isConnectedToServer("local.omegarts.de", 1269, 500) && !serverDown)
            {
                url = "http://local.omegarts.de:1269";
            }
            else
            {
                serverDown = true;
                url = null;
            }

            string DATA_FILE = Path.Combine(path, "database.json");
            string POST_FILE = Path.Combine(path, "post_request.json");

            if (!serverDown)
            {
                string query = (string)body.GetValueOrDefault("queryType");
                bool forResult = false;
                if (query == "find")
                {
                    forResult = true;
                }

                try
                {
                    Post(url, body, forResult);
                    new Thread(() =>
                    {
                        try
                        {
                            var obj = Database.createBody("find", "", false, "", "");
                            var res = Post(url, obj, false);
                            FileStream fileStream = File.Open(DATA_FILE, FileMode.Create);
                            StreamWriter streamWriter = new StreamWriter(fileStream, System.Text.Encoding.UTF8);
                            streamWriter.Write(res);
                            streamWriter.Flush();
                            fileStream.Close();
                        }
                        catch (Exception) { }
                    }).Start();
                }
                catch (Exception) { }

            }
            else
            {
                Dictionary<string, Dictionary<string, object>> dataFull = new Dictionary<string, Dictionary<string, object>> { };
                try
                {
                    FileStream fileStream = File.Open(DATA_FILE, FileMode.OpenOrCreate);
                    StreamReader streamReader = new StreamReader(fileStream, System.Text.Encoding.UTF8);
                    dataFull = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(streamReader.ReadLine());
                    fileStream.Close();
                }
                catch (Exception) { }

                string query = (string)body.GetValueOrDefault("queryType");

                try
                {
                    if (query == "find" && dataFull != null)
                    {
                        var filter = (Dictionary<string, object>)body.GetValueOrDefault("filter");
                        string id = (string)filter.First().Value;

                        string tmpStr = JsonConvert.SerializeObject(dataFull[id]);
                        result = JsonConvert.DeserializeObject<Dictionary<string, object>>(tmpStr);
                    }
                    else if (query == "update" && dataFull != null)
                    {
                        var filter = (Dictionary<string, object>)body.GetValueOrDefault("filter");
                        string id = (string)filter.First().Value;

                        string tmpStr = JsonConvert.SerializeObject(dataFull[id]);

                        var valObj1 = JsonConvert.DeserializeObject<Dictionary<string, Object>>(tmpStr);
                        var valObj2 = new Dictionary<string, Dictionary<string, Object>> { };
                        bool logUpdate = false;
                        string opVal = (string)(body.GetValueOrDefault("operator"));

                        var value = (Dictionary<string, object>)body.GetValueOrDefault("document");
                        string name = value.First().Key;
                        if (name.Contains("log"))
                        {
                            valObj2 = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Object>>>(tmpStr);
                            logUpdate = true;
                        }

                        if (opVal == "$set")
                        {
                            if (logUpdate)
                            {
                                valObj2["log"][name.Split(".")[0]] = value.First().Value;
                            }
                            else
                            {
                                valObj1[name] = value.First().Value;
                            }
                        }
                        else
                        {
                            if (logUpdate)
                            {
                                valObj2["log"].Remove(name.Split(".")[0]);
                            }
                            else
                            {
                                valObj1.Remove(name);
                            }
                        }

                        if (logUpdate)
                        {
                            dataFull[id] = JsonConvert.DeserializeObject<Dictionary<string, Object>>(JsonConvert.SerializeObject(valObj2));
                        }
                        else
                        {
                            dataFull[id] = valObj1;
                        }

                        try
                        {
                            FileStream fs = File.Open(DATA_FILE, FileMode.Create);
                            StreamWriter streamWriter = new StreamWriter(fs, System.Text.Encoding.UTF8);
                            string writeData = JsonConvert.SerializeObject(dataFull);
                            streamWriter.Write(JsonConvert.SerializeObject(dataFull));
                            streamWriter.Flush();
                            fs.Close();
                        }
                        catch (Exception e) {
                            Console.WriteLine(e.Message);
                            if (e.Message == "je")
                            {
                                Console.WriteLine(e.Message);
                            }
                        }

                        new Thread(() =>
                        {
                            try
                            {
                                FileStream fs = File.Open(POST_FILE, FileMode.Append);
                                StreamWriter streamWriter = new StreamWriter(fs, System.Text.Encoding.UTF8);
                                streamWriter.WriteLine(JsonConvert.SerializeObject(body));
                                streamWriter.Flush();
                                fs.Close();
                            }
                            catch (Exception) {}
                        }).Start();

                    }
                }catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    if (e.Message == "je")
                    {
                        Console.WriteLine(e.Message);
                    }
                }

                new Thread(() =>
                {
                    serverDown = !(isConnectedToServer("local.omegarts.de", 1269, 500) || isConnectedToServer("ohio.omegarts.de", 1269, 1000));
                    if (!serverDown)
                    {
                        if (isConnectedToServer("ohio.omegarts.de", 1269, 1000))
                        {
                            url = "http://ohio.omegarts.de:1269";
                        }
                        else if (isConnectedToServer("local.omegarts.de", 1269, 500))
                        {
                            url = "http://local.omegarts.de:1269";
                        }
                        new Thread(() =>
                        {
                            try
                            {
                                FileStream fileStream = File.Open(POST_FILE, FileMode.OpenOrCreate);
                                StreamReader streamReader = new StreamReader(fileStream, System.Text.Encoding.UTF8);
                                string line;
                                while ((line = streamReader.ReadLine()) != null)
                                {
                                    var d = JsonConvert.DeserializeObject<Dictionary<string, object>>(line);
                                    Post(url, d, false);
                                }
                                fileStream.SetLength(0);
                                fileStream.Close();
                            }
                            catch (Exception) { }

                        }).Start();
                    }
                }).Start();

            }

        }
    }
}