namespace Chronos.Server.Data.Repositories

open System
open System.Threading.Tasks

type IRepository<'TEntity> =
    abstract member InsertAsync: 'TEntity -> Task<bool>
    abstract member UpdateAsync: 'TEntity -> Task<bool>
    abstract member DeleteAsync: 'TEntity -> Task<bool>
    abstract member GetAsync: Guid -> Task<'TEntity option>
    inherit IDisposable
