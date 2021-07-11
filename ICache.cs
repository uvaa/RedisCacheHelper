using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EC.Libraries.Cache
{
    /// <summary>
    /// 缓存需要实现的接口
    /// </summary>
    /// <remarks>2016-03-15 杨军  创建</remarks>
   internal interface ICache:IDisposable
    {
        /// <summary>
        /// 当前采用的缓存类型
        /// </summary>
        CacheType Type
        { get; }

        /// <summary>
        /// 实现初始化配置工作，如载入配置数据
        /// </summary>
        void Init();

        /// <summary>
        /// 设置缓存配置实体
        /// </summary>
        /// <param name="config">缓存配置实体</param>
        void SetConfig(CacheConfig config);

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="T">值对应的泛型类型</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Key对应的Value</returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// 获取缓存
        /// </summary>
        /// <typeparam name="type">值对应的类型</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Key对应的Value</returns>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        object Get(string key, Type type);

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <typeparam name="T">需要保存值的泛型类型</typeparam>
        /// <param name="key">Key</param>
        /// <param name="data">缓存的值</param>
        /// <param name="cacheTime">缓存时长（单位：分钟）</param>
        void Set<T>(string key, T data, int cacheTime);

        /// <summary>
        /// 设置缓存(不存在才新增)
        /// </summary>
        /// <typeparam name="T">需要保存值的泛型类型</typeparam>
        /// <param name="key">Key</param>
        /// <param name="data">缓存的值</param>
        /// <param name="cacheTime">缓存时长（单位：分钟）</param>
        /// <remarks>2015-11-12 杨军  创建</remarks>
        bool Add<T>(string key, T data, int cacheTime);

        /// <summary>
        /// 检测缓存是否有效
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True=有效 False=无效</returns>
        bool IsSet(string key);


        /// <summary>
        /// 通过Key值移除缓存
        /// </summary>
        /// <param name="key">Key</param>
        void Remove(string key);


        /// <summary>
        /// 通过正则表达式移除缓存
        /// </summary>
        /// <param name="pattern">正则表达式</param>
        void RemoveByPattern(string pattern);

        /// <summary>
        /// 清除缓存
        /// </summary>
        void Clear();

        /// <summary>
        ///获取缓存中有效的所有Key
        /// </summary>
        IList<string> GetAllKey();


        /// <summary>
        /// 键值递增
        /// </summary>
        /// <param name="key">键码</param>
        /// <param name="amount">递增值</param>
        /// <returns>返回值</returns>
        long Increment(string key, uint amount);

        /// <summary>
        /// 键值递减
        /// </summary>
        /// <param name="key">键码</param>
        /// <param name="amount">递减值</param>
        /// <returns>返回值</returns>
        long Decrement(string key, uint amount);

        /// <summary>
        /// 获取递增、递减Key的当前值
        /// </summary>
        /// <param name="key">键码</param>
        /// <returns>当前值</returns>
        long GetCountVal(string key);

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        bool HashExists(string key, string dataKey);

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        bool HashSet<T>(string key, string dataKey, T t);

        /// <summary>
        /// 移除hash中的某键值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        bool HashDelete(string key, string dataKey);

        /// <summary>
        /// 根据key移除hash中所有键值
        /// </summary>
        /// <param name="key"></param>
        /// <returns>移除数量</returns>
        long HashRemoveAll(string key);

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        T HashGet<T>(string key, string dataKey) where T : class;

        /// <summary>
        /// 获取hashkey所有键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> HashKeys<T>(string key) where T : class;

        /// <summary>
        /// 获取所有hash数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Dictionary<string, T> HashGetAll<T>(string key) where T : class;

        /// <summary>
        /// 获取hash中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long HashLength(string key);

        /// <summary>
        /// 插入列表数据(如果已存在则返回false)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">键</param>
        /// <param name="value">列表</param>
        bool ListAdd<T>(string key, List<T> value);

        /// <summary>
        /// 往list队尾插入数据，不存在则新增
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        long ListRightPush<T>(string key, T value);

        ///// <summary>
        ///// 根据key移除list一条数据并返回
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //T ListRightPop<T>(string key) where T : class;

        /// <summary>
        /// 移除key所有的List
        /// </summary>
        /// <param name="key"></param>
        long ListRemoveAll<T>(string key);

        /// <summary>
        /// 获取指定key的List
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> ListGet<T>(string key) where T : class;

        /// <summary>
        /// 获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long ListLength(string key);

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="score">排序号</param>
        bool SortedSetAdd<T>(string key, T value, double score);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        bool SortedSetRemove<T>(string key, T value);

        /// <summary>
        /// 根据指定值删除
        /// </summary>
        /// <param name="key"></param>
        bool SortedSetRemoveAll(string key);

        /// <summary>
        /// 根据key获取全部数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        List<T> SortedSetGetAll<T>(string key) where T : class;

        /// <summary>
        /// 根据key获取集合中的数量
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        long SortedSetLength(string key);
    }
}
