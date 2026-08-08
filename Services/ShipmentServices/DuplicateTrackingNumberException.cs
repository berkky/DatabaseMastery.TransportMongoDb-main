namespace DatabaseMastery.TransportMongoDb.Services.ShipmentServices
{
    public sealed class DuplicateTrackingNumberException : Exception
    {
        public DuplicateTrackingNumberException()
            : base("A shipment with the same tracking number already exists.")
        {
        }
    }
}
