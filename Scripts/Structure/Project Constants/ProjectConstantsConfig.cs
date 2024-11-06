using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Project Constants", menuName = "Larje/Core/Tools/Project Constants")]
public class ProjectConstantsConfig : ScriptableObject
{
    private const string NAMESPACE = "ProjectConstants";
    private const string FILE_NAME = "ProjectConstants";
    private const string SYMBOL_PREFIX = "PROJECT_CONSTANT";

    [SerializeField] private List<Constant> constants = new List<Constant>();

    [ContextMenu("Save")]
    public void Save()
    {
        EnumScriptBuilder builder = new EnumScriptBuilder(NAMESPACE, FILE_NAME, SYMBOL_PREFIX);
        foreach (Constant constant in constants)
        {
            builder.AddConstant(constant.Name, constant.UseFlags, constant.Values);
            builder.AddSymbol(constant.Name);
        }
        builder.Save();
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

    [Serializable]
    public class Constant
    {
        public string Name;
        public bool UseFlags;
        public string[] Values;
    }
}