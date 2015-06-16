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
            users.Add(new USER() { USER_ID = 1, USERNAME = "kist", FIRST_NAME = "Jeanette", LAST_NAME = "Kist", IS_ACTIVE = false, USER_GROUP_ID = 4, MANAGER_ID = null });
            users.Add(new USER() { USER_ID = 2, USERNAME = "Jackqueline", FIRST_NAME = "Jackqueline", LAST_NAME = "Samms", IS_ACTIVE = false, USER_GROUP_ID = 4, MANAGER_ID = 1 });
            users.Add(new USER() { USER_ID = 3, USERNAME = "Lovella", FIRST_NAME = "Lovella", LAST_NAME = "Chouinard", IS_ACTIVE = false, USER_GROUP_ID = 4, MANAGER_ID = 2 });
            users.Add(new USER() { USER_ID = 4, USERNAME = "Valentine", FIRST_NAME = "Valentine", LAST_NAME = "Alm", IS_ACTIVE = false, USER_GROUP_ID = 4, MANAGER_ID = 1 });
            users.Add(new USER() { USER_ID = 5, USERNAME = "Aliza", FIRST_NAME = "Aliza", LAST_NAME = "Hennen", IS_ACTIVE = false, USER_GROUP_ID = 4, MANAGER_ID = 1 });
            users.Add(new USER() { USER_ID = 6, USERNAME = "Ilona", FIRST_NAME = "Ilona", LAST_NAME = "Escobar", IS_ACTIVE = false, USER_GROUP_ID = 4, MANAGER_ID = 1 });
            users.Add(new USER() { USER_ID = 7, USERNAME = "Shasta", FIRST_NAME = "Shasta", LAST_NAME = "Ruis", IS_ACTIVE = false, USER_GROUP_ID = 4, MANAGER_ID = 2 });
            users.Add(new USER() { USER_ID = 8, USERNAME = "Aliza", FIRST_NAME = "Aliza", LAST_NAME = "Hennen", IS_ACTIVE = false, USER_GROUP_ID = 4, MANAGER_ID = 2 });
            users.Add(new USER() { USER_ID = 9, USERNAME = "Celsa", FIRST_NAME = "Celsa", LAST_NAME = "Nickel", IS_ACTIVE = false, USER_GROUP_ID = 4, MANAGER_ID = 1 });
            users.Add(new USER() { USER_ID = 10, USERNAME = "Gregg", FIRST_NAME = "Gregg", LAST_NAME = "Fahey", IS_ACTIVE = false, USER_GROUP_ID = 4, MANAGER_ID = 1 });

            return users;
        }

        public static IEnumerable<USER> GetGenericJoinedUserStubs()
        {
            var users = GetGenericUserStubs().AsQueryable();
            var userGroups = GetUserGroupStubs().AsQueryable();
            var query = from u in users
                        select new USER()
                        {
                            USER_ID = u.USER_ID,
                            USERNAME = u.USERNAME,
                            FIRST_NAME = u.FIRST_NAME,
                            IS_ACTIVE = u.IS_ACTIVE,
                            LAST_NAME = u.LAST_NAME,
                            MANAGER_ID = u.MANAGER_ID,
                            USER2 = u.MANAGER_ID.HasValue ? (users.FirstOrDefault(s => s.USER_ID == u.MANAGER_ID.Value)) : null,
                            USER1 = users.Where(s => s.MANAGER_ID.HasValue && s.MANAGER_ID.Value == u.USER_ID).ToList(),
                            USER_GROUP = userGroups.FirstOrDefault(s => s.GROUP_ID == u.USER_GROUP_ID),
                            USER_GROUP_ID = u.USER_GROUP_ID
                        };
            return query.ToList();
        }

        public static IEnumerable<UserTree> GetGenericUserTreeStubs()
        {
            var users = GetGenericUserStubs().AsQueryable();
            var userGroups = GetUserGroupStubs().AsQueryable();
            var query = from u in users
                        select new UserTree()
                        {
                            USER_ID = u.USER_ID,
                            USERNAME = u.USERNAME,
                            FIRST_NAME = u.FIRST_NAME,
                            IS_ACTIVE = u.IS_ACTIVE,
                            LAST_NAME = u.LAST_NAME,
                            MANAGER_ID = u.MANAGER_ID,
                            Manager = u.MANAGER_ID.HasValue ? (users.FirstOrDefault(s => s.USER_ID == u.MANAGER_ID.Value)) : null,
                            Employees = users.Where(s => s.MANAGER_ID.HasValue && s.MANAGER_ID.Value == u.USER_ID).ToList(),
                            USER_GROUP = userGroups.FirstOrDefault(s => s.GROUP_ID == u.USER_GROUP_ID),
                            USER_GROUP_ID = u.USER_GROUP_ID
                        };
            return query.ToList();
        }

        public static IEnumerable<USER_GROUP> GetUserGroupStubs()
        {
            var rc = new List<USER_GROUP>();
            rc.Add(new USER_GROUP() { GROUP_ID = 1, GROUP_NAME = "Administrator" });
            rc.Add(new USER_GROUP() { GROUP_ID = 2, GROUP_NAME = "Supervisor" });
            rc.Add(new USER_GROUP() { GROUP_ID = 3, GROUP_NAME = "Plan" });
            rc.Add(new USER_GROUP() { GROUP_ID = 4, GROUP_NAME = "Operator" });
            return rc;
        }

        public static IEnumerable<T1001> GetCompany()
        {

            var compannyDummy = new List<T1001>();
            compannyDummy.Add(new T1001() { COMPANY_ID = 10, BUKRS = "101", BUKRSTXT = "ABSC", CREATED_DATE = Convert.ToDateTime("2015-05-29 10:55:11.317") });
            compannyDummy.Add(new T1001() { COMPANY_ID = 11, BUKRS = "102", BUKRSTXT = "ABSC", CREATED_DATE = Convert.ToDateTime("2015-05-29 10:55:12.980") });
            compannyDummy.Add(new T1001() { COMPANY_ID = 12, BUKRS = "103", BUKRSTXT = "ABSC", CREATED_DATE = Convert.ToDateTime("2015-05-29 10:55:56.660") });
            compannyDummy.Add(new T1001() { COMPANY_ID = 13, BUKRS = "104", BUKRSTXT = "ABSC", CREATED_DATE = Convert.ToDateTime("2015-05-29 10:55:58.143") });
            compannyDummy.Add(new T1001() { COMPANY_ID = 14, BUKRS = "102", BUKRSTXT = "ABSC", CREATED_DATE = Convert.ToDateTime("2015-06-29 10:55:58.143") });

            return compannyDummy;
        }

    }
}
