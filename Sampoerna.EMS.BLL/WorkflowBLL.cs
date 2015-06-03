using System.Collections.Generic;
using System.Linq;
using Sampoerna.EMS.BusinessObject;
using Sampoerna.EMS.BusinessObject.Business;
using Sampoerna.EMS.Contract;
using Voxteneo.WebComponents.Logger;

namespace Sampoerna.EMS.BLL
{
    public class WorkflowBLL : IWorkflowBLL
    {
        private ILogger _logger;
        private IUnitOfWork _uow;
        private IGenericRepository<USER> _repository;

        public WorkflowBLL(IUnitOfWork uow, ILogger logger)
        {
            _logger = logger;
            _uow = uow;
            _repository = _uow.GetGenericRepository<USER>();
        }

        public List<UserTree> GetUserTree()
        {

            var users = _repository.Get().ToList();
            var usersTree =new List<UserTree>();
            foreach (var user in users)
            {
                var tree = new UserTree();
                tree.Id = user.USER_ID;
                if (user.MANAGER_ID != null)
                {
                    tree.Manager = _repository.Get(p => p.USER_ID == user.MANAGER_ID).FirstOrDefault();
                }

                tree.Employees = _repository.Get(p => p.MANAGER_ID != null).Where(x => x.MANAGER_ID.Equals(user.USER_ID)).ToList();
                usersTree.Add(tree);
            }
            return usersTree;
        }
        
        public UserTree GetUserTreeByUserID(int userID)
        {
            var user = _repository.Get(p => p.USER_ID == userID).FirstOrDefault();
            if (userID == null)
                return null;
            
            var tree = new UserTree();
            tree.Id = user.USER_ID;
            if (user.MANAGER_ID != null)
            {
                tree.Manager = _repository.Get(p => p.USER_ID == user.MANAGER_ID).FirstOrDefault();
            }

            tree.Employees = _repository.Get(p => p.MANAGER_ID != null).Where(x => x.MANAGER_ID.Equals(user.USER_ID)).ToList();
            return tree;
        }
        
    }
}
