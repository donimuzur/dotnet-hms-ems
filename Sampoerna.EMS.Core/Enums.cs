using System.ComponentModel;

namespace Sampoerna.EMS.Core
{
    public class Enums
    {
        public enum MenuList
        {
            MasterData = 2,
            ExcisableGoodMovement = 3,
            ExcisableGoodClaimable = 4,
            Settings = 5,
            PBCK1 = 6,
            CK5 = 7,
            LACK1 = 8,
            LACK2 = 9,
            CK4C = 10,
            PBCK4 = 11,
            PBCK7 = 12,
            PBCK3 = 13,
            CK5MRETURN = 14,
            USER = 15,
            LOGIN = 16,
            COMPANY = 17,
            VitualMappingPlant = 18
        }

        public enum PBCK1Type
        {
            [Description("New")]
            New,
            [Description("Additional")]
            Additional
        }
    }
}
