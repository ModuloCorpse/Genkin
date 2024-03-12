namespace Genkin.Core
{
    public class Amount(decimal value)
    {
        private readonly string m_Currency = string.Empty;
        private readonly decimal m_Value = value;

        public string Currency => m_Currency;
        public decimal Value => m_Value;

        public Amount(string currency, decimal value) : this(value) => m_Currency = currency;

        public override string ToString() => $"{m_Value}{m_Currency}";
    }
}
