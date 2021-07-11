using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace EC.Libraries.Cache
{

    /// <summary>
    /// 本地缓存实现
    /// </summary>
    /// <remarks>2016-03-14 杨军  创建</remarks>
    internal class MemoryCacheManager : ICache
    {
        CacheConfig cacheConfig = null;
        /// <summary>
        /// 当前缓存类型
        /// </summary>
        public CacheType Type
        { get { return CacheType.Local; } }

        /// <summary>
        /// 实现初始化配置工作，如载入配置数据
        /// </summary>
        public void Init()
        { 
        
        }

        /// <summary>
        /// 设置缓存配置实体
        /// </summary>
        /// <param name="config">缓存配置实体</param>
        public void SetConfig(CacheConfig config)
        {
            cacheConfig = config;
        }

        /// <summary>
        /// 缓存管理器
        /// </summary>
        protected ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
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
            return (T)Cache[key];
        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="type">值对应的类型</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Key对应的Value</returns>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public object Get(string key, Type type)
        {
            return Cache[key];
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
            if (data == null)
                return;

            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            Cache.Set(new CacheItem(key, data), policy);
        }

        public bool Add<T>(string key, T data, int cacheTime)
        {
            if (data == null)
                return false;
            var res = false;
            var policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + TimeSpan.FromMinutes(cacheTime);
            if (!Cache.Contains(key))
            {
                Cache.Set(new CacheItem(key, data), policy);
                res = true;
            }
            return res;
        }


        /// <summary>
        /// 检测缓存是否有效
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True=有效 False=无效</returns>
        public bool IsSet(string key)
        {
            return (Cache.Contains(key));
        }

        /// <summary>
        /// 通过Key值移除缓存
        /// </summary>
        /// <param name="key">Key</param>
        public void Remove(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// 通过正则表达式移除缓存
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        public void RemoveByPattern(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var keysToRemove = new List<String>();

            foreach (var item in Cache)
                if (regex.IsMatch(item.Key))
                    keysToRemove.Add(item.Key);

            foreach (string key in keysToRemove)
            {
                Remove(key);
            }
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void Clear()
        {
            foreach (var item in Cache)
                Remove(item.Key);
        }

        /// <summary>
        ///获取缓存中有效的所有Key
        /// </summary>
        public IList<string> GetAllKey()
        {
            var keys = Cache.Select(item => item.Key)
                         .ToList();
            return keys;
        }

        public void Dispose()
        { 
        
        }


        /// <summary>
        /// 键值递增
        /// </summary>
        /// <param name="key">键码</param>
        /// <param name="amount">递增值</param>
        /// <returns>返回值</returns>
        public long Increment(string key, uint amount)
        {
            throw new Exception("本地缓存没有Increment功能");
        }

        /// <summary>
        /// 键值递减
        /// </summary>
        /// <param name="key">键码</param>
        /// <param name="amount">递减值</param>
        /// <returns>返回值</returns>
        public long Decrement(string key, uint amount)
        {
            throw new Exception("本地缓存没有Decrement功能");
        }

        /// <summary>
        /// 获取递增、递减Key的当前值
        /// </summary>
        /// <param name="key">键码</param>
        /// <returns>当前值</returns>
        public long GetCountVal(string key)
        {
            throw new Exception("本地缓存没有GetCount功能");
        }

        public bool HashExists(string key, string dataKey)
        {
            throw new Exception("本地缓存没有该功能");
        }

        public bool HashSet<T>(string key, string dataKey, T t)
        {
           throw new Exception("本地缓存没有该功能");
        }

        public bool HashDelete(string key, string dataKey)
        {
           throw new Exception("本地缓存没有该功能");
        }

        public long HashRemoveAll(string key)
        {
           throw new Exception("本地缓存没有该功能");
        }

        public T HashGet<T>(string key, string dataKey) where T : class
        {
           throw new Exception("本地缓存没有该功能");
        }

        public List<T> HashKeys<T>(string key) where T : class
        {
           throw new Exception("本地缓存没有该功能");
        }

        public Dictionary<string, T> HashGetAll<T>(string key) where T : class
        {
            throw new Exception("本地缓存没有该功能");
        }

        public bool ListAdd<T>(string key, List<T> value)
        {
           throw new Exception("本地缓存没有该功能");
        }

        public long ListRightPush<T>(string key, T value)
        {
           throw new Exception("本地缓存没有该功能");
        }

        public T ListRightPop<T>(string key) where T : class
        {
           throw new Exception("本地缓存没有该功能");
        }

        public long ListRemoveAll<T>(string key)
        {
           throw new Exception("本地缓存没有该功能");
        }

        public List<T> ListGet<T>(string key) where T : class
        {
           throw new Exception("本地缓存没有该功能");
        }

        public long ListLength(string key)
        {
           throw new Exception("本地缓存没有该功能");
        }

        public bool SortedSetAdd<T>(string key, T value, double score)
        {
           throw new Exception("本地缓存没有该功能");
        }

        public bool SortedSetRemove<T>(string key, T value)
        {
           throw new Exception("本地缓存没有该功能");
        }

        public bool SortedSetRemoveAll(string key)
        {
           throw new Exception("本地缓存没有该功能");
        }

        public List<T> SortedSetGetAll<T>(string key) where T : class
        {
           throw new Exception("本地缓存没有该功能");
        }

        public long SortedSetLength(string key)
        {
           throw new Exception("本地缓存没有该功能");
        }

        public long HashLength(string key)
        {
            throw new NotImplementedException();
        }
    }
}