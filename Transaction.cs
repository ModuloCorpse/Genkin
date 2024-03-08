using CorpseLib;
using CorpseLib.Serialize;

namespace Ginko
{
    public class Transaction(string name, string description, DateTime date, decimal amount, Guid id) : FinancialItem(name, description, amount, id), ITimelineElement
    {
        public class BytesSerializer : BytesSerializer<Transaction>
        {
            protected override OperationResult<Transaction> DeserializeItem(BytesReader reader, Guid id, decimal amount, string name, string description) => new(new(name, description, reader.ReadDateTime(), amount, id));
            protected override void SerializeItem(Transaction obj, BytesWriter writer) => writer.Write(obj.m_Date);
        }

        private readonly DateTime m_Date = date;
        public DateTime Date => m_Date;

        public override string ToString() => string.Format("[Transaction: {0}, Date = {1}]", base.ToString(), m_Date);
    }
}
