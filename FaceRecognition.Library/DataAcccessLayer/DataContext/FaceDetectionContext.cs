using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FaceDetection.Library.DataAcccessLayer
{
    class FaceDetectionContext : DbContext, IDataContext, IDataContextAsync
    {
        private readonly Guid _instanceId;

        public FaceDetectionContext() { }

        public FaceDetectionContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            _instanceId = Guid.NewGuid();
            Configuration.LazyLoadingEnabled = true;
            Configuration.ProxyCreationEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var entityMethod = typeof(DbModelBuilder).GetMethod("Entity");

                var entityTypes = assembly.GetTypes().Where(t => t.BaseType == typeof(EntityBase));

                foreach (var type in entityTypes)
                {
                    entityMethod.MakeGenericMethod(type)
                      .Invoke(modelBuilder, new object[] { });
                }
            }
        }
        public Guid InstanceId
        {
            get { return _instanceId; }
        }

        public new DbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        public override int SaveChanges()
        {
            var changes = base.SaveChanges();

            return changes;
        }

        public override Task<int> SaveChangesAsync()
        {
            var changesAsync = base.SaveChangesAsync();

            return changesAsync;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            var changesAsync = base.SaveChangesAsync(cancellationToken);

            return changesAsync;
        }

        public void SyncObjectState(object entity)
        {
            throw new NotImplementedException();
        }
    }
}
