using Newtonsoft.Json;
using Nop.Core.Infrastructure;
using Nop.Core.Redis;
using StackExchange.Redis;

namespace Nop.Core.Plugins
{
    /// <summary>
    /// Represents an information about plugins
    /// </summary>
    public partial class RedisPluginsInfo : PluginsInfo
    {
        #region Fields

        private readonly IDatabase _db;

        #endregion

        #region Ctor

        public RedisPluginsInfo(INopFileProvider fileProvider, IRedisConnectionWrapper connectionWrapper)
            : base(fileProvider)
        {
            this._db = connectionWrapper.GetDatabase(RedisDatabaseNumber.Plugin);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Save plugins info to the redis
        /// </summary>
        public override void Save()
        {
            var text = JsonConvert.SerializeObject(this, Formatting.Indented);
            _db.StringSet(nameof(RedisPluginsInfo), text);
        }

        /// <summary>
        /// Save plugins info to the file
        /// </summary>
        public void SaveToFile()
        {
            base.Save();
        }

        /// <summary>
        /// Get plugins info
        /// </summary>
        public override bool LoadPluginInfo()
        {
            //try to get plugin info from the JSON file
            var serializedItem = _db.StringGet(nameof(RedisPluginsInfo));
            if (!serializedItem.HasValue)
            {
                base.LoadPluginInfo();
                Save();

                //delete the plugins info file
                var filePath = _fileProvider.MapPath(NopPluginDefaults.PluginsInfoFilePath);
                _fileProvider.DeleteFile(filePath);
                return false;
            }

            return DeserializePluginInfo(serializedItem);
        }

        #endregion
    }
}
