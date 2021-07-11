using System;

namespace EC.Libraries.Cache.Memcached
{
    /// <summary>
    /// Factory to create ILog instances
    /// </summary>
    public interface ILogFactory
    {
        /// <summary>
        /// Gets the logger.
        /// </summary>
        ILog GetLogger(Type type);

        /// <summary>
        /// Gets the logger.
        /// </summary>
        ILog GetLogger(string typeName);
    }

}
