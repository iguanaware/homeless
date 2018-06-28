using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PollyExample
{
    public class CacheableResults<TResult>
    {
        [DebuggerDisplay("{Item}")]
        private class Wrapper<T>
        {
            public Wrapper(T item)
            {
                this.Item = item;
            }
            public T Item { get; set; }

            public override string ToString()
            {
                return this.Item.ToString();
            }
        }

        private readonly Microsoft.Extensions.Caching.Memory.IMemoryCache memoryCache;
        private readonly Polly.Caching.Memory.MemoryCacheProvider memoryCacheProvider;
        private readonly Polly.Caching.CachePolicy cachePolicy;
        private readonly Polly.Caching.CachePolicy cachePolicyAsync;
        private readonly Polly.Caching.Ttl _dummy_ttl = new Polly.Caching.Ttl(TimeSpan.FromSeconds(1));
        private readonly System.Threading.CancellationToken _dummy_cancellationToken = new System.Threading.CancellationToken();
        public CacheableResults(TimeSpan ts = default(TimeSpan))
        {
            if (ts == default(TimeSpan))
            {
                ts = TimeSpan.FromMinutes(15);
            }
            memoryCache = new Microsoft.Extensions.Caching.Memory.MemoryCache(new Microsoft.Extensions.Caching.Memory.MemoryCacheOptions() { });
            memoryCacheProvider = new Polly.Caching.Memory.MemoryCacheProvider(memoryCache);
            cachePolicy = Polly.Policy.Cache(memoryCacheProvider, ts);
            cachePolicyAsync = Polly.Policy.CacheAsync(memoryCacheProvider, ts);
        }

        [DebuggerStepThrough]
        public async Task<TResult> ExecuteAsync(Func<System.Threading.CancellationToken, Task<TResult>> function, string key, System.Threading.CancellationToken cancellationToken)
        {
            Func<Polly.Context, System.Threading.CancellationToken, Task<Wrapper<TResult>>> revisedfunction = async (context, _cancellationToken) =>
           {
               TResult result = await function(_cancellationToken);
               return new Wrapper<TResult>(result);
           };
            Wrapper<TResult> r = await cachePolicyAsync.ExecuteAsync<Wrapper<TResult>>(revisedfunction, new Polly.Context(key), cancellationToken);
            return r.Item;
        }

        [DebuggerStepThrough]
        public async Task<TResult> ExecuteAsync(Func<Task<TResult>> function, string key)
        {
            Func<System.Threading.CancellationToken, Task<TResult>> revisedfunction = async (a) =>
            {
                return await function();
            };
            return await this.ExecuteAsync(revisedfunction, key, _dummy_cancellationToken);
        }

        [DebuggerStepThrough]
        public TResult Execute(Func<System.Threading.CancellationToken, TResult> function, string key, System.Threading.CancellationToken cancellationToken)
        {
            Func<Polly.Context, System.Threading.CancellationToken, Wrapper<TResult>> revisedfunction = (context, _cancellationToken) =>
            {
                TResult result = function(_cancellationToken);
                return new Wrapper<TResult>(result);
            };
            Wrapper<TResult> r = cachePolicy.Execute<Wrapper<TResult>>(revisedfunction, new Polly.Context(key), cancellationToken);
            return r.Item;
        }
        [DebuggerStepThrough]
        public TResult Execute(Func<TResult> function, string key)
        {
            Func<System.Threading.CancellationToken, TResult> revisedfunction = (cancellationToken) => { return function(); };
            return this.Execute(revisedfunction, key, _dummy_cancellationToken);
        }
        [DebuggerStepThrough]
        public void Clear(string key)
        {
            memoryCacheProvider.Put(key, null, _dummy_ttl);
        }
        [DebuggerStepThrough]
        public async void ClearAsync(string key, System.Threading.CancellationToken cancellationToken)
        {
            await memoryCacheProvider.PutAsync(key, null, _dummy_ttl, cancellationToken, false);
        }


    }
}
