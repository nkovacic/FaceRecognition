using FaceDetection.Library.Models;
using System.Threading;
using System.Threading.Tasks;

namespace FaceDetection.Library.DataAcccessLayer
{
    interface IRepositoryAsync<TEntity> : IRepository<TEntity> where TEntity : EntityBase
    {
        Task<TEntity> FindAsync(params object[] keyValues);
        Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues);
        Task<bool> DeleteAsync(params object[] keyValues);
        Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues);
    }
}