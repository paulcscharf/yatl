using System.Collections.Generic;
using System;

namespace yatl.Utilities
{
    public class DeletableObjectList<T> : IEnumerable<T>
        where T : DeletableObject<T>
    {
        private LinkedList<DeletableObjectEnumerator<T>> enums = new LinkedList<DeletableObjectEnumerator<T>>();

        public T First { get; private set; }
        public T Last { get; private set; }

        public int Count { get; private set; }

        public DeletableObjectList()
        {
            this.Count = 0;
        }

        public void Add(T obj)
        {
            if (obj.ChangingList)
            {
                this.Last = obj;
                if (this.Count == 0)
                    this.First = obj;

                this.Count++;
            }
            else if (obj.List == null)
                obj.AddToList(this);
        }

        public void Remove(T obj)
        {
            if (obj.ChangingList)
            {
                foreach (DeletableObjectEnumerator<T> e in this.enums)
                    e.OnObjectRemove(obj);

                if (obj == this.Last)
                    this.Last = obj.Prev;
                if (obj == this.First)
                    this.First = obj.Next;

                this.Count--;
            }
            else if (obj.List == this)
                obj.RemoveFromList();
        }

        public IEnumerator<T> GetEnumerator()
        {
            DeletableObjectEnumerator<T> e = new DeletableObjectEnumerator<T>(this);
            this.enums.AddFirst(e);
            e.SetNode(this.enums.First);
            //LinkedListNode<GameObjectEnumerator<T>> node = new LinkedListNode<GameObjectEnumerator<T>>(e);
            //e.SetNode(node);
            //this.enums.AddFirst(node);
            return e;
        }

        public void ForgetEnumerator(LinkedListNode<DeletableObjectEnumerator<T>> node)
        {
            this.enums.Remove(node);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class DeletableObjectEnumerator<T> : IEnumerator<T>
        where T : DeletableObject<T>
    {

        private DeletableObjectList<T> list;

        private bool initialised = false;

        private bool currentWasDeleted = false;

        private bool done = false;

        public DeletableObjectEnumerator(DeletableObjectList<T> list)
        {
            this.list = list;
        }

        private LinkedListNode<DeletableObjectEnumerator<T>> node;
        public void SetNode(LinkedListNode<DeletableObjectEnumerator<T>> node)
        {
            if (this.node == null)
                this.node = node;
        }

        public T Current
        {
            get { return this.current; }
        }

        private T current;

        public void OnObjectRemove(T obj)
        {
            if (obj != this.current)
                return;

            this.currentWasDeleted = true;
            this.current = obj.Next;
            if (this.current == null)
                this.done = true;
        }

        public void Dispose()
        {
            this.list.ForgetEnumerator(this.node);
        }

        public bool MoveNext()
        {
            if (this.done)
                return false;
            if (!this.initialised)
            {
                if (this.list.Count == 0)
                    return false;
                this.initialised = true;
                this.current = list.First;
            }
            else
            {
                if (this.currentWasDeleted)
                {
                    this.currentWasDeleted = false;
                    return true;
                }
                this.current = this.current.Next;
                if (this.current == null)
                {
                    this.done = true;
                    return false;
                }
            }
            return true;
        }

        public void Reset()
        {
            this.current = null;
            this.done = false;
            this.initialised = false;
            this.currentWasDeleted = false;
        }

        object System.Collections.IEnumerator.Current
        {
            get { return this.Current; }
        }

    }

}
