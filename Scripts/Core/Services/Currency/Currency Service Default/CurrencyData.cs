using System;
using System.Collections;
using System.Collections.Generic;
using ProjectConstants;
using UnityEngine;

namespace Larje.Core.Services
{
    [Serializable]
    public class CurrencyData
    {
        [SerializeField, HideInInspector] private string inspectorName;
        [SerializeField] private CurrencyType currencyType;
        [SerializeField] public List<PlacementData> placements;

        public CurrencyType CurrencyType => currencyType;
        
        public CurrencyData Build(CurrencyType currencyType)
        {
            this.currencyType = currencyType;
            inspectorName = Enum.GetName(typeof(CurrencyType), currencyType);
            placements = new List<PlacementData>();

            return this;
        }
    }

    [Serializable]
    public class PlacementData
    {
        [SerializeField, HideInInspector] private string inspectorName;
        [SerializeField] private CurrencyPlacementType currencyPlacementType;
        [SerializeField] public int count;
        
        public CurrencyPlacementType CurrencyPlacementType => currencyPlacementType;

        public PlacementData Build(CurrencyPlacementType currencyPlacementType)
        {
            this.currencyPlacementType = currencyPlacementType;
            inspectorName = Enum.GetName(typeof(CurrencyPlacementType), currencyPlacementType);
            
            return this;
        }
    }
    
    public partial class GameData
    {
        public List<CurrencyData> CurrencyData;
    }
}
