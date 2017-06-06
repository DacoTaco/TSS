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
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Base;

//Basically all the convertors used in TSS. often used to retrieve something out of a list using an ID
//used in the WPF multibinding, notes to string, image ID to uri, datetime to string etc etc
namespace TechnicalServiceSystem
{
    /// <summary>
    /// returns the ToString of the item in given index of the given list.
    /// deprecated because it has so many possible failures and BaseClassListIndexConverter is better
    /// </summary>
    public class ListIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length != 2 )
                return null;

            int? idx = values[0] as int?;

            if (!idx.HasValue || values[1] == null)
                return null;

            IList status = values[1] as IList;

            //because 0 doesn't count, it means nothing..
            int id = idx.Value;
            if (id > 0)
                id--;

            if (status == null || id >= status.Count)
                return null;

            return status[id].ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// returns the ToString of the BaseClass item in the given list who's ID is the same as the given ID
    /// </summary>
    public class BaseClassListIndexConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length != 2)
                return null;

            int? idx = values[0] as int?;

            if (!idx.HasValue || values[1] == null || idx == 0)
                return null;

            IList status = values[1] as IList;

            int id = idx.Value;
            if (status == null )
                return null;

            BaseClass Class = null;

            for (int i = 0; i < status.Count; i++)
            {
                BaseClass temp = status[i] as BaseClass;
                if ( temp != null && temp.ID == id)
                    Class = temp;
            }

            if (Class == null)
                return null;

            return Class.ToString();
        }
        static public string Convert(IList list, int TargetID)
        {
            if (list == null || list.Count <= 0)
                return null;

            BaseClass Class = null;

            for (int i = 0; i < list.Count; i++)
            {
                BaseClass temp = list[i] as BaseClass;
                if (temp != null && temp.ID == TargetID)
                    Class = temp;
            }

            if (Class == null)
                return null;

            return Class.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Class to convert the list of notes into 1 single string.
    /// </summary>
    public class NoteListToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null )
                return null;

            IList status = value as IList;

            if (status == null )
                return null;

            string ret = null;
            foreach (var item in status)
            {
                Note note = item as Note;
                if (note == null)
                    continue;

                if(String.IsNullOrEmpty(ret))
                {
                    ret = String.Format("{0} - {1}", note.NoteDate, note.Text);
                }
                else
                {
                    ret += String.Format("{0}{1} - {2}", Environment.NewLine, note.NoteDate, note.Text);
                }
            }

            return ret;
            
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Load image from URI and return the actual image
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
                if(path == null)
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
            catch (Exception ex)
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

    /// <summary>
    /// Return the Date time in a certain format. handy for bindings.
    /// </summary>
    public class DateTimeToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DateTime) || value == null)
                return null;
            DateTime param = (DateTime)value;

            return param.ToString("dd/MM/yyyy");          
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
