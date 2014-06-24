#region

using System.Threading;
using System.Threading.Tasks;

#endregion

namespace FaceDetection.Library.DataAcccessLayer
{
    interface IUnitOfWorkAsync : IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : EntityBase;
        Task<int> CommitAsync();
    }
}