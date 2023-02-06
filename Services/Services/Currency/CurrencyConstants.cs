namespace ProjectConstants
{
    #if !PROJECT_CONSTANT_CURRENCYTYPE
    public enum CurrencyType
    {
        Coins,
        Gems
    }
    #endif

    #if !PROJECT_CONSTANT_CURRENCYPLACEMENTTYPE
    public enum CurrencyPlacementType
    {
        Global,
        Level
    }
    #endif
} 