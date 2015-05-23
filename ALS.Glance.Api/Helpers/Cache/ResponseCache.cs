using System;
using System.Net.Http;
using System.Web;
using System.Web.Caching;
using ALS.Glance.Api.Properties;

namespace ALS.Glance.Api.Helpers.Cache
{
    public class ResponseCache : BaseCache
    {
        private readonly bool _absoluteExpiration;
        private readonly TimeSpan _expireSpan;

        public ResponseCache()
        {
        }

        public ResponseCache(bool absoluteExpiration, DefaultCacheTime defaultCacheTime)
        {
            _absoluteExpiration = absoluteExpiration;
            _expireSpan = GetDefaultTimeSpan(defaultCacheTime);
        }

        public ResponseCache(bool absoluteExpiration, int minutes)
        {
            _absoluteExpiration = absoluteExpiration;
            _expireSpan = new TimeSpan(0, minutes, 0);
        }


        public object GetValue(HttpRequestMessage requestMessage)
        {
            if (!Settings.Default.ResponseCacheEnabled)
            {
                return null;
            }

            var key = HttpUtility.UrlDecode(requestMessage.RequestUri.AbsoluteUri);
            return key == null ? null : Cache.Get(key);
        }

        public void SetValue(HttpRequestMessage requestMessage, object value)
        {
            if (Settings.Default.ResponseCacheEnabled)
            {
                var key = HttpUtility.UrlDecode(requestMessage.RequestUri.AbsoluteUri);
                if (key == null)
                {
                    return;
                }

                Cache.Add(
                    key,
                    value,
                    null,
                    _absoluteExpiration ? DateTime.Now.Add(_expireSpan) : System.Web.Caching.Cache.NoAbsoluteExpiration,
                    _absoluteExpiration ? System.Web.Caching.Cache.NoSlidingExpiration : _expireSpan,
                    CacheItemPriority.Default, null);
            }
        }

        public void Remove(string key)
        {
            if (Settings.Default.ResponseCacheEnabled)
            {
                Cache.Remove(key);
            }
        }

        private static TimeSpan GetDefaultTimeSpan(DefaultCacheTime defaultCacheTime)
        {
            switch (defaultCacheTime)
            {
                case DefaultCacheTime.Short:
                    return new TimeSpan(0, Settings.Default.ResponseCacheDefaultShortTimeInMinutes, 0);
                case DefaultCacheTime.Long:
                    return new TimeSpan(0, Settings.Default.ResponseCacheDefaultLongTimeInMinutes, 0);
                default:
                    return new TimeSpan(0, Settings.Default.ResponseCacheDefaultShortTimeInMinutes, 0);
            }
        }
    }
}