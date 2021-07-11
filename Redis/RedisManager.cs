using EC.Libraries.Cache.Redis;
using EC.Libraries.Framework;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace EC.Libraries.Cache
{
    /// <summary>
    /// 实现Redis的客户端调用类
    /// </summary>
    /// <remarks>2016-03-14 杨军  创建</remarks>
    internal class RedisManager : ICache
    {
        CacheConfig cacheConfig = null;

        private static IConnectionMultiplexer cacheClient;
        private static IDatabase db
        {
            get { return cacheClient.GetDatabase(); }
        }

        /// <summary>
        /// 缓存类型
        /// </summary>
        public CacheType Type
        { get { return CacheType.Redis; } }

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
                try
                {
                    var config = ConfigurationManager.GetSection("LibrariesConfig") as LibrariesConfig;
                    if (config != null)
                    {
                        cacheConfig = config.GetObjByXml<CacheConfig>("CacheConfig");

                        if (cacheConfig == null)
                            throw new Exception("缺少本地缓存配置节点");
                    }
                }
                catch
                {
                    throw new Exception("缺少Redis配置节点");
                }
            }

            if (cacheClient == null)
            {
                lock (typeof(RedisManager))
                {
                    cacheClient = ConnectionMultiplexer.Connect(cacheConfig.Url);
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
            key = key.ToLower();

            var result = db.StringGet(key);
            if (result.IsNullOrEmpty) return default;
            return JsonHelper.ToObject<T>(result.ToString());
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
            key = key.ToLower();
            var result = db.StringGet(key);
            return Newtonsoft.Json.JsonConvert.DeserializeObject(result.ToString(), type);
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
            key = key.ToLower();

            if (data == null) return;

            db.StringSet(key, JsonHelper.ToJson(data), TimeSpan.FromMinutes(cacheTime));

        }

        /// <summary>
        /// 设置缓存(不存在才新增)
        /// </summary>
        /// <typeparam name="T">需要保存值的泛型类型</typeparam>
        /// <param name="key">Key</param>
        /// <param name="data">缓存的值</param>
        /// <param name="cacheTime">缓存时长（单位：分钟）</param>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public bool Add<T>(string key, T data, int cacheTime)
        {
            key = key.ToLower();

            if (data == null) return false;

            return db.StringSet(key, JsonHelper.ToJson(data), TimeSpan.FromMinutes(cacheTime), When.NotExists);
        }

        /// <summary>
        /// 检测缓存是否存在
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True=有效 False=无效</returns>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public bool IsSet(string key)
        {
            key = key.ToLower();
            return db.KeyExists(key);
        }


        /// <summary>
        /// 通过Key值移除缓存
        /// </summary>
        /// <param name="key">Key</param>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public void Remove(string key)
        {
            key = key.ToLower();
            db.KeyDelete(key);
        }


        /// <summary>
        /// 通过正则表达式移除缓存
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public void RemoveByPattern(string pattern)
        {
            pattern = string.Format("*{0}*", pattern.ToLower());
            var endpoints = cacheClient.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = cacheClient.GetServer(endpoint);
                var config = ConfigurationOptions.Parse(server.Multiplexer.Configuration);
                var keys = server.Keys(database: config.DefaultDatabase ?? 0, pattern: pattern, pageSize: int.MaxValue);

                if (keys != null)
                {
                    foreach (var redisKey in keys)
                    {
                        if (db.KeyExists(redisKey))
                        {
                            db.KeyDelete(redisKey);
                        }
                    }
                }
            }

        }


        /// <summary>
        /// 清除缓存
        /// </summary>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public void Clear()
        {
            var endpoint = db.IdentifyEndpoint();
            var server = cacheClient.GetServer(endpoint);
            server.FlushDatabase(db.Database);
        }

        /// <summary>
        ///获取所有key
        /// </summary>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        public IList<string> GetAllKey()
        {
            IList<string> result = new List<string>();
            IEnumerable<RedisKey> keys = null;
            var endpoints = cacheClient.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = cacheClient.GetServer(endpoint);
                var config = ConfigurationOptions.Parse(server.Multiplexer.Configuration);
                keys = server.Keys(database: config.DefaultDatabase ?? 0, pattern: "*", pageSize: int.MaxValue);
            }

            if (keys != null)
            {
                foreach (var item in keys)
                {
                    result.Add(item.ToString());
                }
            }

            return result;

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
            return db.HashIncrement(key, "count", amount);
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
            return db.HashDecrement(key, "count", amount);
        }

        /// <summary>
        /// 获取递增、递减Key的当前值
        /// </summary>
        /// <param name="key">键码</param>
        /// <returns>当前值</returns>
        public long GetCountVal(string key)
        {
            var result = db.HashGet(key, "count");

            if (result.IsNullOrEmpty) return 0;

            return long.Parse(result);
        }

        #region Hash缓存

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashExists(string key, string dataKey)
        {
            return db.HashExists(key, dataKey);
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool HashSet<T>(string key, string dataKey, T t)
        {
            string json = JsonHelper.ToJson(t);
            return db.HashSet(key, dataKey, json);

        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string dataKey)
        {
            return db.HashDelete(key, dataKey);
        }

        /// <summary>
        /// 根据key移除hash中所有键值
        /// </summary>
        /// <param name="key"></param>
        /// <returns>移除数量</returns>
        public long HashRemoveAll(string key)
        {
            RedisValue[] values = db.HashKeys(key);
            return db.HashDelete(key, values);
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string dataKey) where T : class
        {
            string value = db.HashGet(key, dataKey);
            if (string.IsNullOrEmpty(value)) return null;
            return JsonHelper.ToObject<T>(value); ;
        }

        /// <summary>
        /// 获取hashkey所有键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> HashKeys<T>(string key) where T : class
        {
            RedisValue[] values = db.HashKeys(key);
            return ConvetList<T>(values);
        }

        /// <summary>
        /// 获取所有hash数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dictionary<string, T> HashGetAll<T>(string key) where T : class
        {
            var res = new Dictionary<string, T>();
            var value = db.HashGetAll(key);
            if (value != null)
            {
                foreach (var hashEntry in value)
                {
                    res.Add(hashEntry.Name, JsonHelper.ToObject<T>(hashEntry.Value));
                }
            }
            return res; ;
        }
        /// <summary>
        /// 获取hash中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long HashLength(string key)
        {
            return db.HashLength(key);
        }

        private List<T> ConvetList<T>(RedisValue[] values) where T : class
        {
            List<T> result = new List<T>();
            if (values != null)
            {
                foreach (var item in values)
                {
                    var model = JsonHelper.ToObject<T>(item);
                    result.Add(model);
                }
            }
            return result;
        }
        #endregion 同步方法

        #region List
        /// <summary>
        /// 插入列表数据(如果已存在则返回false)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="value">列表</param>
        public bool ListAdd<T>(string key, List<T> value)
        {
            long i = 0;
            if (ListLength(key) == 0)
            {
                //下面的database 是redis的数据库对象.
                foreach (var single in value)
                {
                    var s = JsonHelper.ToJson(single); //序列化
                    i = db.ListRightPush(key, s); //要一个个的插入
                }
            }
            return i > 0;
        }

        /// <summary>
        /// 往list队尾插入数据，不存在则新增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public long ListRightPush<T>(string key, T value)
        {
            return db.ListRightPush(key, JsonHelper.ToJson(value));
        }

        /// <summary>
        /// 移除key所有的List
        /// </summary>
        /// <param name="key"></param>
        public long ListRemoveAll<T>(string key)
        {
            long i = 0;
            var values = db.ListRange(key);
            foreach (var redisValue in values)
            {
                i += db.ListRemove(key, redisValue);
            }
            return i;
        }

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> ListGet<T>(string key) where T : class
        {
            var values = db.ListRange(key);
            return ConvetList<T>(values);
        }

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long ListLength(string key)
        {
            return db.ListLength(key);
        }

        #endregion

        #region SortedSet 有序集合
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="score">排序号</param>
        public bool SortedSetAdd<T>(string key, T value, double score)
        {
            return db.SortedSetAdd(key, JsonHelper.ToJson(value), score);
        }

        /// <summary>
        /// 根据指定值删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public bool SortedSetRemove<T>(string key, T value)
        {
            return db.SortedSetRemove(key, JsonHelper.ToJson(value));
        }

        /// <summary>
        /// 根据指定值删除
        /// </summary>
        /// <param name="key"></param>
        public bool SortedSetRemoveAll(string key)
        {
            var res = false;
            var values = db.SortedSetRangeByRank(key);
            foreach (var redisValue in values)
            {
                res = db.SortedSetRemove(key, redisValue);
            }
            return res;
        }

        /// <summary>
        /// 根据key获取全部数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> SortedSetGetAll<T>(string key) where T : class
        {
            var values = db.SortedSetRangeByRank(key);
            return ConvetList<T>(values);
        }

        /// <summary>
        /// 根据key获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long SortedSetLength(string key)
        {
            return db.SortedSetLength(key);
        }

        #endregion

    }
}
