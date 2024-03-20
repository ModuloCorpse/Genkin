using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Genkin
{
    public static class DefaultAvatars
    {
        private static readonly Dictionary<string, BitmapSource> ms_LoadedImage = [];

        private static void LoadAssemblyResources(string resource)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Stream? internalResourceStream = assembly.GetManifestResourceStream(resource);
            if (internalResourceStream != null)
            {
                System.Drawing.Image? img = System.Drawing.Image.FromStream(internalResourceStream);
                if (img != null)
                {
                    BitmapSource newSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(((System.Drawing.Bitmap)img).GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    ms_LoadedImage[resource] = newSource;
                }
            }
        }

        static DefaultAvatars()
        {
            LoadAssemblyResources("Genkin.Assets.elephant.png");
            LoadAssemblyResources("Genkin.Assets.giraffe.png");
            LoadAssemblyResources("Genkin.Assets.hippo.png");
            LoadAssemblyResources("Genkin.Assets.monkey.png");
            LoadAssemblyResources("Genkin.Assets.panda.png");
            LoadAssemblyResources("Genkin.Assets.parrot.png");
            LoadAssemblyResources("Genkin.Assets.penguin.png");
            LoadAssemblyResources("Genkin.Assets.pig.png");
            LoadAssemblyResources("Genkin.Assets.rabbit.png");
            LoadAssemblyResources("Genkin.Assets.snake.png");
        }

        public static bool TryGetAvatar(string avatar, out BitmapSource? source) => ms_LoadedImage.TryGetValue(avatar, out source);
    }
}
