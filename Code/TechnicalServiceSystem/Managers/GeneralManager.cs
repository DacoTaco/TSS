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

using NHibernate;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using TechnicalServiceSystem.Entities.General;

namespace TechnicalServiceSystem
{
    /// <summary>
    ///     General Manager of TSS. Handles the data in the database of departments,locations,photos,...
    /// </summary>
    public class GeneralManager : Utilities.DatabaseManager
    {
        /// <summary>
        ///     retrieve list with all known departments
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Department> GetDepartments(string companyName = "%")
        {
            ObservableCollection<Department> ret = null;
            try
            {
                if (string.IsNullOrWhiteSpace(companyName))
                    companyName = "%";

                var session = GetSession();
                var list = session.CreateSQLQuery("exec General.GetDepartments :companyName")
                    .AddEntity(typeof(Department))
                    .SetParameter("companyName",companyName,NHibernateUtil.String)
                    .List<Department>();

                ret = new ObservableCollection<Department>(list);
            }
            catch (Exception ex)
            {
                throw new Exception("General_Manager_Failed_Get_Departments : " + ex.Message, ex);
            }

            if(ret == null)
                return new ObservableCollection<Department>();
            return ret;
        }

        public Department GetDepartment(int departmentID)
        {
            try
            {
                var session = GetSession();
                return session.QueryOver<Department>()
                    .Where(d => d.ID == departmentID)
                    .SingleOrDefault();
            }
            catch(Exception ex)
            {
                throw new Exception("General_Manager_Failed_Get_Department : " + ex.Message, ex);
            }

        }
        /// <summary>
        ///     Retrieve List with all known locations. parameters are optional. default is all locations from all companies.
        /// </summary>
        /// <param name="departmentID"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public ObservableCollection<Location> GetLocations(int departmentID = 0, string companyName = "%")
        {
            var ret = new ObservableCollection<Location>();

            try
            {
                var session = GetSession();
                ret = new ObservableCollection<Location>(
                    session.CreateSQLQuery("exec General.GetLocationsByID :departmentID, :companyName ")
                        .AddEntity(typeof(Location))
                        .SetParameter("departmentID", departmentID, NHibernateUtil.Int32)
                        .SetParameter("companyName", companyName, NHibernateUtil.String)
                        .List<Location>()
                );
            }
            catch (Exception ex)
            {
                throw new Exception("General_Manager_Failed_Get_Locations : " + ex.Message, ex);
            }

            return ret;
        }
        public Location GetLocation(int LocationID)
        {
            var session = GetSession();
            return session.QueryOver<Location>()
                .Where(l => l.ID == LocationID)
                .SingleOrDefault();
        }
        public Photo GetPhoto(string PhotoName)
        {
            try
            {
                var session = GetSession();
                return session.QueryOver<Photo>()
                    .Where(p => p.FileName == PhotoName)
                    .SingleOrDefault<Photo>();
            }
            catch (Exception ex)
            {
                throw new Exception("General_mananger_Failed_Get_Photo : " + ex.Message, ex);
            }
        }
        /// <summary>
        ///     Write Photo to server's hard drive and pushes the Photo filename to the database. it takes the file from the
        ///     PhotoSource and writes it to disk. it also changes the given object to reflect the changes
        /// </summary>
        /// <param name="photo"></param>
        /// <returns>bool</returns>
        public bool SavePhotoToServer(ref Photo photo)
        {
            if (photo == null || photo.ID > 0 || 
                String.IsNullOrWhiteSpace(photo.FileName) || String.IsNullOrWhiteSpace(photo.PhotoSource) || 
                File.Exists(photo.FileName) || File.Exists($"./images/{photo.FileName}")
                )
                return true;

            Image image = null;
            ImageFormat imageFormat = null;
            Photo _photo = new Photo() {ID = photo.ID, FileName = photo.FileName, PhotoSource = photo.PhotoSource};
            var filename = "";

            if (_photo.PhotoSource.StartsWith("data:"))
            {
                try
                {                  
                    if(!ImageUriParser.TryParseUri(_photo.PhotoSource,out image,out imageFormat))
                        return false;

                }
                catch (Exception ex)
                {
                    throw new Exception($"General_Manager_Save_Image_Error : {ex.Message}");
                }
            }
            else if (File.Exists(_photo.PhotoSource))
            {
                //in case of actual image data (WPF)
                try
                {
                    image = Image.FromFile(_photo.PhotoSource);
                    if(image != null)
                        imageFormat = image.RawFormat;
                }
                catch(Exception ex)
                {
                    throw new Exception($"General_Manager_Save_Image_Error : {ex.Message}");
                }
            }

            if (image == null || image.Height <= 0 || image.Width <= 0)
                return false;

            try
            {
                filename = $"./images/{_photo.FileName}.{imageFormat.ToString()}";
                var imageStream = new MemoryStream();
                image.Save(imageStream, imageFormat);
                imageStream.Position = 0;

                //flip image if needed, since we are removing exif rotation it needs to be fixed before stripping it
                //http://stackoverflow.com/questions/27835064/get-image-orientation-and-rotate-as-per-orientation
                //exif orientationID = 0x112
                var orientationId = 0x112;
                if (image.PropertyIdList.Contains(orientationId))
                {
                    var orientation = (int)image.GetPropertyItem(orientationId).Value[0];
                    switch (orientation)
                    {
                        case 2:
                            image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            break;
                        case 3:
                            image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case 4:
                            image.RotateFlip(RotateFlipType.Rotate180FlipX);
                            break;
                        case 5:
                            image.RotateFlip(RotateFlipType.Rotate90FlipX);
                            break;
                        case 6:
                            image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case 7:
                            image.RotateFlip(RotateFlipType.Rotate270FlipX);
                            break;
                        case 8:
                            image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        case 1:
                        // No rotation required
                        default:
                            break;
                    }

                    // This EXIF data is now invalid and should be removed.
                    image.RemovePropertyItem(orientationId);
                }

                //resize image untill we have a size we can live with :P
                //max size : 512KB!
                //http://stackoverflow.com/questions/8790275/resize-image-which-is-placed-in-byte-array
                //this process also strips the EXIF btw!
                Bitmap resizedImage = null;
                double imageScale = 1f;
                while (imageStream.ToArray().Length > 524288)
                {
                    resizedImage = new Bitmap(image,
                        new Size((int)(image.Width * imageScale), (int)(image.Height * imageScale)));
                    var OutputStream = new MemoryStream();

                    resizedImage.Save(OutputStream, imageFormat);
                    imageStream = OutputStream;
                    imageScale -= 0.05f;
                }

                if (resizedImage != null)
                    image = resizedImage;


                //save file!
                var path = Path.GetFullPath(Path.Combine(Settings.GetServerPath(), filename));
                if(!Directory.Exists(Path.GetDirectoryName(path)))
                    throw new Exception($"The server path '{Path.GetDirectoryName(path)}' does not exist.");
                image.Save(path, imageFormat);

                //ok, we saved the image, now we need to set the photo object and pass it on to the session!
                _photo.FileName = filename;
                var session = GetSession();
                session.Save(_photo);
                _photo.PhotoSource = null;
                photo = session.QueryOver<Photo>()
                    .Where(p => p.FileName == _photo.FileName)
                    .SingleOrDefault();

                return true;
            }
            catch (Exception ex)
            {
                var path = Path.GetFullPath(Path.Combine(Settings.GetServerPath(), filename));
                if (!String.IsNullOrWhiteSpace(filename) && File.Exists(path))
                    File.Delete(path);

                throw new Exception($"General_Manager_Save_Image_Error : {ex.Message}{Environment.NewLine}{ex.InnerException?.Message??""}");
            }
        }
    }
}