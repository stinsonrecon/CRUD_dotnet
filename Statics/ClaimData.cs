using System.Collections.Generic;

namespace CRUD.Statics
{
    public static class ClaimData
    {
        public static List<string> UserClaims { get; set; } = new List<string>
        {
            "Add Customer",
            "Edit Customer",
            "Delete Customer"
        };
    }
}
