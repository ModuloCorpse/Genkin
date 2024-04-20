using CorpseLib;
using CorpseLib.DataNotation;
using Genkin.Core;
using System.IO;
using System.Windows;
using DataObject = CorpseLib.DataNotation.DataObject;

namespace Genkin
{
    public class UserInfo(string name, string path, Rect rect, Guid id)
    {
        public class DataSerializer : ADataSerializer<UserInfo>
        {
            protected override OperationResult<UserInfo> Deserialize(DataObject reader)
            {
                if (reader.TryGet("name", out string? name) && name != null &&
                    reader.TryGet("pp_path", out string? profilePicturePath) && profilePicturePath != null &&
                    reader.TryGet("pp_x", out int? profilePictureX) && profilePictureX != null &&
                    reader.TryGet("pp_y", out int? profilePictureY) && profilePictureY != null &&
                    reader.TryGet("pp_width", out int? profilePictureWidth) && profilePictureWidth != null &&
                    reader.TryGet("pp_height", out int? profilePictureHeight) && profilePictureHeight != null &&
                    reader.TryGet("id", out Guid? id) && id != null)
                    return new(new(name, profilePicturePath, new((double)profilePictureX, (double)profilePictureY, (double)profilePictureWidth, (double)profilePictureHeight), (Guid)id));
                return new("Deserialization error", "Bad json");
            }

            protected override void Serialize(UserInfo obj, DataObject writer)
            {
                writer["name"] = obj.m_Name;
                writer["pp_path"] = obj.m_ProfilePicturePath;
                writer["pp_x"] = obj.m_ProfilepictureRect.X;
                writer["pp_y"] = obj.m_ProfilepictureRect.Y;
                writer["pp_width"] = obj.m_ProfilepictureRect.Width;
                writer["pp_height"] = obj.m_ProfilepictureRect.Height;
                writer["id"] = obj.m_ID;
            }
        }

        private readonly string m_Name = name;
        private readonly string m_ProfilePicturePath = path;
        private readonly Rect m_ProfilepictureRect = rect;
        private readonly Guid m_ID = id;

        public string Name => m_Name;
        public string ProfilePicturePath => m_ProfilePicturePath;
        public Rect ProfilePictureRect => m_ProfilepictureRect;
        public Guid ID => m_ID;

        public UserInfo() : this(string.Empty, string.Empty, new(0, 0, 85, 85), Guid.NewGuid()) { }

        public User GetUser(string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            string path = Path.GetFullPath(Path.Combine(directory, m_ID.ToString()));
            return new User(m_ID, path);
        }
    }
}
