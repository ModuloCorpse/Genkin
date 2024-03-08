using CorpseLib.Serialize;
using CorpseLib;

namespace Ginko
{
    public class Holding(string name, string description, decimal amount, Guid id) : FinancialItem(name, description, amount, id)
    {
        public class BytesSerializer : BytesSerializer<Holding>
        {
            protected override OperationResult<Holding> DeserializeItem(BytesReader reader, Guid id, decimal amount, string name, string description) => new(new(name, description, amount, id));
            protected override void SerializeItem(Holding obj, BytesWriter writer) { }
        }

        public override string ToString() => string.Format("[Holding: {0}]", base.ToString());
    }
}
