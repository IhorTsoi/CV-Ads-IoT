using CV.Ads_Client.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using FileName = System.String;
using LocalFilePath = System.String;

namespace CV.Ads_Client.Services.Caching
{
    public class FileCachingService : IDisposable
    {
        private readonly int capacity;
        private readonly Dictionary<FileName, FileCacheItem> cacheMap = new Dictionary<FileName, FileCacheItem>();
        private readonly LinkedList<FileCacheItem> lruList = new LinkedList<FileCacheItem>();

        public FileCachingService(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("The capacity can't be less or equal to 0.", nameof(capacity));
            }
            this.capacity = capacity;
        }

        public string Get(FileName fileName)
        {
            if (cacheMap.TryGetValue(fileName, out FileCacheItem cacheItem))
            {
                Logger.Log("cache", "Cache hit!", ConsoleColor.DarkGreen);
                SetMostRecentlyUsed(cacheItem);
                return cacheItem.LocalFilePath;
            }
            Logger.Log("cache", "Cache miss", ConsoleColor.DarkRed);
            return null;
        }

        public void Add(FileName fileName, LocalFilePath localFilePath)
        {
            if (cacheMap.ContainsKey(fileName))
            {
                SetMostRecentlyUsed(cacheMap[fileName]);
                return;
            }

            if (cacheMap.Count >= capacity)
            {
                RemoveFirst();
            }
            var fileCacheItem = new FileCacheItem(fileName, localFilePath);
            lruList.AddLast(fileCacheItem);
            cacheMap.Add(fileName, fileCacheItem);

            Logger.Log("cache", $"Cache updated with ({fileName})", ConsoleColor.DarkYellow);
        }

        private void SetMostRecentlyUsed(FileCacheItem cacheItem)
        {
            lruList.Remove(cacheItem);
            lruList.AddLast(cacheItem);
        }

        private void RemoveFirst()
        {
            FileCacheItem leastRecentlyUsedFileCacheItem = lruList.First.Value;
            File.Delete(leastRecentlyUsedFileCacheItem.LocalFilePath);
            lruList.RemoveFirst();
            cacheMap.Remove(leastRecentlyUsedFileCacheItem.FileName);
        }

        public void Dispose()
        {
            while (lruList.Count > 0)
            {
                RemoveFirst();
            }
        }

        private class FileCacheItem
        {
            public FileName FileName { get; set; }
            public LocalFilePath LocalFilePath { get; set; }

            public FileCacheItem(string fileName, string localFilePath)
            {
                FileName = fileName;
                LocalFilePath = localFilePath;
            }
        }
    }
}
