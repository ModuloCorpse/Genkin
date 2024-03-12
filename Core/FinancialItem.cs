using CorpseLib.Serialize;
using CorpseLib;

namespace Genkin.Core
{
    public abstract class FinancialItem(string name, string description, decimal amount, Guid id)
    {
        public abstract class BytesSerializer<T> : ABytesSerializer<T> where T : FinancialItem
        {
            protected override OperationResult<T> Deserialize(ABytesReader reader)
            {
                Guid id = reader.Read<Guid>();
                decimal amount = reader.Read<decimal>();
                string name = reader.Read<string>();
                string description = reader.Read<string>();
                return DeserializeItem(reader, id, amount, name, description);
            }

            protected abstract OperationResult<T> DeserializeItem(ABytesReader reader, Guid id, decimal amount, string name, string description);

            protected override void Serialize(T obj, ABytesWriter writer)
            {
                writer.Write(obj.m_ID);
                writer.Write(obj.m_Amount.Value);
                writer.Write(obj.m_Name);
                writer.Write(obj.m_Description);
                SerializeItem(obj, writer);
            }
            protected abstract void SerializeItem(T obj, ABytesWriter writer);
        }

        protected Amount m_Amount = new(amount);
        protected readonly Guid m_ID = id;
        protected readonly string m_Name = name;
        protected readonly string m_Description = description;

        public Guid ID => m_ID;
        public string Name => m_Name;
        public string Description => m_Description;
        public string Currency => m_Amount.Currency;
        public decimal Amount => m_Amount.Value;

        internal void SetCurrency(string currency) => m_Amount = new(currency, m_Amount.Value);

        public override string ToString() => $"ID = {m_ID}, Amount = {m_Amount}, Name = \"{m_Name}\", Description = \"{m_Description}\"";
    }
}
