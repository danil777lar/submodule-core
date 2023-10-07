#if UNITY_2022_3_OR_NEWER
using UnityEngine;

namespace Larje.Core.Tools.RoomGenerator
{
    [CreateAssetMenu(fileName = "Wall Config", menuName = "Configs/Wall Config")]
    public class RoomMeshConfig : ScriptableObject
    {
        [SerializeField, Min(0f)] private float wallWidth;
        [SerializeField, Min(0f)] private float wallHeight;
        [SerializeField, Range(0f, 1f)] private float wallDividePercent;
        [SerializeField] private float doorWidth;
        [SerializeField] private float roomBottomHeight;
        [Header("Materials")] [SerializeField] private Material floorMaterial;
        [SerializeField] private Material wallBottomMaterial;
        [SerializeField] private Material wallTopMaterial;

        public float WallWidth => wallWidth;
        public float WallHeight => wallHeight;
        public float WallDividePercent => wallDividePercent;
        public float DoorWidth => doorWidth;
        public float RoomBottomHeight => roomBottomHeight;
        public Material FloorMaterial => floorMaterial;
        public Material WallBottomMaterial => wallBottomMaterial;
        public Material WallTopMaterial => wallTopMaterial;

        public static RoomMeshConfig Instance
        {
            get
            {
                RoomMeshConfig config = Resources.Load<RoomMeshConfig>("WallConfig");
                if (config != null)
                {
                    return config;
                }
                else
                {
                    Debug.LogError("Wall Config Not Found");
                    return null;
                }
            }
        }
    }
}
#endif