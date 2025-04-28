namespace ProjectHierarchyApi.Models
{
    public class CouponAssignRequest
    {
        public string CouponId { get; set; }
        public string AssignedBy { get; set; }
        public string AssignedTo { get; set; }
                public string AssignedEmployee { get; set; }


        public DateTime AssignedAt { get; set; }
        public bool RedeemStatus { get; set; }
        public string RedeemedCanteen { get; set; }
    }
}
