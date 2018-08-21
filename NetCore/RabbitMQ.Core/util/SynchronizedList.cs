using System;
using System.Collections;
using System.Collections.Generic;

namespace RabbitMQ.Util
{
    internal class SynchronizedList<T> : IList<T>
    {
        private readonly IList<T> list;
        private readonly object root;

        internal SynchronizedList()
            : this(new List<T>())
        {
        }

        internal SynchronizedList(IList<T> list)
        {
            this.list = list;
            root = new Object();
        }

        public int Count
        {
            get
            {
                lock (root)
                    return list.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return list.IsReadOnly; }
        }

        public T this[int index]
        {
            get
            {
                lock (root)
                    return list[index];
            }
            set
            {
                lock (root)
                    list[index] = value;
            }
        }

        public object SyncRoot
        {
            get { return root; }
        }

        public void Add(T item)
        {
            lock (root)
                list.Add(item);
        }

        public void Clear()
        {
            lock (root)
                list.Clear();
        }

        public bool Contains(T item)
        {
            lock (root)
                return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (root)
                list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            lock (root)
                return list.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (root)
                return list.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            lock (root)
                return list.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            lock (root)
                return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            lock (root)
                list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            lock (root)
                list.RemoveAt(index);
        }
    }
}
