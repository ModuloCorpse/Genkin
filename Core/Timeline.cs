using System.Collections;

namespace Genkin.Core
{
    public class Timeline<T> : IEnumerable<T> where T : ITimelineElement
    {
        private class TimelineElement
        {
            private readonly T? m_Element;
            private readonly DateTime m_Date;

            public T Element => m_Element!;
            public DateTime Date => m_Date;

            public TimelineElement(T element)
            {
                m_Element = element;
                m_Date = element.Date;
            }

            public TimelineElement(DateTime date)
            {
                m_Element = default;
                m_Date = date;
            }

            public static implicit operator T(TimelineElement element) => element.m_Element!;
        }

        private class TimelineEnumerator(TimelineElement[] list) : IEnumerator<T>
        {
            private readonly TimelineElement[] m_List = list;
            private int m_Position = -1;

            public bool MoveNext()
            {
                m_Position++;
                return m_Position < m_List.Length;
            }

            public void Reset() => m_Position = -1;

            public void Dispose() { }

            object IEnumerator.Current { get => Current; }

            public T Current
            {
                get
                {
                    try
                    {
                        return m_List[m_Position].Element;
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        private readonly List<TimelineElement> m_Elements = [];

        public T this[int idx] { get => m_Elements[idx].Element; }
        public int Count => m_Elements.Count;

        public void Add(T element)
        {
            m_Elements.Add(new(element));
            m_Elements.Sort((t1, t2) => t1.Date.CompareTo(t2.Date));
        }

        public void RemoveAt(int idx) => m_Elements.RemoveAt(idx);

        public int FindFirstElementAfterOrOnDate(DateTime date)
        {
            if (m_Elements.Count == 0)
                return 0;
            if (m_Elements[0].Date > date)
                return 0;
            if (m_Elements[^1].Date < date)
                return m_Elements.Count;
            int index = m_Elements.BinarySearch(new(date), Comparer<TimelineElement>.Create((t1, t2) => t1.Date.CompareTo(t2.Date)));
            while (index > 0 && m_Elements[index - 1].Date == date)
                index--;
            return index;
        }

        public IEnumerator<T> GetEnumerator() => new TimelineEnumerator([.. m_Elements]);
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
