using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EC.Libraries.Framework;

namespace EC.Libraries.Cache
{
    /// <summary>
    /// 缓存配置实体
    /// </summary>
    /// <remarks>2016-03-14 杨军  创建</remarks>
    public class CacheConfig:BaseConfig
    {
        /// <summary>
        /// 采用的缓存方式：Local,Memcached,Redis
        /// </summary>
        //public new string Provider
        //{ set; get; }

        /// <summary>
        /// 连接缓存服务器的Url，若使用Local，则此值无效
        /// </summary>
        public string Url
        { set; get; }

    }
}
