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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Drawing;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Base;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

namespace TechnicalServiceSystem
{
    /// <summary>
    /// General Manager of TSS. Handles the data in the database of departments,locations,photos,...
    /// </summary>
    public class GeneralManager : DatabaseManager
    {
        /// <summary>
        /// retrieve list with all known departments
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<DepartmentInfo> GetDepartments(string companyName = "%")
        {
            ObservableCollection<DepartmentInfo> ret = new ObservableCollection<DepartmentInfo>();
            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "General.GetDepartments";

                        if(!String.IsNullOrWhiteSpace(companyName))
                        {
                            var companyParam = command.CreateParameter();
                            companyParam.ParameterName = "companyName";
                            companyParam.Value = companyName;
                            command.Parameters.Add(companyParam);
                        }

                        connection.Open();
                        using (var rdr = command.ExecuteReader())
                        {
                            Int32 IDPos = rdr.GetOrdinal("Department ID");
                            Int32 NamePos = rdr.GetOrdinal("Department Name");
                            Int32 ParentIDPos = rdr.GetOrdinal("Parent ID");

                            while(rdr.Read())
                            {
                                ret.Add(new DepartmentInfo(
                                    rdr.GetInt32(IDPos),
                                    rdr.GetString(NamePos),
                                    rdr.GetInt32(ParentIDPos)
                                    ));
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("General_Manager_Failed_Get_Departments : " + ex.Message,ex);
            }
            return ret;
        }

        /// <summary>
        /// Retrieve List with all known locations. parameters are optional. default is all locations from all companies. 
        /// </summary>
        /// <param name="departmentID"></param>
        /// <param name="companyName"></param>
        /// <returns></returns>
        public ObservableCollection<LocationInfo> GetLocations(int departmentID = 0, string companyName = "%")
        {
            ObservableCollection<LocationInfo> ret = new ObservableCollection<LocationInfo>();

            try
            {
                using (var connection = this.GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "General.GetLocationsByID";

                        var param = command.CreateParameter();
                        param.ParameterName = "departmentID";

                        param.Value = departmentID;
                        command.Parameters.Add(param);

                        param = command.CreateParameter();
                        param.ParameterName = "companyName";
                        param.Value = companyName;
                        command.Parameters.Add(param);

                        connection.Open();
                        using (var rdr = command.ExecuteReader())
                        {
                            Int32 IDPos = rdr.GetOrdinal("Location ID");
                            Int32 NamePos = rdr.GetOrdinal("Location Name");
                            Int32 ParentIDPos = rdr.GetOrdinal("Department ID");

                            while (rdr.Read())
                            {
                                ret.Add(new LocationInfo(
                                    rdr.GetInt32(IDPos),
                                    rdr.GetString(NamePos),
                                    rdr.GetInt32(ParentIDPos)
                                    ));
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("General_Manager_Failed_Get_Locations : " + ex.Message, ex);
            }

            return ret;
        }

        /// <summary>
        /// Retrieve a Photo's location on the server using it's ID
        /// </summary>
        /// <param name="PhotoID"></param>
        /// <returns></returns>
        public string GetPhoto(int PhotoID)
        {
            string ret = String.Empty;

            try
            {
                using (var connection = GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "General.GetPhoto";
                        var param = command.CreateParameter();
                        param.ParameterName = "photoID";
                        param.Value = PhotoID;
                        command.Parameters.Add(param);

                        connection.Open();

                        using (var rdr = command.ExecuteReader())
                        {
                            Int32 PhotoName = rdr.GetOrdinal("PhotoName");

                            if (rdr.Read())
                                ret = rdr.GetString(PhotoName);
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                throw new Exception("General_mananger_Failed_Get_Photo : " + ex.Message, ex);
            }
            return ret;
        }
        /// <summary>
        /// Retrieve a Photo's ID using its name on the server
        /// </summary>
        /// <param name="PhotoName"></param>
        /// <returns></returns>
        public int GetPhoto(string PhotoName)
        {
            int ret = 0;

            if (String.IsNullOrWhiteSpace(PhotoName))
                return 0;

            try
            {
                using (var connection = GetConnection())
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "General.GetPhotoID";
                        var param = command.CreateParameter();
                        param.ParameterName = "photoName";
                        param.Value = PhotoName;
                        command.Parameters.Add(param);

                        connection.Open();

                        using (var rdr = command.ExecuteReader())
                        {
                            Int32 PhotoIDPos = rdr.GetOrdinal("PhotoID");

                            if (rdr.Read())
                                ret = rdr.GetInt32(PhotoIDPos);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("General_mananger_Failed_Get_Photo : " + ex.Message, ex);
            }
            return ret;
        }
        
        /// <summary>
        /// write the filename to the database! if the filename is already in the database it will return the ID of the one in the database!
        /// </summary>
        /// <param name="photoPath"></param>
        /// <returns>PhotoID</returns>
        private int SavePhotoToDatabase(string photoPath)
        {
            int ret = 0;
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();

                    //check if photo isn't already in the database!
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "General.GetPhotoID";
                        command.CommandType = CommandType.StoredProcedure;

                        var param = command.CreateParameter();
                        param.ParameterName = "photoName";
                        param.Value = photoPath;
                        command.Parameters.Add(param);

                        var retValue = command.CreateParameter();
                        retValue.Direction = ParameterDirection.ReturnValue;
                        command.Parameters.Add(retValue);

                        //if the photo is found we just return the ID of the file in system.
                        //else, add it to the system
                        if (command.ExecuteNonQuery() > 0)
                        {
                            ret = (int)retValue.Value;
                            return ret;
                        }
                    }

                    using (var trans = connection.BeginTransaction())
                    {
                        using (var command = connection.CreateCommand())
                        {
                            command.Transaction = trans;
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = "General.AddPhoto";


                            var param = command.CreateParameter();
                            param.ParameterName = "PhotoName";
                            param.Value = photoPath;
                            command.Parameters.Add(param);

                            var retValue = command.CreateParameter();
                            retValue.Direction = ParameterDirection.ReturnValue;
                            command.Parameters.Add(retValue);

                            if (command.ExecuteNonQuery() == 0)
                            {
                                trans.Rollback();
                            }
                            else
                            {
                                trans.Commit();
                                ret = (int)retValue.Value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("General_mananger_Failed_Save_Photo_DB : " + ex.Message, ex);
            }
            return ret;
        }

        /// <summary>
        /// Write Photo to server's hard drive and pushes the Photo filename to the database. it takes the file from the PhotoSource and writes it to disk. it also changes the given object to reflect the changes
        /// </summary>
        /// <param name="photo"></param>
        /// <returns>PhotoID</returns>
        public int SavePhotoToServer(ref PhotoInfo photo)
        {
            int ret = 0;
            int newID = 0;
            string filename = photo.FileName;
            ImageFormat format = ImageFormat.Jpeg;
            Image image = null;

            if (photo == null || string.IsNullOrWhiteSpace(filename) || String.IsNullOrWhiteSpace(photo.PhotoSource))
                return ret;

            if (photo.PhotoSource.StartsWith("data:"))
            {
                //its a mime data string!
                try
                {
                    //ok so the start is good. lets try and extract the header!
                    string[] dataheader = photo.PhotoSource.Split(new string[] { "data:" }, 2, StringSplitOptions.None);
                    if (dataheader == null || dataheader.Length < 0 || dataheader.Length > 2 || !dataheader[1].StartsWith("image/"))
                    {
                        //not valid data, obviously
                        return ret;
                    }

                    dataheader = dataheader[1].Split(new string[] { "image/" }, 2, StringSplitOptions.None);
                    //check if we failed to extract the image tag out!
                    if (dataheader == null || dataheader.Length < 0 || dataheader.Length > 2)
                    {
                        return ret;
                    }

                    //extract the file type
                    dataheader = dataheader[1].Split(new string[] { ";" }, 2, StringSplitOptions.RemoveEmptyEntries);
                    if (dataheader == null || dataheader.Length < 0 || dataheader.Length > 2)
                    {
                        return ret;
                    }

                    string mineExtention = dataheader[0].ToLower();
                    string extention = String.Empty;

                    //check if the extention is legit. this is a list of common online mime types for images
                    switch (mineExtention)
                    {
                        case "jpeg":
                        case "jpg":
                        case "pjpeg":
                            extention = ".jpg";
                            format = ImageFormat.Jpeg;
                            break;
                        case "gif":
                            extention = ".gif";
                            format = ImageFormat.Gif;
                            break;
                        case "png":
                        case "x-png":
                            extention = ".png";
                            format = ImageFormat.Png;
                            break;
                        default:
                            break;
                    }

                    if (String.IsNullOrWhiteSpace(extention))
                    {
                        //no valid extention found;
                        return ret;
                    }


                    //now check if its a base64 encoding. im not sure what to do else, so break! xD
                    if (dataheader[1] == null || dataheader[1].Length <= 6 || !dataheader[1].StartsWith("base64"))
                    {
                        return ret;
                    }

                    string[] rawData = dataheader[1].Split(new string[] { "base64," }, 2, StringSplitOptions.None);
                    //now check if actually have data xD
                    if (rawData == null || rawData[1].Length <= 0)
                    {
                        return ret;
                    }

                    string encodedData = rawData[1];
                    byte[] data = Convert.FromBase64String(encodedData);

                    //check if the data is an image!
                    if(IsValidImage(data))
                    {
                        MemoryStream imageStream = new MemoryStream(data);
                        image = Image.FromStream(imageStream);

                        if (image != null && image.Height > 0)
                        {
                            //we have an image! :o :D 
                            filename = String.Format("{0}{1}", filename, extention);
                        }
                        else
                        {
                            image = null;
                            format = null;
                        }
                    }
                    else
                    {
                        image = null;
                        format = null;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("General_Manager_Save_Photo_Failed : failed to save MIME Data", ex);
                }
            }
            else
            {
                //in case of actual image data (WPF)
                if(File.Exists(photo.PhotoSource))
                {
                    try
                    {
                        image = Image.FromFile(photo.PhotoSource);

                        if(image != null && image.Height > 0)
                            format = image.RawFormat;
                        else
                        {
                            image = null;
                        }
                    }
                    catch
                    {
                        image = null;
                        format = null;
                    }
                }

            }


            //we retrieved the data! time to write it away to server!
            if(image != null)
            {
                try
                {
                    //setup everything for saving of images
                    filename = String.Format("./images/{0}", filename);
                    HttpServerUtility Server = HttpContext.Current.Server;

                    double scale = 1f;
                    MemoryStream imageStream = new MemoryStream();
                    image.Save(imageStream, format);
                    imageStream.Position = 0;
                    Bitmap resizedImage = null;
                    //exif orientationID = 0x112
                    int orientationId = 0x112;

                    //flip image if needed, since we are removing exif rotation needs to be fixed before stripping it
                    //http://stackoverflow.com/questions/27835064/get-image-orientation-and-rotate-as-per-orientation

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
                    while (imageStream.ToArray().Length > 512000)
                    {
                        resizedImage = new Bitmap(image, new Size((int)(image.Width * scale), (int)(image.Height * scale)));
                        MemoryStream OutputStream = new MemoryStream();

                        resizedImage.Save(OutputStream, format);

                        imageStream = OutputStream;

                        scale -= 0.05f;
                    }
                    if (resizedImage != null)
                        image = resizedImage;


                    //save file! just passing the SERVER.MapPath didn't work for some odd reason, so here we go :P
                    string path = Server.MapPath(filename);
                    image.Save(path, format);

                    if (!File.Exists(Server.MapPath(filename)))
                        filename = String.Empty;
                }
                catch(Exception ex)
                {
                    filename = String.Empty;
                }
                
            }


            //if we succeeded we should have a filename. so save it to database. if we got a photoID back, we can use it and adjust the PhotoInfo
            if (!String.IsNullOrWhiteSpace(filename))
            {
               newID = SavePhotoToDatabase(filename);
            }

            if(newID > 0)
            {
                photo.ID = newID;
                photo.FileName = filename;
                photo.PhotoSource = String.Empty;
                ret = newID;
            }

            return ret;
        }

        /// <summary>
        /// check if the data in a byte array is an actual image.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns> boolean stating if its valid or not</returns>
        private static bool IsValidImage(byte[] bytes)
        {
            //basically, if we can cast the data into an image we have an actual image!
            if (bytes == null || bytes.Length <= 0)
                return false;

            try
            {
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    Image image = Image.FromStream(ms);

                    if (image.Height > 0)
                        return true;
                }
            }
            catch (ArgumentException)
            {
                return false;
            }
            return true;
        }
    }
}
