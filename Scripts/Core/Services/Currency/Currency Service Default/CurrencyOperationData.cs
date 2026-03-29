using ProjectConstants;
using UnityEngine;

public struct CurrencyOperationData
{
    public CurrencyType Currency;
    public CurrencyPlacementType Placement;
    public int Amount;

    public bool UsePosition;
    public Vector3 WorldPosition;
}
