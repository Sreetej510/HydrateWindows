using System;
using System.Collections.Generic;
using System.Threading;

namespace Hydrate.Models
{
    public class Database
    {
        public static string dbName;
        public static string collection;
        private PostData postData;
        private Thread threadPost;

        public static void DatabaseSet(string dbName, string collection)
        {
            Database.dbName = dbName;
            Database.collection = collection;
        }

        public static Dictionary<string,object> createBody(string query, string op , bool onlyFirst, object filter, object document)
        {
            return new Dictionary<string, object>()
                        {
                            {"dbname", dbName },
                            {"collection", collection },
                            { "queryType", query},
                            { "operator",op},
                            { "onlyFirst", onlyFirst},
                            { "filter", filter},
                            { "document", document}
                        };
        }

        public static Dictionary<string, object> createFilter(string query, object value)
        {
            return new Dictionary<string, object>() {
                        {query,value }
                    };
        }

        public Database post(string param, string value, string op, string query, bool onlyFirst, object document)
        {
            var filter = createFilter(param, value);
            var body = createBody(query,op, onlyFirst, filter, document);
            postData = new PostData(body);
            threadPost = new Thread(new ThreadStart(postData.Run));
            threadPost.Start();
            return this;
        }

        public void onSuccess(Action<Dictionary<string, object>> lambda)
        {
            threadPost.Join();
            new Thread(()=>onSuccessThread(lambda)).Start();
        }

        public void onSuccessSync(Action<Dictionary<string, object>> lambda)
        {
            threadPost.Join();
            onSuccessThread(lambda);
        }

        private void onSuccessThread(Action<Dictionary<string, object>> lambda)
        {
            lambda.Invoke(postData.getResult());
        }

    }
}
