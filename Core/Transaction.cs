using CorpseLib;
using CorpseLib.Serialize;

namespace Genkin.Core
{
    public class Transaction(string name, string description, DateTime date, decimal amount, Guid id) : ITimelineElement
    {
        public class BytesSerializer : ABytesSerializer<Transaction>
        {
            protected override OperationResult<Transaction> Deserialize(ABytesReader reader)
            {
                Guid id = reader.Read<Guid>();
                decimal amount = reader.Read<decimal>();
                string name = reader.Read<string>();
                string description = reader.Read<string>();
                DateTime date = reader.Read<DateTime>();
                OperationResult<List<uint>> tagsResult = reader.ReadList<uint>();
                if (!tagsResult)
                    return new(tagsResult.Error, tagsResult.Description);
                List<uint> tags = tagsResult.Result!;
                Transaction transaction = new(name, description, date, amount, id);
                foreach (uint tag in tags)
                    transaction.AddTag(tag);
                return new(transaction);
            }

            protected override void Serialize(Transaction obj, ABytesWriter writer)
            {
                writer.Write(obj.m_ID);
                writer.Write(obj.m_Amount.Value);
                writer.Write(obj.m_Name);
                writer.Write(obj.m_Description);
                writer.Write(obj.m_Date);
                writer.WriteList(obj.m_Tags);
            }
        }

        private readonly List<uint> m_Tags = [];
        protected Amount m_Amount = new(amount);
        protected readonly Guid m_ID = id;
        private readonly DateTime m_Date = date;
        protected readonly string m_Name = name;
        protected readonly string m_Description = description;

        public DateTime Date => m_Date;
        public uint[] Tags => [..m_Tags];
        public Guid ID => m_ID;
        public string Name => m_Name;
        public string Description => m_Description;
        public string Currency => m_Amount.Currency;
        public decimal Amount => m_Amount.Value;

        public bool HaveTag(uint tag) => m_Tags.Contains(tag);
        public void AddTag(uint tag) => m_Tags.Add(tag);
        internal void SetCurrency(string currency) => m_Amount = new(currency, m_Amount.Value);

        public override string ToString() => $"[Transaction: ID = {m_ID}, Amount = {m_Amount}, Name = \"{m_Name}\", Description = \"{m_Description}\", Date = {m_Date}]";
    }
}