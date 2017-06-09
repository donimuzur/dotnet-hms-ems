using System;

namespace Sampoerna.EMS.CustomService.Repositories
{
    public class GenericService
    {
        protected readonly UnitOfWork uow;

        public GenericService()
        {
            this.uow = new UnitOfWork();
        }

        protected virtual Exception HandleException(string message, Exception inner)
        {
            return new Exception(message, inner);
        }
    }
}
