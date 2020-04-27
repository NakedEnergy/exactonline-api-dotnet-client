using Newtonsoft.Json;
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
                if (entity != null && entity.GetType() == typeof(Dictionary<string, object>))
                {
                    var value = (Dictionary<string, object>)entity;
                    Deserialize(value);
                }
                else
                {
                    var value = entity;
                    if (value != null)
                    {
                        // Set EPOCH datetime
                        Type valueType = value.GetType();
                        if (valueType == typeof(DateTime))
                        {
                            var date = (DateTime)entity;
                            TimeSpan t = (date - new DateTime(1970, 1, 1));
                            double timestamp = t.TotalMilliseconds;
                            dictionary[key] = string.Format("/Date({0})/", timestamp);
                        }

                        // For collection within this collection > send to this method recursively
                        if (valueType == typeof(ArrayList))
                        {
                            IEnumerable<object> dictionaries = ((ArrayList)value).ToArray().Where(x => x.GetType() == typeof(Dictionary<string, object>));

                            foreach (var dict in dictionaries)
                            {
                                Deserialize((Dictionary<string, object>)dict);
                            }
                        }
                    }
                }
            }
            return dictionary;
        }

        //Deserialize
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var dictionary = serializer.Deserialize<Dictionary<string, object>>(reader);
            dictionary = this.Deserialize(dictionary);
            return dictionary;
        }



        //Serialize
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

        }
    }
}
