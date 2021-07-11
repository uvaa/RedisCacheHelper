using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;

namespace EC.Libraries.Cache
{

    /// <summary>
    /// ���ػ���ʵ��
    /// </summary>
    /// <remarks>2016-03-14 ���  ����</remarks>
    internal class MemoryCacheManager : ICache
    {
        CacheConfig cacheConfig = null;
        /// <summary>
        /// ��ǰ��������
        /// </summary>
        public CacheType Type
        { get { return CacheType.Local; } }

        /// <summary>
        /// ʵ�ֳ�ʼ�����ù�������������������
        /// </summary>
        public void Init()
        { 
        
        }

        /// <summary>
        /// ���û�������ʵ��
        /// </summary>
        /// <param name="config">��������ʵ��</param>
        public void SetConfig(CacheConfig config)
        {
            cacheConfig = config;
        }

        /// <summary>
        /// ���������
        /// </summary>
        protected ObjectCache Cache
        {
            get
            {
                return MemoryCache.Default;
            }
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <typeparam name="T">ֵ��Ӧ�ķ�������</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Key��Ӧ��Value</returns>        
        public T Get<T>(string key) where T : class
        {
            return (T)Cache[key];
        }

        /// <summary>
        /// ��ȡ����
        /// </summary>
        /// <typeparam name="type">ֵ��Ӧ������</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Key��Ӧ��Value</returns>
        /// <remarks>2015-11-12 ���  ����</remarks>
        public object Get(string key, Type type)
        {
            return Cache[key];
        }

        /// <summary>
        /// ���û���
        /// </summary>
        /// <typeparam name="T">��Ҫ����ֵ�ķ�������</typeparam>
        /// <param name="key">Key</param>
        /// <param name="data">�����ֵ</param>
        /// <param name="cacheTime">����ʱ������λ�����ӣ�</param>
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
        /// ��⻺���Ƿ���Ч
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>True=��Ч False=��Ч</returns>
        public bool IsSet(string key)
        {
            return (Cache.Contains(key));
        }

        /// <summary>
        /// ͨ��Keyֵ�Ƴ�����
        /// </summary>
        /// <param name="key">Key</param>
        public void Remove(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// ͨ��������ʽ�Ƴ�����
        /// </summary>
        /// <param name="pattern">������ʽ</param>
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
        /// �������
        /// </summary>
        public void Clear()
        {
            foreach (var item in Cache)
                Remove(item.Key);
        }

        /// <summary>
        ///��ȡ��������Ч������Key
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
        /// ��ֵ����
        /// </summary>
        /// <param name="key">����</param>
        /// <param name="amount">����ֵ</param>
        /// <returns>����ֵ</returns>
        public long Increment(string key, uint amount)
        {
            throw new Exception("���ػ���û��Increment����");
        }

        /// <summary>
        /// ��ֵ�ݼ�
        /// </summary>
        /// <param name="key">����</param>
        /// <param name="amount">�ݼ�ֵ</param>
        /// <returns>����ֵ</returns>
        public long Decrement(string key, uint amount)
        {
            throw new Exception("���ػ���û��Decrement����");
        }

        /// <summary>
        /// ��ȡ�������ݼ�Key�ĵ�ǰֵ
        /// </summary>
        /// <param name="key">����</param>
        /// <returns>��ǰֵ</returns>
        public long GetCountVal(string key)
        {
            throw new Exception("���ػ���û��GetCount����");
        }

        public bool HashExists(string key, string dataKey)
        {
            throw new Exception("���ػ���û�иù���");
        }

        public bool HashSet<T>(string key, string dataKey, T t)
        {
           throw new Exception("���ػ���û�иù���");
        }

        public bool HashDelete(string key, string dataKey)
        {
           throw new Exception("���ػ���û�иù���");
        }

        public long HashRemoveAll(string key)
        {
           throw new Exception("���ػ���û�иù���");
        }

        public T HashGet<T>(string key, string dataKey) where T : class
        {
           throw new Exception("���ػ���û�иù���");
        }

        public List<T> HashKeys<T>(string key) where T : class
        {
           throw new Exception("���ػ���û�иù���");
        }

        public Dictionary<string, T> HashGetAll<T>(string key) where T : class
        {
            throw new Exception("���ػ���û�иù���");
        }

        public bool ListAdd<T>(string key, List<T> value)
        {
           throw new Exception("���ػ���û�иù���");
        }

        public long ListRightPush<T>(string key, T value)
        {
           throw new Exception("���ػ���û�иù���");
        }

        public T ListRightPop<T>(string key) where T : class
        {
           throw new Exception("���ػ���û�иù���");
        }

        public long ListRemoveAll<T>(string key)
        {
           throw new Exception("���ػ���û�иù���");
        }

        public List<T> ListGet<T>(string key) where T : class
        {
           throw new Exception("���ػ���û�иù���");
        }

        public long ListLength(string key)
        {
           throw new Exception("���ػ���û�иù���");
        }

        public bool SortedSetAdd<T>(string key, T value, double score)
        {
           throw new Exception("���ػ���û�иù���");
        }

        public bool SortedSetRemove<T>(string key, T value)
        {
           throw new Exception("���ػ���û�иù���");
        }

        public bool SortedSetRemoveAll(string key)
        {
           throw new Exception("���ػ���û�иù���");
        }

        public List<T> SortedSetGetAll<T>(string key) where T : class
        {
           throw new Exception("���ػ���û�иù���");
        }

        public long SortedSetLength(string key)
        {
           throw new Exception("���ػ���û�иù���");
        }

        public long HashLength(string key)
        {
            throw new NotImplementedException();
        }
    }
}