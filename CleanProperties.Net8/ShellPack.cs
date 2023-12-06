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

using System.Text;

using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;

using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace CleanProperties.Net8;

public static class ShellPack
{
    public static void DisplayPropertyValue(string fileName, string propertyName)
    {
        IShellProperty p = ShellObject.FromParsingName(fileName).Properties.GetProperty(propertyName);
        DisplayPropertyValue(p);
    }

    public static void DisplayPropertyValue(IShellProperty p)
    {
        Console.WriteLine($"{p.CanonicalName} = {GetPropertyValue(p).value}");
    }

    public static (bool exist, string value) GetPropertyValue(IShellProperty p)
    {
        bool exist = p.ValueAsObject != null;
        string value = exist
            ? p.FormatForDisplay(PropertyDescriptionFormatOptions.None)
            : string.Empty;

        return (exist, value);
    }

    public static void EnumProperties(string fileName)
    {
        ShellPropertyCollection collection = new(fileName);

        var properties = collection
            .Where(p => p.CanonicalName != null)
            .ToArray();

        Array.ForEach(properties, DisplayPropertyValue);
    }

    public static void EnumProperties(string fileName, string filter)
    {
        ShellPropertyCollection collection = new(fileName);

        var properties = collection
            .Where(p => p.CanonicalName != null &&
                p.CanonicalName.StartsWith(filter, StringComparison.CurrentCultureIgnoreCase))
            .ToArray();

        Array.ForEach(properties, DisplayPropertyValue);
    }

    public static string? GetPersonalProperties(string fileName, bool hideCommonNames = false)
    {
        StringBuilder sb = new();
        //sb.Append('"').Append(fileName).Append('"');
        int length = 0;

        try
        {
            ShellProperties p = ShellObject.FromParsingName(fileName).Properties;

            var x = GetPropertyValue(p.GetProperty(Author));
            if (x.exist)
            {
                if (hideCommonNames &&
                    (x.value.Equals("User", StringComparison.OrdinalIgnoreCase) ||
                    x.value.Equals("UserName", StringComparison.OrdinalIgnoreCase) ||
                    x.value.Equals("Пользователь", StringComparison.CurrentCultureIgnoreCase)))
                {
                    // skip Common Name
                }
                else
                {
                    sb.Append(" [Author: ").Append(x.value).Append(']');
                }

                length++;
            }

            x = GetPropertyValue(p.GetProperty(Document.LastAuthor));
            if (x.exist)
            {
                if (hideCommonNames &&
                    (x.value.Equals("User", StringComparison.OrdinalIgnoreCase) ||
                    x.value.Equals("UserName", StringComparison.OrdinalIgnoreCase) ||
                    x.value.Equals("Пользователь", StringComparison.CurrentCultureIgnoreCase)))
                {
                    // skip Common Name
                }
                else
                {
                    sb.Append(" [LastAuthor: ").Append(x.value).Append(']');
                }

                length++;
            }

            x = GetPropertyValue(p.GetProperty(Document.Manager));
            if (x.exist)
            {
                sb.Append(" [Manager: ").Append(x.value).Append(']');
                length++;
            }

            x = GetPropertyValue(p.GetProperty(Company));
            if (x.exist)
            {
                if (hideCommonNames &&
                    x.value.Equals("Company", StringComparison.OrdinalIgnoreCase))
                {
                    // skip Common Name
                }
                else
                {
                    sb.Append(" [Company: ").Append(x.value).Append(']');
                }

                length++;
            }

            x = GetPropertyValue(p.GetProperty(Title));
            if (x.exist)
            {
                sb.Append(" [Title: ").Append(x.value).Append(']');
                length++;
            }

            x = GetPropertyValue(p.GetProperty(Category));
            if (x.exist)
            {
                sb.Append(" [Category: ").Append(x.value).Append(']');
                length++;
            }

            x = GetPropertyValue(p.GetProperty(Keywords));
            if (x.exist)
            {
                sb.Append(" [Keywords: ").Append(x.value).Append(']');
                length++;
            }

            x = GetPropertyValue(p.GetProperty(ContentStatus));
            if (x.exist)
            {
                sb.Append(" [ContentStatus: ").Append(x.value).Append(']');
                length++;
            }

            x = GetPropertyValue(p.GetProperty(Subject));
            if (x.exist)
            {
                sb.Append(" [Subject: ").Append(x.value).Append(']');
                length++;
            }

            x = GetPropertyValue(p.GetProperty(Comment));
            if (x.exist)
            {
                sb.Append(" [Comment: ").Append(x.value).Append(']');
                length++;
            }

            return length == 0 ? null : sb.ToString().Trim();
        }
        catch
        {
            return sb.Append(" Error!").ToString();
        }
    }

    public static string? CleanPersonalProperties(FileInfo file, bool ignoreReadOnly = false)
    {
        var lastWritten = file.LastWriteTime;
        var readOnly = file.IsReadOnly;

        if (readOnly)
        {
            if (ignoreReadOnly)
            {
                file.IsReadOnly = false;
            }
            else
            {
                return " ReadOnly!";
            }
        }

        try
        {
            using var writer = ShellObject.FromParsingName(file.FullName).Properties.GetPropertyWriter();

            writer.WriteProperty(Author, null, true);
            writer.WriteProperty(Category, null, true);
            writer.WriteProperty(Comment, null, true);
            writer.WriteProperty(Company, null, true);
            writer.WriteProperty(ContentStatus, null, true);
            writer.WriteProperty(Keywords, null, true);
            writer.WriteProperty(Subject, null, true);
            writer.WriteProperty(Title, null, true);
            writer.WriteProperty(Document.LastAuthor, null, true);
            writer.WriteProperty(Document.Manager, null, true);

            file.LastAccessTime = lastWritten;

            if (readOnly)
            {
                file.IsReadOnly = true;
            }

            return null;
        }
        catch // FileLoadException
        {
            return " InUse!";
        }
    }

    public static void ShowPropertyInfo(string propertyName)
    {
        ShellPropertyDescription p = SystemProperties.GetPropertyDescription(propertyName);
        string info = $@"
Property:              {p.CanonicalName}
PropertyKey:           {p.PropertyKey.FormatId:B}, {p.PropertyKey.PropertyId}
Label:                 {p.DisplayName}
Edit Invitation:       {p.EditInvitation}
Display Type:          {p.DisplayType}
Var Enum Type:         {p.VarEnumType}
Value Type:            {p.ValueType}
Default Column Width:  {p.DefaultColumWidth}
Aggregation Type:      {p.AggregationTypes}
Has Multiple Values:   {p.TypeFlags.HasFlag(PropertyTypeOptions.MultipleValues)}
Is Group:              {p.TypeFlags.HasFlag(PropertyTypeOptions.IsGroup)}
Is Innate:             {p.TypeFlags.HasFlag(PropertyTypeOptions.IsInnate)}
Is Queryable:          {p.TypeFlags.HasFlag(PropertyTypeOptions.IsQueryable)}
Is Viewable:           {p.TypeFlags.HasFlag(PropertyTypeOptions.IsViewable)}
Is SystemProperty:     {p.TypeFlags.HasFlag(PropertyTypeOptions.IsSystemProperty)}";

        Console.WriteLine(info);
    }

    public static void SetPropertyValue(string fileName, string propertyName, string value)
    {
        IShellProperty p = ShellObject.FromParsingName(fileName).Properties.GetProperty(propertyName);

        if (p.ValueType == typeof(string))
        {
            (p as ShellProperty<string>)!.Value = value;
        }
        else if (p.ValueType == typeof(string[]))
        {
            (p as ShellProperty<string[]>)!.Value = value.Split(';', StringSplitOptions.RemoveEmptyEntries);
        }
        else if (p.ValueType == typeof(DateTime?))
        {
            (p as ShellProperty<DateTime?>)!.Value = DateTime.Parse(value);
        }
        else if (p.ValueType == typeof(ushort?))
        {
            (p as ShellProperty<ushort?>)!.Value = ushort.Parse(value);
        }
        else if (p.ValueType == typeof(short?))
        {
            (p as ShellProperty<short?>)!.Value = short.Parse(value);
        }
        else if (p.ValueType == typeof(uint?))
        {
            (p as ShellProperty<uint?>)!.Value = uint.Parse(value);
        }
        else if (p.ValueType == typeof(int?))
        {
            (p as ShellProperty<int?>)!.Value = int.Parse(value);
        }
        else if (p.ValueType == typeof(ulong?))
        {
            (p as ShellProperty<ulong?>)!.Value = ulong.Parse(value);
        }
        else if (p.ValueType == typeof(long?))
        {
            (p as ShellProperty<long?>)!.Value = long.Parse(value);
        }
        else if (p.ValueType == typeof(double?))
        {
            (p as ShellProperty<double?>)!.Value = double.Parse(value);
        }
    }
}
