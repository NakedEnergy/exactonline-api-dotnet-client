using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ExactOnline.Client.Sdk.Helpers
{
    /// <summary>
    /// Custom JavaScriptConverter for parsing datetime value correctly
    /// </summary>
    public class JssDateTimeConverter : JsonConverter
    {
        public IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new[] { typeof(Object) };
            }
        }

        public override bool CanConvert(Type objectType) => SupportedTypes.Contains(objectType);


        public Dictionary<string, object> Deserialize(Dictionary<string, object> dictionary)
        {
            var keys = new List<string>(dictionary.Keys);

            foreach (string key in keys)
            {
                object entity = dictionary[key];

                // Check if content is a dictionary > send to this method recursively
                if (entity != null && entity is JObject jObject)
                {
                    var dict = jObject.ToObject<Dictionary<string, object>>();
                    dictionary[key] = Deserialize(dict);
                }
                // For collection within this collection > send to this method recursively
                else if (entity != null && entity is JArray jArray)
                {
                    IEnumerable<JToken> dictionaries = jArray.Where(x => x.Type == JTokenType.Object);
                    var list = new List<object>();
                    foreach (var dict in dictionaries)
                    {
                        list.Add(Deserialize(dict.ToObject<Dictionary<string, object>>()));
                    }
                    dictionary[key] = new ArrayList(list);
                }
                else if (entity != null && entity is JToken jToken)
                {
                    // Set EPOCH datetime
                    if (jToken.Type == JTokenType.Date)
                    {
                        var date = (DateTime)entity;
                        TimeSpan t = (date - new DateTime(1970, 1, 1));
                        double timestamp = t.TotalMilliseconds;
                        dictionary[key] = string.Format("/Date({0})/", timestamp);
                    }
                    else
                    {
                        dictionary[key] = jToken.ToObject<Object>();
                    }
                }

            }
            return dictionary;
        }

        //Deserialize
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var dictionary = jObject.ToObject<Dictionary<string, object>>();
            dictionary = this.Deserialize(dictionary);
            return dictionary;
        }



        //Serialize
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

        }
    }
}
