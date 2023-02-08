using System;

namespace ProjectConstants
{
    #if !PROJECT_CONSTANT_CURRENCYTYPE
    public enum CurrencyType
    {
        Coins = 1,
        Gems = 2
    }
    #endif

    #if !PROJECT_CONSTANT_CURRENCYPLACEMENTTYPE
    public enum CurrencyPlacementType
    {
        Global = 1,
        Level = 2
    }
    
    [Flags]
    public enum CurrencyPlacementTypes
    {
        Global = 1,
        Level = 2
    }
    #endif
} 