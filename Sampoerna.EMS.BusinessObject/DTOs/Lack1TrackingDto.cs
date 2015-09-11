namespace Sampoerna.EMS.BusinessObject.DTOs
{
    public class Lack1TrackingDto
    {
        public long LACK1_TRACKING_ID { get; set; }
        public int LACK1_ID { get; set; }
        public long INVENTORY_MOVEMENT_ID { get; set; }
        public System.DateTime CREATED_DATE { get; set; }

        public InventoryMovementDto INVENTORY_MOVEMENT { get; set; }

    }
}
