using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Larje.Core.Services.DebugConsole
{
    public class DebugConsoleMethodGroupPanel : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Transform contentRoot;
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private List<Color> colors;

        private bool _opened;
        private int _index;
        private string _groupName;
        private Action _onClicked;
        private Action _onUpdate;
        
        public void Init(int index, string groupName, Action onClicked, Action onUpdate)
        {
            _index = index;
            _groupName = groupName;
            _onClicked = onClicked;
            _onUpdate = onUpdate;
            
            title.text = groupName;
            title.color = colors[_index % colors.Count];
            
            button.onClick.AddListener(OnClicked);
            
            contentRoot.DestroyAllChildren();
            contentRoot.localScale = new Vector3(1f, 0f, 1f);
        }

        public void AddChild(Transform child)
        {
            child.SetParent(contentRoot);
            child.transform.localScale = Vector3.one;
        }

        public void SetState(bool opened)
        {
            _opened = opened;
            this.DOKill();
            contentRoot.DOScaleY(opened ? 1f : 0f, 0.3f)
                .SetTarget(this)
                .OnUpdate(() => _onUpdate?.Invoke())
                .SetEase(Ease.OutCubic);
        }

        private void OnClicked()
        {
            if (!_opened)
            {
                _onClicked?.Invoke();
            }
        }
    }
}