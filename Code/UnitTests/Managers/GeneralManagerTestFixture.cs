/*TSS - Technical Service System , a system build to help Technical Services maintain their reports and equipment
Copyright(C) 2019 - Joris 'DacoTaco' Vermeylen

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

using NUnit.Framework;
using System;
using System.IO;
using TechnicalServiceSystem;
using TechnicalServiceSystem.Entities.General;

namespace UnitTests.Managers
{
    [TestFixture]
    class GeneralManagerTestFixture : NhibernateTestFixture
    {
        private GeneralManager gnrlManager = new GeneralManager();
        private readonly string _serverPath = AppDomain.CurrentDomain.BaseDirectory;
        private string _imagesPath => Path.Combine(_serverPath, @".\images");

        [SetUp]
        public void SetUp()
        {
            if (!Directory.Exists(_imagesPath))
                Directory.CreateDirectory(Path.GetFullPath(_imagesPath));
        }

        [TearDown]
        public void TearDown()
        {
            if(File.Exists(Path.Combine(_imagesPath, "temp.png")))
                File.Delete(Path.Combine(_imagesPath, "temp.png"));
            if (Directory.Exists(_imagesPath))
                Directory.Delete(_imagesPath);
        }
        [Test]
        public void CanRetrieveSingleLocation()
        {
            var _location = gnrlManager.GetLocation(5);

            Assert.NotNull(_location);
            Assert.AreEqual(5, _location.ID);
            Assert.That(_location.Description, Is.EqualTo("Verbandkamer"), "Location Name");
            Assert.That(_location.Department.ID,Is.EqualTo(3),"Department ID");
        }

        [Test]
        public void CanRetrievePhoto()
        {
            var photo = gnrlManager.GetPhoto("./system/DefaultUser.jpg");

            Assert.NotNull(photo);
            Assert.AreEqual(1,photo.ID);
            Assert.That(photo.FileName, Is.EqualTo("./system/DefaultUser.jpg"));
        }

        [Test]
        public void CanSaveImageToServerAndDB()
        {
            var photo = new Photo(0, "temp")
            {
                PhotoSource = "data:image/png;base64,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L4Oa9PT0+vXr+/v7my8nm6BDPbAft8Y6HA5zvhcVFbVnz55nz57JY/iFOe89fPiwDC5ldsTSpUsbNGhQunTp0aNH82of7qEVmDkuxcTEmJN799wa6zVat26dl5cngyv8e++/2RH79+/Pz8+XB+AWJQ0sJSXl53e4MpdPkZGRZnd26dLl7NmzshVOK1++vKxK5ueu+AHnQSUN7O7du82aNbtx44a5sjK7c8KECVeuXJHHUHw1a9aU1V+hK6spaWDGrFmzxo4d6+S7GeAPcnJyBg0aJENxmLPKqVOn0pUFuSAwuMru3buL+2ZFa9asCQgICAsLS0pKkk2wEgKzEHOC7eTvzc2lb4MGDUxaa9eulU2wJAKzkFatWhX5km8LFiwwp4KRkZGeejIoioXALKRy5cqy+kVubm6bNm0qVapkTiNlE+yAwCykSpUqsvpJcnKyw+Ho2LHjkydPZBPsg8Cs4vXr1506dZLhPzZv3mzOBkeNGiUzbIjArOLAgQP/vlZ2YmKiSWv58uU/RtgXgVlFhw4d8vPzHz16ZNKaPn26bIXNEZhVBAYGDh06NCQkRGZ4BQKzin379l2+fFkGeAsCAxQRGKCIwABFBAYoIjBAEYEBiggMUERggCICAxQRGKCIwABFBAYoIjBAEYEBiggMUERggCICAxQRGKCIwABFBAYoIjBAEYEBiggMUERggCICAxQRGKCIwABFBAYoIjBAEYEBiggMUERggCICAxQRGKCIwABFBAYoIjBAEYEBiggMUERggCICAxQRGKCIwABFBAYoIjBAEYEBiggMUERggCICAxQRGKCIwABFBAYoIjBAEYEBiggMUERggCICAxQRGKCIwABFBAYoIjBAEYEBiggMUERggCICAxQRGKCIwABFBAYoIjBAEYEBiggMUERggCICAxQRGKCIwABFBAYoIjBAEYEBiggMUPPt2z9IaVaF3raMHwAAAABJRU5ErkJggg=="
            };

            Assert.True(gnrlManager.SavePhotoToServer(ref photo));
            Assert.NotNull(photo);
            Assert.True(photo.ID > 0);
            Assert.Null(photo.PhotoSource);
            Assert.AreEqual("./images/temp.png",photo.FileName.ToLower());
            Assert.True(File.Exists(Path.Combine(_serverPath, photo.FileName)));
        }
    }
}
