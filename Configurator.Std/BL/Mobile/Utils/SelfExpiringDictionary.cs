using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Configurator.Std.BL.Mobile.Utils
{
   public class SelfExpiringDictionary<K, V> : IDisposable
   {
      class Item
      {
         public DateTime Ts { get; set; }
         public V Data { get; set; }
      }

      private readonly ConcurrentDictionary<K, Item> mmapStorage = new ConcurrentDictionary<K, Item>(Environment.ProcessorCount, 2);

      private readonly System.Timers.Timer mobjTimer;
      private readonly int mintEvictionWindowMs;

      public delegate void EvictedEventHandler(K k, V v);
      public event EvictedEventHandler Evict;

      public SelfExpiringDictionary(int evictionWindowMs, int intervalMs)
      {
         mintEvictionWindowMs = evictionWindowMs;
         mobjTimer = new System.Timers.Timer(intervalMs);
         mobjTimer.AutoReset = true;
         mobjTimer.Elapsed += (sender, args) =>
         {
            var now = DateTime.UtcNow;
            var toClean = mmapStorage.Where(pair => (long)(now - pair.Value.Ts).TotalMilliseconds > mintEvictionWindowMs).Select(pair => pair.Key).ToList();
            foreach (var key in toClean)
            {
               if (Remove(key, out V value))
               {
                  Evict?.Invoke(key, value);
               }
            }
         };
      }

      public SelfExpiringDictionary(int evictionWindowMs) : this(evictionWindowMs, 1000) { }

      public void Add(K key) => Add(key, default(V));

      public bool Contains(K key) => mmapStorage.ContainsKey(key);

      public bool Remove(K key) => Remove(key, out _);

      public int Size() => mmapStorage.Count;

      public void Add(K key, V value)
      {
         var now = DateTime.UtcNow;
         mmapStorage.AddOrUpdate(key, new Item
         {
            Ts = now,
            Data = value
         }, (k, v) =>
         {
            v.Ts = now;
            v.Data = value;
            return v;
         });

         if (!mobjTimer.Enabled)
         {
            mobjTimer.Start();
         }
      }

      public V Get(K key)
      {
         try
         {
            return mmapStorage.First(x => x.Key.Equals(key)).Value.Data;
         }
         catch (Exception)
         {
            return default(V);
         }
      }

      public IEnumerable<K> WithValue(V v)
      {
         return mmapStorage.Where(pair => pair.Value.Data.Equals(v)).Select(pair => pair.Key).ToList();
      }

      public long GetAge(K key)
      {
         try
         {
            return (long)(DateTime.UtcNow - (mmapStorage.First(x => x.Key.Equals(key)).Value.Ts)).TotalMilliseconds;
         }
         catch (Exception)
         {
            return -1;
         }
      }

      public bool Remove(K key, out V value)
      {
         bool result = mmapStorage.TryRemove(key, out Item objItem);
         if (result)
         {
            value = objItem.Data;
         }
         else
         {
            value = default(V);
         }
         if (mmapStorage.Count == 0)
         {
            mobjTimer.Stop();
         }

         return result;
      }

      public void Dispose()
      {
         if (mobjTimer.Enabled)
         {
            mobjTimer.Stop();
         }
         mobjTimer.Dispose();
      }
   }
}
