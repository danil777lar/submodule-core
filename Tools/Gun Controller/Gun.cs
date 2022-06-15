using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Larje.Core.Tools.GunController
{
    public class Gun : MonoBehaviour
    {
        [SerializeField] private bool _stateOnStart;
        [SerializeField] private Transform _model;
        [Header("IKTargets")]
        [SerializeField] private Transform _rightArmTarget;
        [SerializeField] private Transform _leftArmTarget;
        [Header("Points")]
        [SerializeField] private Transform _enablePoint;
        [SerializeField] private Transform _disablePoint;
        [Header("Aim Options")]
        [SerializeField] private Transform _aimPoint;
        [SerializeField] private bool _twoDimensionAim;
        [Header("Shoot Options")]
        [SerializeField] private ParticleSystem _ammoSpawn;
        [SerializeField] private GameObject _ammoPrefab;
        [SerializeField] private int _ammoCountPerShoot;
        [SerializeField] private float _ammoScatter;
        [SerializeField] private float _recoilSize;

        private Quaternion _aimPointDefaultRotation;
        private Tween _stateChangeTween;

        [Space]
        public Transform aimTarget;
        public Transform RightArmTarget => _rightArmTarget;
        public Transform LeftArmTarget => _leftArmTarget;


        private void Start()
        {
            _aimPointDefaultRotation = _aimPoint.localRotation;
            ChangeState(_stateOnStart, 0f);
        }

        private void Update()
        {
            UpdateAim();
        }


        public void ChangeState(bool arg, float fade)
        {
            _model.SetParent(arg ? _enablePoint : _disablePoint);

            _stateChangeTween?.Kill();
            Vector3 startPosition = _model.localPosition;
            Quaternion startRotation = _model.localRotation;
            _stateChangeTween = DOTween.To(() => 0f, (v) =>
            {
                _model.localPosition = Vector3.Lerp(startPosition, Vector3.zero, v);
                _model.localRotation = Quaternion.Lerp(startRotation, Quaternion.Euler(Vector3.zero), v);
            }, 1f, fade);
        }

        public void Shoot() 
        {
            for (int i = 0; i < _ammoCountPerShoot; i++) 
            {
                GameObject ammoInstance = Instantiate(_ammoPrefab);
                ammoInstance.transform.position = _ammoSpawn.transform.position;
                ammoInstance.transform.rotation = _ammoSpawn.transform.rotation;
                ammoInstance.transform.localRotation *= Quaternion.Euler(Vector3.right * UnityEngine.Random.Range(-_ammoScatter / 2f, _ammoScatter / 2f));
            }
            _ammoSpawn.Play();

            _model.DOLocalMoveZ(-_recoilSize, 0.1f)
                .OnComplete(() => _model.DOLocalMoveZ(0f, 1f));
        }

        public void Drop() 
        {
            transform.SetParent(GameObject.FindGameObjectWithTag("Level").transform);
            GetComponent<Rigidbody>().isKinematic = false;
        }


        private void UpdateAim() 
        {
            Quaternion targetRotation = _aimPointDefaultRotation;
            if (aimTarget != null) 
            {
                Vector3 aimTargetPos = aimTarget.position;
                if (_twoDimensionAim)
                    aimTargetPos.z = _aimPoint.position.z;
                targetRotation = Quaternion.LookRotation(_aimPoint.parent.InverseTransformPoint(aimTargetPos) - _aimPoint.localPosition);
            }
            _aimPoint.localRotation = Quaternion.Lerp(_aimPoint.localRotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}