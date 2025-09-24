using System;
using System.Collections.Generic;
using ProjectConstants;
using UnityEditor;
using UnityEngine;

namespace Larje.Core.Tools.PrefabPacker
{
    public class PrefabPacker : MonoBehaviour
    {
        [SerializeField] private PrefabPackerConfig config;
        [SerializeField] private List<PackedObject> packedObjects;

        public event Action EventLevelUnpacked;

        private void Start()
        {
            UnpackLevel();
            EventLevelUnpacked?.Invoke();
        }

        [ContextMenu("Pack Level")]
        private void PackLevel()
        {
            if (packedObjects == null || packedObjects.Count == 0)
            {
                packedObjects = new List<PackedObject>();
                foreach (PackablePrefab levelObject in GetComponentsInChildren<PackablePrefab>(true))
                {
                    PackedObject packedObject = new PackedObject();
                    packedObject.name = levelObject.gameObject.name;
                    packedObject.type = levelObject.ObjectType;
                    packedObject.data = levelObject.PackObject();
                    packedObjects.Add(packedObject);
                }
            }
        }

        [ContextMenu("Unpack Level")]
        private void UnpackLevel()
        {
            if (packedObjects != null && packedObjects.Count != 0)
            {
                foreach (PackedObject packedObject in packedObjects)
                {
                    PackablePrefab prefab = config.GetPrefabByType(packedObject.type);
#if UNITY_EDITOR
                    PackablePrefab instance = null;
                    if (Application.isPlaying)
                    {
                        instance = Instantiate(prefab);
                    }
                    else
                    {
                        instance = PrefabUtility.InstantiatePrefab(prefab) as PackablePrefab;
                    }
#else
                    PackablePrefab instance = Instantiate(prefab);
#endif
                    instance.UnpackObject(packedObject.data);
                }

                packedObjects = null;
            }
        }

        [Serializable]
        private class PackedObject
        {
            [HideInInspector] public string name;
            public PackablePrefabType type;
            public PackablePrefab.PackableLevelObjectData data;
        }
    }
}