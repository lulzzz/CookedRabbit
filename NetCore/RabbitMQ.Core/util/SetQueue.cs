using System;
using System.Collections.Generic;

namespace RabbitMQ.Util
{
    public class SetQueue<T>
    {
        private HashSet<T> members = new HashSet<T>();
        private LinkedList<T> queue = new LinkedList<T>();

        public bool Enqueue(T item)
        {
            if(this.members.Contains(item))
            {
                return false;
            }
            this.members.Add(item);
            this.queue.AddLast(item);
            return true;
        }

        public T Dequeue()
        {
            if (this.queue.Count == 0)
            {
                return default(T);
            }
            T item = this.queue.First.Value;
            this.queue.RemoveFirst();
            this.members.Remove(item);
            return item;
        }

        public bool Contains(T item)
        {
            return this.members.Contains(item);
        }

        public bool IsEmpty()
        {
            return this.members.Count == 0;
        }

        public bool Remove(T item)
        {
            this.queue.Remove(item);
            return this.members.Remove(item);
        }

        public void Clear()
        {
            this.queue.Clear();
            this.members.Clear();
        }
    }
}
