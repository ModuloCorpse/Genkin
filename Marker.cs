namespace Ginko
{
    public class Marker(decimal amount, DateTime date) : ITimelineElement
    {
        private readonly DateTime m_Date = date;
        private decimal m_Amount = amount;

        public DateTime Date => m_Date;
        public decimal Amount => m_Amount;

        public void UpdateAmount(decimal amount) => m_Amount += amount;

        public override string ToString() => string.Format("[Marker: Amount = {0}, Date = {1}]", m_Amount, m_Date);

    }
}
