using CorpseLib;
using CorpseLib.Encryption;
using CorpseLib.Serialize;
using System.Collections;
using System.IO;
using System.Text;

namespace Genkin.Core
{
    public class User : IEnumerable<Account>
    {
        private readonly List<Account> m_Accounts = [];
        private readonly string m_Name;
        private readonly string m_Path;
        private string m_LoadPassword = string.Empty;
        private string m_SavePassword = string.Empty;

        public Account this[int idx] { get => m_Accounts[idx]; }

        public static User? NewUser(string name, string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            Guid guid = Guid.NewGuid();
            string path = Path.GetFullPath(Path.Combine(directory, guid.ToString()));
            return new User(name, path);
        }

        public static List<User> LoadUsers(string directory)
        {
            if (!Directory.Exists(directory))
                return [];
            List<User> users = [];
            BytesSerializer serializer = NewSerializer();
            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
            {
                try
                {
                    string absolute = Path.GetFullPath(file);
                    FileBytesReader reader = new(serializer, absolute);
                    string userName = reader.Read<string>();
                    reader.Close();
                    users.Add(new(userName, absolute));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return users;
        }

        private static BytesSerializer NewSerializer()
        {
            BytesSerializer serializer = new();
            serializer.Register(new Transaction.BytesSerializer());
            serializer.Register(new Saving.BytesSerializer());
            serializer.Register(new Account.BytesSerializer());
            return serializer;
        }

        private User(string name, string path)
        {
            m_Name = name;
            m_Path = path;
        }

        public void Load()
        {
            BytesSerializer serializer = NewSerializer();
            BytesReader reader = new(serializer, File.ReadAllBytes(m_Path));
            if (reader.Read<string>() == m_Name)
            {
                byte[] decrypted = new AesEncryptor(m_LoadPassword).Decrypt(reader.ReadAll());
                reader = new(serializer, decrypted);
                int accountCount = reader.Read<int>();
                for (int i = 0; i != accountCount; i++)
                {
                    OperationResult<Account> account = reader.SafeRead<Account>();
                    if (account && account.Result != null)
                        m_Accounts.Add(account.Result);
                }
            }
        }

        public void Save()
        {
            BytesSerializer serializer = NewSerializer();
            BytesWriter writer = new(serializer);
            writer.Write(m_Name);
            BytesWriter accountWriter = new(serializer);
            accountWriter.Write(m_Accounts.Count);
            foreach (Account account in m_Accounts)
                accountWriter.Write(account);
            byte[] encryptedBytes = new AesEncryptor(m_SavePassword).Encrypt(accountWriter.Bytes);
            writer.Write(encryptedBytes);
            File.WriteAllBytes(m_Path, writer.Bytes);
        }

        public void SetLoadPassword(string password) => m_LoadPassword = password;
        public void SetSavePassword(string password) => m_SavePassword = password;

        public Account NewAccount(string name, string description, string currency)
        {
            Account account = new(name, description, currency, Guid.NewGuid());
            m_Accounts.Add(account);
            return account;
        }

        public List<Transaction> GetTransactions(Filter filter)
        {
            List<Transaction> result = [];
            foreach (Account account in m_Accounts)
            {
                if (filter.Account.Count == 0 || filter.Account.Contains(account.ID))
                    result.AddRange(account.GetTransactions(filter));
            }
            return result;
        }

        public override string ToString()
        {
            StringBuilder builder = new("[User: Name = ");
            builder.Append(m_Name);
            builder.Append(", Path = ");
            builder.Append(m_Path);
            builder.Append(", Accounts = [");
            if (m_Accounts.Count > 0)
                builder.AppendLine();
            foreach (Account account in m_Accounts)
                builder.AppendLine(account.ToString(1));
            builder.Append("]]");
            return builder.ToString();
        }

        public IEnumerator<Account> GetEnumerator() => ((IEnumerable<Account>)m_Accounts).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)m_Accounts).GetEnumerator();
    }
}
