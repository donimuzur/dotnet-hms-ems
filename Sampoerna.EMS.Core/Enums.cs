using System.ComponentModel;

namespace Sampoerna.EMS.Core
{
    public class Enums
    {
        public enum MenuList
        {
            MasterData = 2,
            ExcisableGoodsMovement = 3,
            ExcisableGoodsClaimable = 4,
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
            POA = 18,
            NPPBKC = 19
        }

        public enum PBCK1Type
        {
            [Description("New")]
            New,
            [Description("Additional")]
            Additional
        }

        public enum CK5Type
        {
            Domestic = 0,
            Intercompany = 1,
            PortToImporter = 2,
            ImporterToPlant = 3,
            Export = 4,
            Manual = 5,
            DomesticAlcohol = 6,
            Completed = 7
        }
    }
}
