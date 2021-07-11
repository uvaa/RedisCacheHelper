using EC.Libraries.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace EC.Libraries.Cache
{

    /// <summary>
    /// 缓存调用实现
    /// </summary>
    /// <remarks>2016-03-14 杨军  创建</remarks>
    internal class CacheProvider : ICacheProvider
    {
        CacheType _cacheType;
        private static readonly object _lock = new object();
        private ICache _cacheClient = null;

        CacheConfig cacheConfig = null;

        /// <summary>
        /// 当前采用的缓存类型
        /// </summary>
        public CacheType Type { get { return _cacheType; } }

        /// <summary>
        /// 获取所需的基础调用实体
        /// </summary>
        /// <returns></returns>
        public ICacheProvider Instance
        {
            get
            {
                if (_cacheClient == null)
                    Init();

                return this;
            }
        }

        /// <summary>
        /// 获取调用服务的实例。
        /// </summary>
        /// <returns>类型实例</returns>
        /// <remarks>2016-02-17 余勇 添加</remarks>
        ICache CacheClient
        {
            get
            {
                if (_cacheClient == null)
                {
                    Init();
                }
                return _cacheClient;
            }
        }

        public void Init(BaseConfig config = null)
        {
            lock (_lock)
            {
                if (config != null)
                {
                    cacheConfig = (CacheConfig)config;
                }
                else
                {
                    if (ConfigurationManager.GetSection("LibrariesConfig") is LibrariesConfig librariesConfig)
                    {
                        cacheConfig = librariesConfig.GetObjByXml<CacheConfig>("CacheConfig");

                        if (cacheConfig == null)
                            throw new Exception("缺少缓存配置CacheConfig");
                    }
                }
                if (cacheConfig != null)
                {
                    switch ((CacheType)Enum.Parse(typeof(CacheType), cacheConfig.Provider))
                    {
                        //使用Memcached作为缓存服务器
                        case CacheType.Memcached:
                            _cacheClient = new MemcacheManager();
                            break;

                        //使用Redis作为缓存服务器
                        case CacheType.Redis:
                            _cacheClient = new RedisManager();
                            break;

                        //使用本地缓存作为缓存服务器
                        default:
                            _cacheClient = new MemoryCacheManager();
                            break;

                    }
                    _cacheType = _cacheClient.Type;
                    _cacheClient.SetConfig(cacheConfig);
                    _cacheClient.Init();
                }
                else
                {
                    throw new Exception("缺少缓存配置CacheConfig");
                }
            }
        }

        /// <summary>
        /// 获取或设置缓存(默认60分)
        /// </summary>
        /// <typeparam name="T">值对应的泛型类型</typeparam>
        /// <param name="key">Key</param>
        /// <param name="acquire">缓存方法</param>
        /// <param name="cacheTime">缓存时长（单位：分钟）</param>
        /// <returns>T</returns>
        /// <remarks>2016-04-19 余勇 添加</remarks>
        public T Get<T>(string key, Func<T> acquire, int cacheTime = 60) where T : class
        {
            if (CacheClient.IsSet(key))
            {
                return CacheClient.Get<T>(key);
            }
            else
            {
                var result = acquire();
                CacheClient.Set(key, result, cacheTime);
                return result;
            }
        }

        /// <summary>
        /// 获取或设置缓存(默认60分)
        /// </summary>
        /// <typeparam name="type">值对应的类型</typeparam>
        /// <param name="key">Key</param>
        /// <param name="acquire">缓存方法</param>
        /// <param name="cacheTime">缓存时长（单位：分钟）</param>
        /// <returns>T</returns>
        /// <remarks>2016-04-19 余勇 添加</remarks>
        public object Get(string key, Func<object> acquire, Type type, int cacheTime = 60)
        {
            if (CacheClient.IsSet(key))
            {
                return CacheClient.Get(key, type);
            }
            else
            {
                var result = acquire();
                CacheClient.Set(key, result, cacheTime);
                return result;
            }
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">值对应的泛型类型</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Key对应的Value</returns>
        public T Get<T>(string key) where T : class
        {
            return CacheClient.Get<T>(key);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T">需要保存值的泛型类型</typeparam>
        /// <param name="key">Key</param>
        /// <param name="data">缓存的值</param>
        /// <param name="cacheTime">缓存时长（单位：分钟）</param>
        public void Set<T>(string key, T data, int cacheTime)
        {
            CacheClient.Set<T>(key, data, cacheTime);
        }

        /// <summary>
        /// 检测缓存是否有效
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True=有效 False=无效</returns>
        public bool IsSet(string key)
        {
            return CacheClient.IsSet(key);
        }


        /// <summary>
        /// 通过Key值移除缓存
        /// </summary>
        /// <param name="key">Key</param>
        public void Remove(string key)
        {
            CacheClient.Remove(key);
        }


        /// <summary>
        /// 通过正则表达式移除缓存
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        public void RemoveByPattern(string pattern)
        {
            CacheClient.RemoveByPattern(pattern);
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void Clear()
        {
            CacheClient.Clear();
        }

        /// <summary>
        ///获取缓存中有效的所有Key
        /// </summary>
        public IList<string> GetAllKey()
        {
            return CacheClient.GetAllKey();
        }

        public void Dispose()
        {
            //if (_cacheClient != null)
            //{
            //   // _cacheClient.Dispose();
            //   // _cacheClient = null; //余勇 添加 执行Dispose后_cacheClient需要重新初始化
            //}
        }

        /// <summary>
        /// 键值递增
        /// </summary>
        /// <param name="key">键码</param>
        /// <param name="amount">递增值</param>
        /// <returns>返回值</returns>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public long Increment(string key, uint amount)
        {
            return CacheClient.Increment(key, amount);
        }

        /// <summary>
        /// 键值递减
        /// </summary>
        /// <param name="key">键码</param>
        /// <param name="amount">递减值</param>
        /// <returns>返回值</returns>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public long Decrement(string key, uint amount)
        {
            return CacheClient.Decrement(key, amount);
        }

        /// <summary>
        /// 获取递增、递减Key的当前值
        /// </summary>
        /// <param name="key">键码</param>
        /// <returns>当前值</returns>
        public long GetCountVal(string key)
        {
            return CacheClient.GetCountVal(key);
        }

        public bool HashExists(string key, string dataKey)
        {
            return CacheClient.HashExists(key, dataKey);
        }

        public bool HashSet<T>(string key, string dataKey, T t)
        {
            return CacheClient.HashSet<T>(key, dataKey, t);
        }

        public bool HashDelete(string key, string dataKey)
        {
            return CacheClient.HashDelete(key, dataKey);
        }

        public long HashRemoveAll(string key)
        {
            return CacheClient.HashRemoveAll(key);
        }

        public T HashGet<T>(string key, string dataKey) where T : class
        {
            return CacheClient.HashGet<T>(key, dataKey);
        }

        public List<T> HashKeys<T>(string key) where T : class
        {
            return CacheClient.HashKeys<T>(key);
        }

        public Dictionary<string, T> HashGetAll<T>(string key) where T : class
        {
            return CacheClient.HashGetAll<T>(key);
        }

        public bool ListAdd<T>(string key, List<T> value)
        {
            return CacheClient.ListAdd<T>(key, value);
        }

        public long ListRightPush<T>(string key, T value)
        {
            return CacheClient.ListRightPush<T>(key, value);
        }

        //public T ListRightPop<T>(string key) where T : class
        //{
        //    return CacheClient.ListRightPop<T>(key);
        //}

        public long ListRemoveAll<T>(string key)
        {
            return CacheClient.ListRemoveAll<T>(key);
        }

        public List<T> ListGet<T>(string key) where T : class
        {
            return CacheClient.ListGet<T>(key);
        }

        public long ListLength(string key)
        {
            return CacheClient.ListLength(key);
        }

        public bool SortedSetAdd<T>(string key, T value, double score)
        {
            return CacheClient.SortedSetAdd<T>(key, value, score);
        }

        public bool SortedSetRemove<T>(string key, T value)
        {
            return CacheClient.SortedSetRemove<T>(key, value);
        }

        public bool SortedSetRemoveAll(string key)
        {
            return CacheClient.SortedSetRemoveAll(key);
        }

        public List<T> SortedSetGetAll<T>(string key) where T : class
        {
            return CacheClient.SortedSetGetAll<T>(key);
        }

        public long SortedSetLength(string key)
        {
            return CacheClient.SortedSetLength(key);
        }

        public long HashLength(string key)
        {
            return CacheClient.HashLength(key);
        }
    }
}
