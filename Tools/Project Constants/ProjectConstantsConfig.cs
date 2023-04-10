#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Project Constants", menuName = "Project Constants")]
public class ProjectConstantsConfig : ScriptableObject
{
    private const string PATH = "/Plugins/ProjectConstants/ProjectConstants.cs";
    private const string NAMESPACE = "ProjectConstants";

    [SerializeField] private List<Constant> constants = new List<Constant>();

    [ContextMenu("Save")]
    public async void Save()
    {
        string fullPath = Application.dataPath + PATH;
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
        AssetDatabase.Refresh();
    }

    [ContextMenu("Load Default")]
    public void LoadDefaultValues()
    {
        constants.Clear();
        List<Type> allTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(x => x.IsEnum && x.Namespace == NAMESPACE)
            .ToList();
        List<Type> simpleEnums = allTypes.FindAll(x => Attribute.GetCustomAttribute(x, typeof(FlagsAttribute)) == null);
        List<Type> flagEnums = allTypes.FindAll(x => Attribute.GetCustomAttribute(x, typeof(FlagsAttribute)) != null);
        foreach (Type type in simpleEnums)
        {
            Constant constant = new Constant();
            constant.Name = type.Name;
            constant.Values = Enum.GetNames(type);
            constant.UseFlags = flagEnums.Select(x => x.Name).Where(s => s.Contains(constant.Name)).ToList().Count > 0;
            constants.Add(constant);
        }
    }

    private string GetScriptText()
    {
        string script = $"using System;\nnamespace {NAMESPACE}\n{{\n";
        script += "//SCRIPT IS GENERATED AUTOMATICALLY\n";
        script += "//TO CHANGE THE CONTENT YOU SHOULD USE THE CONFIG\n";
        foreach (Constant constant in constants)
        {
            int index = 0;
            string constantText = $"\tpublic enum {constant.Name}\n\t{{\n";
            string constantFlagsText = $"\t[Flags]\n\tpublic enum {constant.Name}s\n\t{{\n";
            foreach (string value in constant.Values)
            {
                constantText += $"\t\t{value} = {Mathf.Pow(2, index)},\n";
                constantFlagsText += $"\t\t{value} = {Mathf.Pow(2, index)},\n";
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

    private void UpdateScriptingDefinedSymbols()
    {
        BuildTargetGroup[] targetGroups = new[]
            { BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.Standalone };
        foreach (BuildTargetGroup buildGroup in targetGroups)
        {
            List<string> allSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildGroup).Split(';').ToList();
            allSymbols = allSymbols.Where(x => !x.Contains("PROJECT_CONSTANT")).ToList();
            foreach (Constant constant in constants)
            {
                allSymbols.Add($"PROJECT_CONSTANT_{constant.Name.ToUpper()}");
            }

            string symbolsLine = "";
            allSymbols.ForEach(x => symbolsLine += $"{x};");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(buildGroup, symbolsLine);
        }
    }

    [Serializable]
    public class Constant
    {
        public string Name;
        public bool UseFlags;
        public string[] Values;
    }
}
#endif