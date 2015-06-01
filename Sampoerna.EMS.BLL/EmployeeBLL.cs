using Sampoerna.EMS.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Voxteneo.WebComponents.Logger;
using Sampoerna.EMS.BusinessObject;
namespace Sampoerna.EMS.BLL
{
    public class EmployeeBLL : IEmployeeBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;

        public EmployeeBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
        }


        public void Add(Employee employee)
        {
            var repo = _uow.GetGenericRepository<Employee>();
            repo.Insert(employee);
            _uow.SaveChanges();
        }


        public List<Employee> GetAll()
        {
            var repo = _uow.GetGenericRepository<Employee>();
            return repo.Get().ToList();
        }


        public void Update(Employee employee)
        {
            var repo = _uow.GetGenericRepository<Employee>();
            repo.Update(employee);
            _uow.SaveChanges();
        }


        public void Delete(Employee employee)
        {
            var repo = _uow.GetGenericRepository<Employee>();
            repo.Delete(employee);
            _uow.SaveChanges();
        }


        public void GetNextApprover(string employeeName)
        {
            throw new NotImplementedException();
        }
    }
}
