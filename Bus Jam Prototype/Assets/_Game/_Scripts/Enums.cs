namespace _Game._Scripts
{
    public enum PrefabType
    {
        WaitingCell = 0,
        BusStopCell = 1,
        HumanView = 2,
        BusView = 3
    }
    
    public enum CommonColor
    {
        // human colors
        Red = 0,
        Green = 1,
        Blue = 2,
        Yellow = 3,
        Magenta = 4,
        Orange = 5,
        // common use colors
        Ground = 6,
        BusStop = 7,
        BoardArea = 8,
        Cell = 9,
        Road = 10,
        NotAvailableCell = 11
    }
    
    public enum HumanPosition
    {
        WaitingCell = 0,
        BusStopCell = 1,
        InBus = 2,
        Moving = 3
    }
    
    public enum HumanBusType
    {
        Red = 0,
        Green = 1,
        Blue = 2,
        Yellow = 3,
        Magenta = 4,
        Orange = 5,
    }
    
    public enum LoadedScene
    {
        NONE = 0,
        START = 1,
        GAME = 2,
        EDITOR = 3,
    }
}