using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Tools.PrefabPacker
{
    [CreateAssetMenu(fileName = "Level Prefabs Config", menuName = "Configs/Level Prefabs Config")]
    public class PrefabPackerConfig : ScriptableObject
    {
        [SerializeField] private List<PackablePrefab> levelObjectPrefabs;

        public PackablePrefab GetPrefabByType(PackablePrefabType levelObjectType)
        {
            return levelObjectPrefabs.Find(x => x.ObjectType == levelObjectType);
        }
    }
}