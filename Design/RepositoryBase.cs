using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ESuntikanAppData
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        public ESuntikanLocalDB context;

        protected DbSet<T> dbSet
        {
            get
            {
                return context.Set<T>();
            }
        }

        public RepositoryBase(ESuntikanLocalDB context)
        {
            this.context = context;
        }

        public RepositoryBase()
        {

        }

        #region IRepository Members

        public T Add(T entity)
        {
            try
            {
                var newEntry = dbSet.Add(entity);
                context.SaveChanges();
                return newEntry;
            }
            catch (DbEntityValidationException ex)
            {
                dbSet.Remove(entity);
                throw ex;
            }
            catch (Exception ex)
            {
                dbSet.Remove(entity);
                throw ex;
            }
            return null;
        }

        public void AddMultiple(T entity)
        {
            try
            {
                var newEntry = dbSet.Add(entity);
            }
            catch (DbEntityValidationException ex)
            {
                dbSet.Remove(entity);
            }
            catch (Exception ex)
            {
            }
        }

        public bool Add()
        {
            try
            {
                context.SaveChanges();

                return true;
            }
            catch (DbEntityValidationException ex)
            {
            }
            catch (Exception ex)
            {
                throw ex;

            }

            return false;
        }

        public void UpdateMultiple(T entity)
        {
            var entry = context.Entry<T>(entity);

            var pkey = dbSet.Create().GetType().GetProperty("ID").GetValue(entity, null);

            if (entry.State != EntityState.Detached)
            {
                var set = context.Set<T>();
                T attachedEntity = set.Find(pkey);
                if (attachedEntity != null)
                {
                    var attachedEntry = context.Entry(attachedEntity);
                    attachedEntry.CurrentValues.SetValues(entity);
                }
                else
                {
                    entry.State = EntityState.Modified; // This should attach entity
                }
            }
        }

        public int Update(T entity)
        {
            try
            {
                if (entity == null)
                {
                    throw new ArgumentException("Cannot add a null entity.");
                }

                var entry = context.Entry<T>(entity);
                object pkey;

                try
                {
                    pkey = dbSet.Create().GetType().GetProperty("ID").GetValue(entity, null);
                }
                catch
                {
                    pkey = dbSet.Create().GetType().GetProperty("Code").GetValue(entity, null);
                }

                if (entry.State != EntityState.Detached)
                {
                    var set = context.Set<T>();
                    T attachedEntity = set.Find(pkey);
                    if (attachedEntity != null)
                    {
                        var attachedEntry = context.Entry(attachedEntity);
                        attachedEntry.CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entry.State = EntityState.Modified; // This should attach entity
                    }

                    return context.SaveChanges();
                }
                else if (entry.State == EntityState.Modified)
                    return context.SaveChanges();
            }
            catch (EntityException ex)
            {
            }
            catch (Exception ex)
            {
                return 0;
            }

            return 0;
        }

        public int Delete(T entity)
        {
            try
            {
                dbSet.Remove(entity);
                return context.SaveChanges();
            }
            catch
            {
                var entry = context.Entry<T>(entity);
                entry.State = EntityState.Unchanged;
                return 0;
            }

        }

        public int DeleteMultiple(IQueryable<T> entities)
        {
            foreach (var entity in entities)
            {
                dbSet.Remove(entity);
            }

            try
            {
                return context.SaveChanges();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public IQueryable<T> GetAll()
        {
            return dbSet.AsQueryable();
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> whereCondition)
        {
            return dbSet.Where(whereCondition);
        }

        public T GetSingle(Expression<Func<T, bool>> whereCondition)
        {
            var results = dbSet.Where(whereCondition);

            if (results.Count() > 0)
            {
                return results.FirstOrDefault<T>();
            }
            else
            {
                return null;
            }
        }

        public void Attach(T entity)
        {
            dbSet.Attach(entity);
        }

        public IQueryable<T> GetQueryable()
        {
            return dbSet.AsQueryable<T>();
        }

        public long Count()
        {
            return dbSet.Count();
        }

        public long Count(Expression<Func<T, bool>> whereCondition)
        {
            return dbSet.Where(whereCondition).LongCount<T>();
        }

        #endregion
    }
}
