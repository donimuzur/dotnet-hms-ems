namespace Sampoerna.EMS.CustomService.Repositories
{
    public class RepositoryFactory
    {
        public static GenericRepository<TEntity> GetInstance<TEntity, TContext>(TContext context)
            where TEntity : class
            where TContext : Data.EMSDataModel
        {
            return new GenericRepository<TEntity>(context);
        }
    }
}
