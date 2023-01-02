using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services
{
    [Serializable]
    public class CurrencyData
    {
        [SerializeField, HideInInspector] private string inspectorName;
        [HideInInspector] public readonly CurrencyType CurrencyType;
        public List<PlacementData> placements;

        public CurrencyData(CurrencyType currencyType)
        {
            CurrencyType = currencyType;
            inspectorName = Enum.GetName(typeof(CurrencyType), currencyType);
            placements = new List<PlacementData>();
        }
    }

    [Serializable]
    public class PlacementData
    {
        [SerializeField, HideInInspector] private string inspectorName;
        [HideInInspector] public readonly CurrencyPlacementType CurrencyPlacementType;
        public int count;

        public PlacementData(CurrencyPlacementType currencyPlacementType)
        {
            CurrencyPlacementType = currencyPlacementType;
            inspectorName = Enum.GetName(typeof(CurrencyPlacementType), currencyPlacementType);
        }
    }
    
    public partial class GameData
    {
        public List<CurrencyData> CurrencyData;
    }
}
