using System;
using System.Collections.Concurrent;
using System.Threading;

namespace blqw.DI
{

    /// <summary>
    /// 对象池
    /// </summary>
    class ObjectPool<T>
        where T : class
    {
        public ObjectPool(int capacity, Func<T> factory, Action<T> finalize)
        {
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
            Finalize = finalize ?? throw new ArgumentNullException(nameof(finalize));
            Capacity = capacity;
        }

        /// <summary>
        /// 对象池容量大小
        /// </summary>
        public int Capacity { get; }
        // 对象缓存
        private readonly ConcurrentQueue<T> _cache = new ConcurrentQueue<T>();
        // 计数器
        private int _counter = 0;
        /// <summary>
        /// 弹出对象
        /// </summary>
        public IDisposable Pop(out T obj)
        {
            // 尝试从缓存中获取对象
            _cache.TryDequeue(out obj);
            if (obj != null)
            {
                return new Recyclable(obj, this); // 返回可回收对象
            }

            // 计数器超过池最大容量,或计数器+1超过池最大容量,则直接返回新的 StringBuilder 且不回收
            if (_counter > Capacity || Interlocked.Increment(ref _counter) > Capacity)
            {
                obj = Factory();
                return NoRecycle; //不回收
            }
            obj = Factory();
            return new Recyclable(obj, this);
        }

        // 不执行回收操作的空对象
        private readonly IDisposable NoRecycle = new Recyclable(null, null);

        public Func<T> Factory { get; }
        public Action<T> Finalize { get; }

        /// <summary>
        /// 可回收的对象
        /// </summary>
        class Recyclable : IDisposable
        {
            /// <summary>
            /// 初始化对象
            /// </summary>
            /// <param name="obj">待回收的 对象</param>
            public Recyclable(T obj, ObjectPool<T> pool)
            {
                _object = obj;
                _pool = pool;
            }

            // 待回收的对象
            private T _object;
            private readonly ObjectPool<T> _pool;

            /// <summary>
            /// 回收对象
            /// </summary>
            public void Dispose()
            {
                var obj = Interlocked.Exchange(ref _object, null);
                if (obj != null)
                {
                    _pool.Finalize?.Invoke(obj);
                    _pool._cache.Enqueue(obj); //回收
                }
            }
            // 析构函数
            ~Recyclable() => Dispose();
        }
    }
}
