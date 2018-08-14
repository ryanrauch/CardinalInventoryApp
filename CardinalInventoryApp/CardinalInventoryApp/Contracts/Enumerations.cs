namespace CardinalInventoryApp.Contracts
{
    public enum InventoryAction
    {
        UserViewedAuto = 0,
        UserViewedManual = 1,
        ReceivedManual = 2,
        ReceivedAuto = 3,
        RemovedDuringInventory = 4
    }

    public enum SmartWatchWristOrientation
    {
        LeftHanded = 0,
        RightHanded = 1
    }
}
