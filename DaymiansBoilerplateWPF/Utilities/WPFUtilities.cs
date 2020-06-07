using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using CommonServiceLocator;
using DaymsWPFBoiler.Data.Utilities;
using DaymsWPFBoiler.Resources;
using DaymsWPFBoiler.ViewModels;

namespace DaymiansBoilerplateWPF.Utilities
{
    public class WPFUtilities
    {
        /// <summary>
        /// DateTime.Now. See remarks for more information.
        /// </summary>
        /// <remarks>
        ///     Choosing to set the default start date as the current system time, 
        ///     rather than the start of the day due to ops being done on the road, etc. -- Daymian 12/31/19
        /// </remarks>
        public static DateTime DEFAULT_START_DATE => DateTime.Now/*.Date*/;
        public static DateTime DEFAULT_END_DATE => DateTime.Now.Date.AddDays(1).AddTicks(-1);
        
        public static GridLength HEIGHT_COLLAPSED { get; } = new GridLength(0);
        public static GridLength HEIGHT_TEXT { get; } = new GridLength(30);
        public static GridLength HEIGHT_TEXT_MULTILINE { get; } = new GridLength(60);
        public static GridLength HEIGHT_COMBOBOX { get; } = new GridLength(35);

        public static string ImagesFilterString { get; } = CreateImagesFilterString();

        public static bool ConfirmClose(bool isDirty)
        {
            if (isDirty)
            {
                // Open Confirm Dialog for abandoning changes
                MessageBoxViewModel mbox = ServiceLocator.Current.GetInstance<MessageBoxViewModel>(DataFunctions.ToSQLiteDT(DateTime.Now));
                mbox.Caption = "Confirm Close Caption";
                mbox.Message = "Confirm Close Message?";
                mbox.Buttons = MessageBoxButton.YesNo;
                mbox.Image = MessageBoxImage.Question;
                return MessageBoxResult.Yes == mbox.Show(ServiceLocator.Current.GetInstance<MainViewModel>().Dialogs);
            }
            return true;
        }

        public static bool ConfirmDelete(string itemNameInCaption, string itemNameInMessage)
        {
            // Open Confirm Dialog for Delete
            MessageBoxViewModel mbox = ServiceLocator.Current.GetInstance<MessageBoxViewModel>(DataFunctions.ToSQLiteDT(DateTime.Now));
            mbox.Caption = "Delete " + itemNameInCaption;
            mbox.Message = "Are you sure you want to delete this " + itemNameInMessage + "?";
            mbox.Buttons = MessageBoxButton.YesNo;
            mbox.Image = MessageBoxImage.Question;
            return MessageBoxResult.Yes == mbox.Show(ServiceLocator.Current.GetInstance<MainViewModel>().Dialogs) ? true : false;
        }

        public static void ShowDeleteError(string itemNameInMessage)
        {
            MessageBoxViewModel mbox2 = ServiceLocator.Current.GetInstance<MessageBoxViewModel>(DataFunctions.ToSQLiteDT(DateTime.Now));
            mbox2.Caption = "Database Error";
            mbox2.Message = "Error deleting " + itemNameInMessage + " from database!";
            mbox2.Buttons = MessageBoxButton.OK;
            mbox2.Image = MessageBoxImage.Error;
            mbox2.Show(ServiceLocator.Current.GetInstance<MainViewModel>().Dialogs);
        }

        private static string CreateImagesFilterString()
        {
            string filter = string.Empty;

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string sep = string.Empty;
            string allExtensions = string.Empty;

            foreach (ImageCodecInfo c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                filter = string.Format("{0}{1}{2} ({3})|{3}", filter, sep, codecName, c.FilenameExtension);
                allExtensions += c.FilenameExtension + ";";
                sep = "|";
            }

            filter = string.Format("{0}{1}{2} ({3})|{3}", filter, sep, "All Files", "*.*");
            filter = string.Format("{2} ({3})|{3}{1}{0}", filter, sep, "All Image Files", allExtensions.Substring(0, allExtensions.Length - 1));

            return filter;
        }

        /// <summary>
        /// Converts <see cref="BitmapImage"/> to <see cref="Bitmap"/>
        /// </summary>
        /// <param name="bmapImage"></param>
        /// <returns></returns>
        public static Bitmap BitmapImageToBitmap(BitmapImage bmapImage)
        {
            BitmapEncoder enc = new BmpBitmapEncoder();
            Bitmap bmp = null;
            using (MemoryStream outStream = new MemoryStream())
            {
                enc.Frames.Add(BitmapFrame.Create(bmapImage));
                enc.Save(outStream);
                bmp = new Bitmap(outStream);
            }
            return new Bitmap(bmp);
        }

        public static byte[] BitmapToBytes(Bitmap bitmap)
        {
            return BitmapToBytes(bitmap, ImageFormat.Jpeg);
        }

        public static byte[] BitmapToBytes(Bitmap bitmap, ImageFormat imageFormat)
        {
            if (null != bitmap)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, imageFormat);
                    return stream.ToArray();
                }
            }
            return null;
        }

        public static async Task<BitmapSource> BytesToBitmapSourceAsync(byte[] picture)
        {
            return await Task.Run(() =>
            {
                using (MemoryStream stream = new MemoryStream(picture))
                {
                    return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                }
            });
        }

        public static BitmapSource BytesToBitmapSource(byte[] picture)
        {
            using (MemoryStream stream = new MemoryStream(picture))
            {
                return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }

        public static System.Drawing.Image BytesToImage(byte[] picture)
        {
            using (MemoryStream stream = new MemoryStream(picture))
            {
                return System.Drawing.Image.FromStream(stream);
            }
        }

        public static BitmapImage BytesToBitmap(byte[] picture)
        {
            return BytesToBitmap(picture, 200, "Height");
        }

        public static BitmapImage BytesToBitmapImage(byte[] picture)
        {
            using (var memStream = new MemoryStream(picture))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = memStream;
                image.EndInit();
                return image;
            }
        }

        public static BitmapImage BytesToBitmap(byte[] picture, int sizeLimit, string dimension)
        {
            if (null != picture)
            {
                using (MemoryStream stream = new MemoryStream(picture))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    if ((dimension == "Height" && bitmap.PixelHeight > sizeLimit) ||
                        (dimension == "Width" && bitmap.PixelWidth > sizeLimit))
                    {
                        using (MemoryStream stream2 = new MemoryStream(picture))
                        {
                            bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.StreamSource = stream2;
                            bitmap.CacheOption = BitmapCacheOption.OnLoad;
                            if (dimension == "Height")
                            {
                                bitmap.DecodePixelHeight = sizeLimit;
                            }
                            else if (dimension == "Width")
                            {
                                bitmap.DecodePixelWidth = sizeLimit;
                            }
                            bitmap.EndInit();
                        }
                    }
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            return null;
        }

        public static BitmapImage BitmapToImage(Bitmap bitmap)
        {
            return BitmapToImage(bitmap, ImageFormat.Png);
        }

        public static BitmapImage BitmapToImage(Bitmap bitmap, ImageFormat imageFormat)
        {
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, imageFormat);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            stream.Seek(0, SeekOrigin.Begin);
            image.StreamSource = stream;
            image.EndInit();
            image.Freeze();
            return image;
        }

        public static void AddTabIndexStopToControl(Control control, int tabIndex, bool isTabStop)
        {
            if (null != control)
            {
                AddTabIndexToControl(control, tabIndex);
                control.IsTabStop = isTabStop;
            }
        }

        public static void AddTabIndexToControl(Control control, int tabIndex)
        {
            if (null != control)
            {
                control.TabIndex = tabIndex;
                control.IsTabStop = true;
            }
        }

    }

}
