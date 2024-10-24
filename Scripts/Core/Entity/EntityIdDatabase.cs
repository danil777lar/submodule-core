using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Larje.Core.Services;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Larje.Core.Entities
{
    [CreateAssetMenu(fileName = "Entity Id Database", menuName = "Larje/Core/Entity/Entity Id Database")]
    public class EntityIdDatabase : ScriptableObject
    {
        private const string NAMESPACE = "Larje.Core.Entities";
        private const string FILE_NAME = "EntityId";
        private const string SYMBOL_PREFIX = "ENTITY_DATABASE_INITIALIZED";

        [SerializeField] private List<EntityGroup> _entityGroups;

        [ContextMenu("Save")]
        private void Save()
        {
            List<string> allIds = new List<string>() { "None" };
            allIds.AddRange(_entityGroups.SelectMany(x => x.Ids)); 
            
            new EnumScriptBuilder(NAMESPACE, FILE_NAME, SYMBOL_PREFIX)
                .SetIntValueType(EnumScriptBuilder.IntValueType.MD5)
                .AddConstant(FILE_NAME, false, allIds.ToArray())
                .Save();
        }

        [Serializable]
        private class EntityGroup
        {
            public string Name;
            public List<string> Ids;
        }
    }
}