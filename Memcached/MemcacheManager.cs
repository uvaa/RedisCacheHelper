using EC.Libraries.Cache.Memcached;
using EC.Libraries.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EC.Libraries.Cache
{
    /*
       <appSettings>
        <!--Memcached服务器的配置-->
        <add key="memcached" value="127.0.0.1:11211,127.0.0.2:11211,127.0.0.3:11211" />
      </appSettings>
     */

    /// <summary>
    /// 实现Memcached的客户端功能
    /// </summary>
    /// <remarks>2015-11-12 杨军  创建</remarks>
    internal class MemcacheManager : ICache
    {
        private static ICacheClient cacheClient;
        CacheConfig cacheConfig = null;

        /// <summary>
        /// Type of current cache client
        /// </summary>
        public CacheType Type
        { get { return CacheType.Memcached; } }

        /// <summary>
        /// 设置缓存配置实体
        /// </summary>
        /// <param name="config">缓存配置实体</param>
        public void SetConfig(CacheConfig config)
        {
            cacheConfig = config;
        }

        /// <summary>
        /// 初始化操作
        /// </summary>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public void Init()
        {
         
            if (cacheConfig == null)
            {
                var config = ConfigurationManager.GetSection("LibrariesConfig") as LibrariesConfig;
                if (config != null)
                {
                    cacheConfig = config.GetObjByXml<CacheConfig>("CacheConfig");

                    if (cacheConfig == null)
                        throw new Exception("缺少本地缓存配置节点");
                }
            }

            if (cacheClient == null)
            {
                lock (typeof(MemcacheManager))
                {
                    cacheClient = new MemcachedClientCache(cacheConfig.Url.Split(',').ToList());
                }
            }

        }

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">值对应的泛型类型</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Key对应的Value</returns>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public T Get<T>(string key) where T : class
        {
            return cacheClient.Get<T>(key);
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
            var obj = cacheClient.Get<object>(key);
            return Newtonsoft.Json.JsonConvert.DeserializeObject(obj.ToString(), type);
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T">需要保存值的泛型类型</typeparam>
        /// <param name="key">Key</param>
        /// <param name="data">缓存的值</param>
        /// <param name="cacheTime">缓存时长（单位：分钟）</param>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public void Set<T>(string key, T data, int cacheTime)
        {
            cacheClient.Set<T>(key, data,new TimeSpan(0, cacheTime, 0));
        }

        public bool Add<T>(string key, T data, int cacheTime)
        {
           return cacheClient.Add<T>(key, data, new TimeSpan(0, cacheTime, 0));
        }

        /// <summary>
        /// 检测缓存是否有效
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True=有效 False=无效</returns>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public bool IsSet(string key)
        {
            if (cacheClient.Get<object>(key) == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 通过Key值移除缓存
        /// </summary>
        /// <param name="key">Key</param>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public void Remove(string key)
        {
            cacheClient.Remove(key);
        }

        /// <summary>
        /// 通过正则表达式移除缓存
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public void RemoveByPattern(string pattern)
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public void Clear()
        {
            cacheClient.FlushAll();
        }


        /// <summary>
        ///获取所有key
        /// </summary>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public IList<string> GetAllKey()
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        /// <summary>
        ///获取缓存中有效的所有Key
        /// </summary>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public void Dispose()
        {
            cacheClient.Dispose();
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
            return cacheClient.Increment(key, amount);
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
            return cacheClient.Decrement(key, amount);
        }

        /// <summary>
        /// 获取递增、递减Key的当前值
        /// </summary>
        /// <param name="key">键码</param>
        /// <returns>当前值</returns>
        public long GetCountVal(string key)
        {
            return cacheClient.Increment(key, 0);
        }

        public bool HashExists(string key, string dataKey)
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public bool HashSet<T>(string key, string dataKey, T t)
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public bool HashDelete(string key, string dataKey)
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public long HashRemoveAll(string key)
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public T HashGet<T>(string key, string dataKey) where T : class
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public List<T> HashKeys<T>(string key) where T : class
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public Dictionary<string, T> HashGetAll<T>(string key) where T : class
        {
            throw new Exception("Memcache缓存没有该功能");
        }

        public bool ListAdd<T>(string key, List<T> value)
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public long ListRightPush<T>(string key, T value)
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public T ListRightPop<T>(string key) where T : class
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public long ListRemoveAll<T>(string key)
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public List<T> ListGet<T>(string key) where T : class
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public long ListLength(string key)
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public bool SortedSetAdd<T>(string key, T value, double score)
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public bool SortedSetRemove<T>(string key, T value)
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public bool SortedSetRemoveAll(string key)
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public List<T> SortedSetGetAll<T>(string key) where T : class
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public long SortedSetLength(string key)
        {
             throw new Exception("Memcache缓存没有该功能");
        }

        public long HashLength(string key)
        {
            throw new NotImplementedException();
        }
    }
}
