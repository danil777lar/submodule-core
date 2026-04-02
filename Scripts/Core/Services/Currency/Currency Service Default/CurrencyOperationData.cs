using ProjectConstants;
using UnityEngine;

public struct CurrencyOperationData
{
    public CurrencyType Currency;
    public CurrencyPlacementType Placement;
    public int Amount;

    public bool UsePosition;
    public Vector3 WorldPosition;

    public CurrencyOperationData(CurrencyType currency, CurrencyPlacementType placement, int amount)
    {
        Currency = currency;
        Placement = placement;
        Amount = amount;

        UsePosition = false;
        WorldPosition = Vector3.zero;
    }
}
