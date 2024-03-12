namespace Genkin.Core
{
    public class Filter
    {
        public List<uint> Tags = [];
        public DateTime StartDate = DateTime.MinValue; //included
        public DateTime EndDate = DateTime.MaxValue; //included
        public HashSet<Guid> Account = [];
        public HashSet<Guid> Saving = [];

        public bool Match(Transaction transaction)
        {
            if (transaction.Date < StartDate || transaction.Date > EndDate)
                return false;
            foreach (uint tag in Tags)
            {
                if (!transaction.HaveTag(tag))
                    return false;
            }
            return true;
        }
    }
}
