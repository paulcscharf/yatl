namespace yatl.Utilities
{
    public abstract class DeletableObject<T> where T : DeletableObject<T>
    {
        public T Next { get; private set; }
        public T Prev { get; private set; }

        public DeletableObjectList<T> List { get; private set; }

        public bool ChangingList { get; private set; }

        public bool Deleted { get; private set; }

        protected DeletableObject()
        {
            this.ChangingList = false;
            this.Deleted = false;
        }

        public void Delete()
        {
            if (this.Deleted)
                return;

            this.onDelete();
            this.RemoveFromList();
            this.Deleted = true;
        }

        protected virtual void onDelete()
        {
        }

        public void AddToList(DeletableObjectList<T> list)
        {
            if (this.List != null)
                return;

            this.List = list;
            this.Deleted = false;

            if (list.Last != null)
            {
                this.Prev = list.Last;
                this.Prev.Next = (T)this;
            }

            this.ChangingList = true;
            list.Add((T)this);
            this.ChangingList = false;
        }

        public void RemoveFromList()
        {
            if (this.List == null)
                return;

            this.ChangingList = true;
            this.List.Remove((T)this);
            this.ChangingList = false;

            if (this.Next != null)
            {
                this.Next.Prev = this.Prev;
            }
            if (this.Prev != null)
            {
                this.Prev.Next = this.Next;
            }
            this.Next = null;
            this.Prev = null;
            this.List = null;
        }

    }

}
