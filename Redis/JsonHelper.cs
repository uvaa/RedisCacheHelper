namespace EC.Libraries.Cache.Redis
{
    /// <summary>
    /// Json序列化工具类    
    /// </summary>
    /// <remarks>
    /// 2013-02-22 罗雄伟 创建
    /// </remarks>
    public static class JsonHelper
    {
        /// <summary>
        /// Json序列化
        /// </summary>
        /// <param name="source">需要序列化对象</param>
        /// <returns>Json字符串</returns>
        public static string ToJson(this object source)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(source);
        }

        /// <summary>
        /// Json反序列化
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="jsonString">Json字符串</param>
        /// <returns>返回对象</returns>
        public static T ToObject<T>(this string jsonString) where T : class
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
        }

    }
}
