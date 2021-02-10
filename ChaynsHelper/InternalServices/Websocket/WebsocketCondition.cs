using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChaynsHelper.InternalServices.Websocket
{
    public class ConditionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Condition condition = (Condition) value;
            JObject o = new JObject();
            foreach (var entry in condition._GetEntries())
            {
                var conditionEntry = entry;
                var key = conditionEntry.Key;
                var val = conditionEntry.Value;
                JToken t = JToken.FromObject(val);
                if (t.Type != JTokenType.Object)
                {
                    o.Add(new JProperty(key, val));
                }
                else
                {
                    JObject v = JObject.FromObject(val);
                    o.Add(new JProperty(key, v));
                }
            }

            foreach (var entry in condition._GetComplexEntries())
            {
                var conditionEntry = entry;
                var key = conditionEntry.Key;
                JObject v = JObject.FromObject(new
                {
                    values = conditionEntry.Values,
                    type = conditionEntry.Type
                });
                o.Add(new JProperty(key, v));
            }

            o.WriteTo(writer);
        }

        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Condition);
        }
    }

    [JsonConverter(typeof(ConditionConverter))]
    public class Condition
    {
        private List<ConditionEntry> Entries { get; } = new List<ConditionEntry>();
        private List<ConditionEntryMultiple> ComplexEntries { get; } = new List<ConditionEntryMultiple>();

        public Condition()
        {
        }

        public Condition(string name, object value)
        {
            Entries.Add(new ConditionEntry(name, value));
        }

        public Condition(string name, ConditionType type, IEnumerable values)
        {
            ComplexEntries.Add(new ConditionEntryMultiple(name, type, values));
        }

        public Condition Add(string name, object value)
        {
            Entries.Add(new ConditionEntry(name, value));
            return this;
        }

        public Condition AddMultiple(string name, ConditionType type, IEnumerable values)
        {
            ComplexEntries.Add(new ConditionEntryMultiple(name, type, values));
            return this;
        }

        public List<ConditionEntry> _GetEntries()
        {
            return Entries;
        }

        public List<ConditionEntryMultiple> _GetComplexEntries()
        {
            return ComplexEntries;
        }
    }


    public class ConditionEntry
    {
        public ConditionEntry(string key, object value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }
        public object Value { get; set; }
    }

    public class ConditionEntryMultiple
    {
        public ConditionEntryMultiple(string key, ConditionType type, IEnumerable values)
        {
            Key = key;
            Values = values;
            switch (type)
            {
                case ConditionType.AllOf:
                    Type = "all_of";
                    break;
                case ConditionType.OneOf:
                    Type = "one_of";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        public string Key { get; set; }
        public string Type { get; set; }
        public IEnumerable Values { get; set; }
    }

    public class MultipleCondition
    {
        public MultipleCondition(ConditionType type, IEnumerable values)
        {
            Values = values;
            switch (type)
            {
                case ConditionType.AllOf:
                    Type = "all_of";
                    break;
                case ConditionType.OneOf:
                    Type = "one_of";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
        
        public string Type { get; set; }
        public IEnumerable Values { get; set; }
    }

    public enum ConditionType
    {
        OneOf,
        AllOf
    }
}