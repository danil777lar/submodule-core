using System;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Tools.PrefabPacker
{
    public class PackablePrefab : MonoBehaviour
    {
        [SerializeField] private PackablePrefabType objectType;

        public PackablePrefabType ObjectType => objectType;

        public PackableLevelObjectData PackObject()
        {
            PackableLevelObjectData data = new PackableLevelObjectData()
            {
                activeSelf = gameObject.activeSelf,
                parent = transform.parent,
                position = transform.position,
                scale = transform.localScale,
                rotation = transform.rotation
            };
            DestroyImmediate(gameObject);
            return data;
        }

        public void UnpackObject(PackableLevelObjectData data)
        {
            gameObject.SetActive(data.activeSelf);
            transform.SetParent(data.parent);
            transform.position = data.position;
            transform.localScale = data.scale;
            transform.rotation = data.rotation;
        }

        [Serializable]
        public class PackableLevelObjectData
        {
            public bool activeSelf;
            public Transform parent;
            public Vector3 position;
            public Vector3 scale;
            public Quaternion rotation;
        }
    }
}