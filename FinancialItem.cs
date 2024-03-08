using CorpseLib.Serialize;
using CorpseLib;

namespace Ginko
{
    public abstract class FinancialItem(string name, string description, decimal amount, Guid id)
    {
        public abstract class BytesSerializer<T> : ABytesSerializer<T> where T : FinancialItem
        {
            protected override OperationResult<T> Deserialize(BytesReader reader)
            {
                Guid id = reader.ReadGuid();
                decimal amount = reader.ReadDecimal();
                string name = reader.ReadString();
                string description = reader.ReadString();
                return DeserializeItem(reader, id, amount, name, description);
            }

            protected abstract OperationResult<T> DeserializeItem(BytesReader reader, Guid id, decimal amount, string name, string description);

            protected override void Serialize(T obj, BytesWriter writer)
            {
                writer.Write(obj.m_ID);
                writer.Write(obj.m_Amount);
                writer.WriteWithLength(obj.m_Name);
                writer.WriteWithLength(obj.m_Description);
                SerializeItem(obj, writer);
            }
            protected abstract void SerializeItem(T obj, BytesWriter writer);
        }

        protected readonly Guid m_ID = id;
        protected readonly string m_Name = name;
        protected readonly string m_Description = description;
        protected readonly decimal m_Amount = amount;

        public Guid ID => m_ID;
        public string Name => m_Name;
        public string Description => m_Description;
        public decimal Amount => m_Amount;

        public override string ToString() => string.Format("ID = {0}, Amount = {1}, Name = \"{2}\", Description = \"{3}\"", m_ID, m_Amount, m_Name, m_Description);
    }
}
