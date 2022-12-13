using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Larje.Core.Services
{
    [Serializable]
    public class CurrencyData
    {
        public CurrencyType currencyType;
        public List<PlacementData> placements;
    }

    [Serializable]
    public class PlacementData
    {
        public CurrencyPlacementType currencyPlacementType;
        public int count;
    }
    
    public partial class GameData
    {
        public List<CurrencyData> CurrencyData;
    }
}
