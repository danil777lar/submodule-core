using UnityEngine;
using UnityEngine.UI;

namespace Larje.Core.Tools.UIWidgets
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class WidgetPanel : MaskableGraphic
    {
        private const string SHADER_NAME = "LarjeUI/Panel";

        private static readonly int ID_RadiusPx = Shader.PropertyToID("_RadiusPx");
        private static readonly int ID_BorderPx = Shader.PropertyToID("_BorderPx");
        private static readonly int ID_SoftnessPx= Shader.PropertyToID("_SoftnessPx");
        private static readonly int ID_RectSize = Shader.PropertyToID("_RectSize");
        private static readonly int ID_FillColor = Shader.PropertyToID("_FillColor");
        private static readonly int ID_BorderColor = Shader.PropertyToID("_BorderColor");

        [Header("Shape")]
        [SerializeField, Min(0f)] private float radiusPx = 16f;
        [SerializeField, Min(0f)] private float borderPx = 0f;
        [Space]
        [SerializeField, Min(0f)] private float softnessPx = 0f;

        [Header("Colors")]
        [SerializeField] private Color fillColor = Color.white;
        [SerializeField] private Color borderColor = Color.black;

        private bool _returnRuntimeMat;
        private Material _runtimeMat;

        // public override Material materialForRendering
        // {
        //     get
        //     {  
        //         EnsureMaterial();
        //         Material mat = base.materialForRendering;
        //         _returnRuntimeMat = !_returnRuntimeMat;
        //         mat = GetModifiedMaterial(mat);

        //         return mat;
        //     }
        // }

        protected override void Awake()
        {
            base.Awake();
            EnsureMaterial();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            EnsureMaterial();
            SetAllDirty();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            DestroyRuntimeMaterial();
        }

        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();

            radiusPx = Mathf.Max(0f, radiusPx);
            borderPx = Mathf.Max(0f, borderPx);

            EnsureMaterial();
            PushShaderParams();
            SetAllDirty();
        }
        #endif

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Rect r = GetPixelAdjustedRect();
            float w = Mathf.Max(1f, r.width);
            float h = Mathf.Max(1f, r.height);

            Vector4 fc = fillColor;
            Vector4 bc = borderColor;
            Vector4 rbs = new Vector4(radiusPx, borderPx, softnessPx, 0f);
            Vector4 size = new Vector4(w, h, 1f / w, 1f / h);

            UIVertex v = UIVertex.simpleVert;
            v.color = color;
            v.uv1 = fc;
            v.uv2 = bc;
            v.uv3 = rbs;
            v.tangent = size;

            v.position = new Vector3(r.xMin, r.yMin);
            v.uv0 = new Vector4(0f, 0f, 0f, 0f);
            vh.AddVert(v);

            v.position = new Vector3(r.xMin, r.yMax);
            v.uv0 = new Vector4(0f, 1f, 0f, 0f);
            vh.AddVert(v);

            v.position = new Vector3(r.xMax, r.yMax);
            v.uv0 = new Vector4(1f, 1f, 0f, 0f);
            vh.AddVert(v);

            v.position = new Vector3(r.xMax, r.yMin);
            v.uv0 = new Vector4(1f, 0f, 0f, 0f);
            vh.AddVert(v);

            vh.AddTriangle(0, 1, 2);
            vh.AddTriangle(2, 3, 0);

            PushShaderParams();
        }

        private void EnsureMaterial()
        {
            if (_runtimeMat != null) return;

            Shader shader = Shader.Find(SHADER_NAME);
            if (shader == null)
            {
                shader = Shader.Find("UI/Default");
            }

            _runtimeMat = new Material(shader)
            {
                name = $"(Runtime) UI Panel - {gameObject.name}",
                hideFlags = HideFlags.HideAndDontSave
            };

            material = _runtimeMat;
        }

        private void DestroyRuntimeMaterial()
        {
            if (_runtimeMat == null) return;

    #if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(_runtimeMat);
            else
                Destroy(_runtimeMat);
    #else
            Destroy(_runtimeMat);
    #endif

            _runtimeMat = null;
        }

        private void PushShaderParams()
        {
            if (_runtimeMat == null) return;

            Rect r = GetPixelAdjustedRect();
            float w = Mathf.Max(1f, r.width);
            float h = Mathf.Max(1f, r.height);

            float maxR = 0.5f * Mathf.Min(w, h);
            float clampedRadius = Mathf.Clamp(radiusPx, 0f, maxR);

            float maxB = Mathf.Min(w, h) * 0.5f;
            float clampedBorder = Mathf.Clamp(borderPx, 0f, maxB);

            // _runtimeMat.SetColor(ID_FillColor, fillColor);
            // _runtimeMat.SetColor(ID_BorderColor, borderColor);
            // _runtimeMat.SetFloat(ID_RadiusPx, clampedRadius);
            // _runtimeMat.SetFloat(ID_BorderPx, clampedBorder);
            // _runtimeMat.SetFloat(ID_SoftnessPx, softnessPx);
            // _runtimeMat.SetVector(ID_RectSize, new Vector4(w, h, 1f / w, 1f / h));
        }

        public void SetRadius(float px)
        {
            radiusPx = Mathf.Max(0f, px);
            SetMaterialDirty();
            SetVerticesDirty();
        }

        public void SetBorder(float px)
        {
            borderPx = Mathf.Max(0f, px);
            SetMaterialDirty();
            SetVerticesDirty();
        }
    }
}
