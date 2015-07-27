using System;
using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;

namespace Sampoerna.EMS.BLL.Test
{
    public class FakeStuffs
    {

        public static IEnumerable<USER> GetGenericUserStubs()
        {
            var users = new List<USER>();
            users.Add(new USER() { USER_ID = "1", FIRST_NAME = "Jeanette", LAST_NAME = "Kist", IS_ACTIVE = false, USER_GROUP_ID = "4" });
            users.Add(new USER() { USER_ID = "2", FIRST_NAME = "Jackqueline", LAST_NAME = "Samms", IS_ACTIVE = false, USER_GROUP_ID = "4" });
            users.Add(new USER() { USER_ID = "3", FIRST_NAME = "Lovella", LAST_NAME = "Chouinard", IS_ACTIVE = false, USER_GROUP_ID = "4"});
            users.Add(new USER() { USER_ID = "4", FIRST_NAME = "Valentine", LAST_NAME = "Alm", IS_ACTIVE = false, USER_GROUP_ID = "4" });
            users.Add(new USER() { USER_ID = "5", FIRST_NAME = "Aliza", LAST_NAME = "Hennen", IS_ACTIVE = false, USER_GROUP_ID = "4" });
            users.Add(new USER() { USER_ID = "6", FIRST_NAME = "Ilona", LAST_NAME = "Escobar", IS_ACTIVE = false, USER_GROUP_ID = "4" });
            users.Add(new USER() { USER_ID = "7", FIRST_NAME = "Shasta", LAST_NAME = "Ruis", IS_ACTIVE = false, USER_GROUP_ID = "4" });
            users.Add(new USER() { USER_ID = "8", FIRST_NAME = "Celsa", LAST_NAME = "Nickel", IS_ACTIVE = false, USER_GROUP_ID = "4" });
            users.Add(new USER() { USER_ID = "10", FIRST_NAME = "Gregg", LAST_NAME = "Fahey", IS_ACTIVE = false, USER_GROUP_ID = "4" });

            return users;
        }

       
       
        public static IEnumerable<USER_GROUP> GetUserGroupStubs()
        {
            var rc = new List<USER_GROUP>();
            rc.Add(new USER_GROUP() { USER_GROUP_ID = "1", GROUP_DESCRIPTION = "Administrator" });
            rc.Add(new USER_GROUP() { USER_GROUP_ID = "2", GROUP_DESCRIPTION = "Supervisor" });
            rc.Add(new USER_GROUP() { USER_GROUP_ID = "3", GROUP_DESCRIPTION = "Plan" });
            rc.Add(new USER_GROUP() { USER_GROUP_ID = "4", GROUP_DESCRIPTION = "Operator" });
            return rc;
        }

        public static IEnumerable<T001> GetCompany()
        {

            var compannyDummy = new List<T001>();
            compannyDummy.Add(new T001() {  BUKRS = "101", BUTXT = "ABSC", CREATED_DATE = Convert.ToDateTime("2015-05-29 10:55:11.317") });
            compannyDummy.Add(new T001() {  BUKRS = "102", BUTXT = "ABSC", CREATED_DATE = Convert.ToDateTime("2015-05-29 10:55:12.980") });
            compannyDummy.Add(new T001() {  BUKRS = "103", BUTXT = "ABSC", CREATED_DATE = Convert.ToDateTime("2015-05-29 10:55:56.660") });
            compannyDummy.Add(new T001() {  BUKRS = "104", BUTXT = "ABSC", CREATED_DATE = Convert.ToDateTime("2015-05-29 10:55:58.143") });
            compannyDummy.Add(new T001() {  BUKRS = "102", BUTXT = "ABSC", CREATED_DATE = Convert.ToDateTime("2015-06-29 10:55:58.143") });

            return compannyDummy;
        }

    }
}
