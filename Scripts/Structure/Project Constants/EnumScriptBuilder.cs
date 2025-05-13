using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnumScriptBuilder
{
    private const string PATH = "/Plugins/ProjectConstants/"; 
    
    private string _nameSpace;
    private string _fileName;
    private string _symbolPrefix;
    private IntValueType _intValueType = IntValueType.BiteShift;
    
    private List<string> _symbols = new List<string>(){""};
    private List<Constant> _constants = new List<Constant>();

    public EnumScriptBuilder(string nameSpace, string fileName, string symbolPrefix)
    {
        _nameSpace = nameSpace;
        _fileName = fileName;
        _symbolPrefix = symbolPrefix;
    }
    
    public EnumScriptBuilder SetIntValueType(IntValueType type)
    {
        _intValueType = type;
        return this;
    }
    
    public EnumScriptBuilder AddSymbol(string symbol)
    {
        if (_symbols.Count == 1 && _symbols[0] == "")
        {
            _symbols.Clear();
        }
        
        _symbols.Add(symbol);
        return this;
    }

    public EnumScriptBuilder AddConstant(string name, bool useFlags, string[] values)
    {
        Constant constant = new Constant
        {
            Name = name,
            UseFlags = useFlags,
            Values = values
        };
        
        _constants.Add(constant);
        return this;
    }

    public async void Save()
    {
        string fullPath = Application.dataPath + PATH + _fileName + ".cs";
        if (!Directory.Exists(Path.GetDirectoryName(fullPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        }

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        await File.WriteAllTextAsync(fullPath, GetScriptText());
        
        UpdateScriptingDefinedSymbols();
        
        #if UNITY_EDITOR
        AssetDatabase.Refresh();
        #endif
    }
    
    private void UpdateScriptingDefinedSymbols()
    {
        #if UNITY_EDITOR
        
        BuildTargetGroup[] targetGroups = new[]
        {
            BuildTargetGroup.Android, 
            BuildTargetGroup.iOS, 
            BuildTargetGroup.Standalone,
            BuildTargetGroup.WebGL
        };
        
        
        foreach (BuildTargetGroup buildGroup in targetGroups)
        {
            List<string> allSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildGroup).Split(';').ToList();
            allSymbols = allSymbols.Where(x => !x.Contains(_symbolPrefix)).ToList();
            foreach (string symbol in _symbols)
            {
                string fullSymbol = $"{_symbolPrefix}";
                fullSymbol += string.IsNullOrEmpty(symbol) ? "" : $"_{symbol.ToUpper().Replace(" ", "_")}";
                allSymbols.Add(fullSymbol);
            }

            string symbolsLine = "";
            allSymbols.ForEach(x => symbolsLine += $"{x};");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, symbolsLine);
        }
        
        #endif
    }
    
    private string GetScriptText()
    {
        string script = $"using System;\nnamespace {_nameSpace}\n{{\n";
        script += "//SCRIPT IS GENERATED AUTOMATICALLY\n";
        script += "//TO CHANGE THE CONTENT YOU SHOULD USE THE CONFIG\n";
        foreach (Constant constant in _constants)
        {
            int index = 0;
            string constantText = $"\tpublic enum {constant.Name}\n\t{{\n";
            string constantFlagsText = $"\t[Flags]\n\tpublic enum {constant.Name}s\n\t{{\n";
            foreach (string value in constant.Values)
            {
                constantText += $"\t\t{value} = {GetIntValue(value, index)},\n";
                constantFlagsText += $"\t\t{value} = {GetIntValue(value, index)},\n";
                index++;
            }

            constantText += "\t}\n\n";
            constantFlagsText += "\t}\n\n";
            script += constantText;
            if (constant.UseFlags)
            {
                script += constantFlagsText;
            }
        }

        script += "}\n";
        return script;
    }

    private string GetIntValue(string value, int index)
    {
        switch (_intValueType)
        {
            case IntValueType.Index:
                return index.ToString();
            case IntValueType.BiteShift:
                return GetIntValueByBiteShift(index);
            
            case IntValueType.MD5:
                return GetIntValueByMD5(value);
        }

        return "";
    }

    private string GetIntValueByBiteShift(int index)
    {
        return $"1 << {index}";
    }
    
    private string GetIntValueByMD5(string value)
    {
        MD5 md5Hasher = MD5.Create();
        byte[] hashed = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(value));
        return BitConverter.ToInt32(hashed, 0).ToString();
    }
    
    private class Constant
    {
        public string Name;
        public bool UseFlags;
        public string[] Values;
    }

    public enum IntValueType
    {
        Index,
        BiteShift,
        MD5
    }
}