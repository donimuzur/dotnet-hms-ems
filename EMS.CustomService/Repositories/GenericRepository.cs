using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Sampoerna.EMS.CustomService.Repositories
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        #region Generic Repository internal variables declaration
        internal Data.EMSDataModel Context;
        internal DbSet<TEntity> DbSet;
        #endregion

        #region Generic Repository Contructors
        public GenericRepository(Data.EMSDataModel context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        #endregion

        #region Generic Repository General Methods
        public virtual IQueryable<TEntity> Get()
        {
            IQueryable<TEntity> query = DbSet;
            return query;
        }

        public virtual IQueryable<TEntity> Get(int offset, int limit)
        {
            IQueryable<TEntity> query = DbSet;
            return query.Skip(offset).Take(limit);

        }

        public virtual TEntity Find(object id)
        {
            return DbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public virtual TEntity InsertWithReturn(TEntity entity)
        {
            return DbSet.Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Delete(TEntity entity)
        {
            if (Context.Entry(entity).State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            DbSet.Remove(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entity = DbSet.Find(id);
            Delete(entity);

        }
        public T UnProxy<T>(T proxyObject) where T : class
        {
            var proxyCreationEnabled = Context.Configuration.ProxyCreationEnabled;
            try
            {
                Context.Configuration.ProxyCreationEnabled = false;
                T poco = Context.Entry(proxyObject).CurrentValues.ToObject() as T;
                return poco;
            }
            finally
            {
                Context.Configuration.ProxyCreationEnabled = proxyCreationEnabled;
            }
        }

        public virtual void SaveChanges()
        {
            Context.SaveChanges();
        }
        #endregion

        #region Generic Repository Parameterized Methods
        public virtual IEnumerable<TEntity> GetMany(Func<TEntity, bool> criteria)
        {
            return DbSet.Where(criteria);
        }

        public virtual IQueryable<TEntity> GetManyQueryable(Func<TEntity, bool> criteria)
        {
            return DbSet.Where(criteria).AsQueryable();
        }

        public TEntity Get(Func<TEntity, bool> criteria)
        {
            return DbSet.Where(criteria).FirstOrDefault();
        }

        public void Delete(Func<TEntity, bool> criteria)
        {
            IQueryable<TEntity> objects = DbSet.Where(criteria).AsQueryable();
            foreach (TEntity obj in objects)
                DbSet.Remove(obj);
        }

        public IQueryable<TEntity> GetAll()
        {
            return DbSet;
        }

        public IQueryable<TEntity> GetWithInclude(System.Linq.Expressions.Expression<Func<TEntity, bool>> criteria, params string[] includes)
        {
            IQueryable<TEntity> query = DbSet;
            query = includes.Aggregate(query, (current, include) => current.Include(include));

            return query.Where(criteria);
        }

        public bool Exist(object id)
        {
            return DbSet.Find(id) != null;
        }

        public TEntity GetSingle(Func<TEntity, bool> criteria)
        {
            return DbSet.SingleOrDefault(criteria);
        }

        public TEntity GetFirst(Func<TEntity, bool> criteria)
        {
            return DbSet.FirstOrDefault(criteria);
        }
        #endregion
    }
}
