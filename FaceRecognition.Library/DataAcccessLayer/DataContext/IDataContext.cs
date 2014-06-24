#region

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace FaceDetection.Library.DataAcccessLayer
{
    interface IDataContext : IDisposable
    {
        DbSet<T> Set<T>() where T : class;
        DbEntityEntry Entry(object o);
        int SaveChanges();
        void SyncObjectState(object entity);
    }

    interface IDataContextAsync : IDataContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Task<int> SaveChangesAsync();
    }
}