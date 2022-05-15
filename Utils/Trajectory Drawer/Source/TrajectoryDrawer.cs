using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryDrawer : MonoBehaviour
{
    [Header("Simulation Options")]
    [SerializeField] private float _updateDelta;
    [SerializeField] private float _drag;
    [SerializeField] private float _gravityScale;
    [SerializeField] private string[] _ignoreLayers;
    [Header("Visual")]
    [SerializeField] private float _startCutLenght;
    [SerializeField] private LineRenderer _linePrefab;
    [SerializeField] private ParticleSystem _partsPrefab;
    [SerializeField] private ParticleSystem _hitPrefab;

    private LineRenderer _lineInstance;
    private ParticleSystem _partsInstance;
    private ParticleSystem _hitInstance;


    public void ShowTrajectory(Vector3 force) 
    {
        TrySpawnLine();
        TrySpawnLineParts();
        TrySpawnHit();

        float cuttedLenght = 0f;
        Vector3 velocity = force;
        Vector3 position = transform.position;
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i < 100; i++)
        {
            points.Add(position);

            velocity = velocity * (1f - _updateDelta * _drag);
            velocity.y -= 9.8f * _gravityScale * _updateDelta;

            Vector3 prevPosition = position;
            position = position + velocity * _updateDelta;

            if (cuttedLenght < _startCutLenght) 
            {
                cuttedLenght += (velocity * _updateDelta).magnitude;
                points.RemoveAt(0);
            }

            RaycastHit2D hit = Physics2D.Raycast(prevPosition, position - prevPosition, velocity.magnitude * _updateDelta, ~LayerMask.GetMask(_ignoreLayers));
            if (hit)
            {
                points.Add(hit.point);
                break;
            }
        }

        TryUpdateLine(points);
        TryDestroyLineParts();
        TryUpdateHit();
    }

    public void HideTrajectory() 
    {
        TryDestroyLine();
        TryDestroyLineParts();
        TryDestroyHit();
    }


    #region Line

    private void TrySpawnLine() 
    {
        if (_lineInstance || !_linePrefab) return;

        _lineInstance = Instantiate(_linePrefab);
        _lineInstance.transform.SetParent(transform);
        _lineInstance.transform.localPosition = Vector3.zero;
    }

    private void TryUpdateLine(List<Vector3> points) 
    {
        if (!_lineInstance) return;

        _lineInstance.positionCount = points.Count;
        _lineInstance.SetPositions(points.ToArray());
    }

    private void TryDestroyLine() 
    {
        if (_lineInstance) 
        {
            Destroy(_lineInstance);
        }
    }

    #endregion

    #region LineParts

    private void TrySpawnLineParts()
    {

    }

    private void TryUpdateLineParts()
    {

    }

    private void TryDestroyLineParts()
    {

    }

    #endregion

    #region Hit

    private void TrySpawnHit()
    {

    }

    private void TryUpdateHit()
    {

    }

    private void TryDestroyHit()
    {

    }

    #endregion
}
