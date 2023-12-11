#region License
/*
Copyright 2022-2023 Dmitrii Evdokimov
Open source software

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
#endregion

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;

namespace Diev.Extensions.RuntimeConfig;

internal class ConfigBase
{
    private static string _profile = (AppContext.GetData(nameof(Profile)) as string ?? string.Empty) + '.'; //for faster access

    public string Profile
    {
        get => G(nameof(Profile));
        set
        {
            S(nameof(Profile), value);
            _profile = value + '.';
        }
    }

    public string[] Profiles
    {
        get => GArray(nameof(Profiles));
        set => S(nameof(Profiles), value);
    }

    public void Save(string appPath)
    {
        const string runtimeOptions = nameof(runtimeOptions);
        const string configProperties = nameof(configProperties);

        string path = Path.ChangeExtension(appPath, "runtimeconfig.json");
        string json = File.ReadAllText(path);

        var configNode = JsonNode.Parse(json);
        var entry = configNode![runtimeOptions]![configProperties];

        Type t = typeof(Config);
        var properties = t.GetProperties(/*BindingFlags.Public | BindingFlags.Instance*/);

        foreach (var p in properties)
        {
            switch (p.Name)
            {
                //case "ED807":
                //    var v = p.GetValue(p);
                //    var s = v as string;
                //    entry![p.Name] = s; // p.GetValue(p) as string;
                //    break;

                case nameof(Profile):
                    entry![nameof(Profile)] = Profile;
                    break;

                case nameof(Profiles): //TODO https://blog.okyrylchuk.dev/system-text-json-features-in-the-dotnet-6#heading-serialization-order-of-properties
                    var arr = new JsonArray();

                    foreach (var profile in Profiles)
                    {
                        arr.Add(profile); //TODO new JsonArray() { Profiles }
                    }

                    entry![nameof(Profiles)] = arr;
                    break;

                default: // BaseName or with Profile.Name
                    //var profiled = Attribute.IsDefined(t, typeof(ProfileAttribute));
                    //string name = profiled ? _profile + p.Name : p.Name;

                    string name = p.Name;

                    switch (p.PropertyType.Name)
                    {
                        case "String":
                            //entry![name] = p.GetValue(p) as string;
                            var v1 = p.GetValue(p);
                            entry![name] = v1 as string;
                            break;

                        case "Int32":
                            entry![name] = p.GetValue(p) as int?;
                            break;
                    }
                    break;
            }
        }

        var options = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
            //Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };
        json = configNode.ToJsonString(options); //TODO JsonSerializer.Serialize() ?

        File.WriteAllText(path, json);
    }

    protected static string G(string name, string defValue = "")
        => AppContext.GetData(name) as string ?? defValue;

    protected static string[] GArray(string name)
    {
        var value = AppContext.GetData(name);

        if (value is null)
        {
            return Array.Empty<string>();
        }

        if (value is String)
        {
            var values = JsonSerializer.Deserialize<string[]>((string)value);
            S(name, values);

            if (values is null)
            {
                return Array.Empty<string>();
            }

            return values;
        }

        return (string[])value;
    }

    protected static int GInt(string name, int defValue = 0)
    {
        var o = AppContext.GetData(name);

        if (o is null) return defValue;
        if (o is int i) return i;

        return int.Parse((string)o);
    }

    protected static string GP(string name, string defValue = "")
        => G(_profile + name, defValue);

    protected static int GPInt(string name, int defValue = 0)
        => GInt(_profile + name, defValue);


#if NET7_0_OR_GREATER

    protected static void S(string name, string value = "")
        => AppContext.SetData(name, value);

    protected static void S(string name, int value)
        => AppContext.SetData(name, value);

    protected static void S(string name, string[]? value)
        => AppContext.SetData(name, value);

#else

    // AppContext.SetData(string name, object? data); // available from .NET 7+
    // See a lifehack at
    // https://www.strathweb.com/2019/12/runtime-host-configuration-options-and-appcontext-data-in-net-core/

    protected static void S(string name, string value = "")
        => AppDomain.CurrentDomain.SetData(name, value);

    protected static void S(string name, int value)
        => AppDomain.CurrentDomain.SetData(name, value);

    protected static void S(string name, string[]? value)
        => AppDomain.CurrentDomain.SetData(name, value);

#endif

    protected static void SP(string name, string value = "")
        => S(_profile + name, value);

    protected static void SP(string name, int value)
        => S(_profile + name, value);
}
