using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Genkin
{
    public partial class UserScrollItem : UserControl
    {
        public event Action? UserClicked;

        public UserScrollItem()
        {
            InitializeComponent();
            MouseEnter += UserScrollItem_MouseEnter;
            MouseLeave += UserScrollItem_MouseLeave;
            MouseLeftButtonDown += UserScrollItem_MouseLeftButtonDown;
        }

        private void UserScrollItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            BrushConverter converter = new();
            Arrow.Foreground = (SolidColorBrush)converter.ConvertFrom("#3B556D")!;
            Background = (SolidColorBrush)converter.ConvertFrom("#1C2942")!;
            UserClicked?.Invoke();
        }

        private void UserScrollItem_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BrushConverter converter = new();
            Arrow.Foreground = (SolidColorBrush)converter.ConvertFrom("#3B556D")!;
            Background = (SolidColorBrush)converter.ConvertFrom("#0B162C")!;
        }

        private void UserScrollItem_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            BrushConverter converter = new();
            Arrow.Foreground = (SolidColorBrush)converter.ConvertFrom("#0B162C")!;
            Background = (SolidColorBrush)converter.ConvertFrom("#3B556D")!;
        }

        public UserScrollItem(UserInfo userInfo) : this()
        {
            ProfileName.Content = userInfo.Name;
            SetProfilePicture(userInfo);
        }

        public void SetProfilePicture(UserInfo userInfo)
        {
            if (File.Exists(userInfo.ProfilePicturePath))
            {
                ProfileImage.Source = new BitmapImage(new Uri(userInfo.ProfilePicturePath, UriKind.Relative));
                Rect rect = userInfo.ProfilePictureRect;
                ProfileImage.RenderTransform = new TranslateTransform(-rect.X, -rect.Y);
                ProfileImage.Clip = new RectangleGeometry { Rect = new(0, 0, rect.X + rect.Width, rect.Y + rect.Height) };
            }
            else
            {
                string? avatarSource = userInfo.ProfilePicturePath switch
                {
                    "elephant" or "giraffe" or "hippo" or "monkey" or
                    "panda" or "parrot" or "penguin" or "pig" or "rabbit" or
                    "snake" => string.Format("Genkin.Assets.{0}.png", userInfo.ProfilePicturePath),
                    _ => null
                };
                if (avatarSource != null && DefaultAvatars.TryGetAvatar(avatarSource, out BitmapSource? source))
                {
                    ProfileImage.Source = source;
                    ProfileImage.Stretch = Stretch.UniformToFill;
                    ProfileImage.RenderTransform = new TranslateTransform(0, 0);
                    ProfileImage.Clip = new RectangleGeometry { Rect = new(0, 0, 85, 85) };
                }
            }
        }
    }
}
