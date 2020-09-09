/*TSS - Technical Service System , a system build to help Technical Services maintain their reports and equipment
Copyright(C) 2017 - Joris 'DacoTaco' Vermeylen

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.If not, see http://www.gnu.org/licenses */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using TechnicalServiceSystem.Entities;
using TechnicalServiceSystem.Entities.Tasks;

//Basically all the convertors used in TSS. often used to retrieve something out of a list using an ID
//used in the WPF multibinding, notes to string, image ID to uri, datetime to string etc etc
namespace TechnicalServiceSystem
{
    /// <summary>
    ///     returns the ToString of the BaseClass item in the given list who's ID is the same as the given ID
    /// </summary>
    public class BaseClassListIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2)
                return null;

            var idx = values[0] as int?;

            if (!idx.HasValue || values[1] == null || idx == 0)
                return null;

            var status = values[1] as IList;

            if (status == null)
                return null;

            object Class = null;
            var id = idx.Value;
            for (var i = 0; i < status.Count; i++)
            {
                var temp = status[i] as BaseEntity;
                if (temp != null && temp.ID == id)
                {
                    Class = temp;
                    break;
                }
                    
            }

            if (Class == null)
                return null;

            return Class.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static string Convert(IList list, int TargetID)
        {
            if (list == null || list.Count <= 0)
                return null;

            object Class = null;

            for (var i = 0; i < list.Count; i++)
            {
                var temp = list[i] as BaseEntity;
                if (temp != null && temp.ID == TargetID)
                {
                    Class = temp;
                    break;
                }                 
            }

            if (Class == null)
                return null;

            return Class.ToString();
        }
    }

    /// <summary>
    ///     Class to convert the list of notes into 1 single string.
    /// </summary>
    public class NoteListToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is IList<Note>))
                return null;

            string ret = null;
            var list = value as IList<Note>;
            foreach (var note in list)
            {
                ret += $"{ (string.IsNullOrEmpty(ret)?"":Environment.NewLine) }{ note.NoteDate.ToString("dd/MM/yyyy HH:mm:ss") } - { note.Text }";
            }

            return ret;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     Load image from URI and return the actual image
    /// </summary>
    public class ImageUriToBitmapImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var path = value as string;
            //create new stream and create bitmap frame
            var bitmapImage = new BitmapImage();


            if (path == null)
            {
                path = value.ToString();
                if (path == null)
                    return bitmapImage;
            }

            try
            {
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new FileStream(path, FileMode.Open, FileAccess.Read);
                //load the image now so we can immediately dispose of the stream
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                //clean up the stream to avoid file access exceptions when attempting to delete images
                bitmapImage.StreamSource.Dispose();
                return bitmapImage;
            }
            catch
            {
                bitmapImage = new BitmapImage();
            }

            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public static class ImageUriParser
    {
        public static bool TryParseUri(string URI, out Image ImageOutput , out ImageFormat imageFormat)
        {
            var regex = new Regex(@"data:(?<type>[\w]+)/(?<extension>\w+);(?<encoding>\w+),(?<data>.*)", RegexOptions.Compiled);
            ImageOutput = null;
            imageFormat = null;
            ImageFormat format;

            var match = regex.Match(URI);

            //var mime = match.Groups["mime"].Value;
            var readType = match.Groups["type"].Value;
            var readExtension = match.Groups["extension"].Value;
            var readEncoding = match.Groups["encoding"].Value;
            var readData = match.Groups["data"].Value;

            if (readType.ToLower() != "image")
                throw new ArgumentException($"URI is not of image type. got '{readType}' instead");
            if (readEncoding.ToLower() != "base64")
                throw new ArgumentException($"URI is not encoded with base64. got '{readEncoding}' instead.");
            if (String.IsNullOrWhiteSpace(readExtension))
                throw new ArgumentException($"URI did not contain any valid extension/image type");
            if (String.IsNullOrWhiteSpace(readData))
                throw new ArgumentException($"URI did not contain any valid data");

            try
            {
                format = (ImageFormat)typeof(ImageFormat)
                    .GetProperty(readExtension, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase)
                    .GetValue(null);
            }
            catch(Exception e)
            {
                throw new ArgumentException($"URI did not contain any valid extension/image type'{readExtension}'", e);
            }

            try
            {
                var data = Convert.FromBase64String(readData);
                var imageStream = new MemoryStream(data);
                ImageOutput = Image.FromStream(imageStream);
                imageFormat = format;


                if (ImageOutput == null || imageFormat == null || ImageOutput.Height == 0 || ImageOutput.Width == 0)
                {
                    ImageOutput = null;
                    imageFormat = null;
                    return false;
                }
            }
            catch
            {
                ImageOutput = null;
                imageFormat = null;
                return false;
            }

            return true;
        }
    }
}