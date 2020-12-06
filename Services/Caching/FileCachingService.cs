using System;
using System.Collections.Generic;
using System.IO;
using FileName = System.String;
using LocalFilePath = System.String;

namespace CV.Ads_Client.Services.Caching
{
    public class FileCachingService
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
                LocalFilePath localFilePath = cacheItem.LocalFilePath;
                lruList.Remove(cacheItem);
                lruList.AddLast(cacheItem);
                return localFilePath;
            }
            return null;
        }

        public void Add(FileName fileName, LocalFilePath localFilePath)
        {
            if (Get(fileName) != null)
            {
                return;
            }

            if (cacheMap.Count >= capacity)
            {
                RemoveFirst();
            }

            var fileCacheItem = new FileCacheItem(fileName, localFilePath);
            lruList.AddLast(fileCacheItem);
            cacheMap.Add(fileName, fileCacheItem);
        }

        private void RemoveFirst()
        {
            FileCacheItem leastRecentlyUsedFileCacheItem = lruList.First.Value;
            File.Delete(leastRecentlyUsedFileCacheItem.LocalFilePath);
            lruList.RemoveFirst();
            cacheMap.Remove(leastRecentlyUsedFileCacheItem.FileName);
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
