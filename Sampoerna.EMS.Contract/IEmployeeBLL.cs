using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sampoerna.EMS.BusinessObject;
namespace Sampoerna.EMS.Contract
{
    public interface IEmployeeBLL
    {
        void Add(Employee employee);
        List<Employee> GetAll();

        void Update(Employee employee);

        void Delete(Employee employee);

        void GetNextApprover(string employeeName);
    }
}
