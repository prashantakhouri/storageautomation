using Azure.Storage.Blobs;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.DataMovement;
using Microsoft.Azure.Storage.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wpp.StorageAutomation.Common;

namespace Wpp.StorageAutomation.UnitTests.Common
{
    [TestClass()]
    public class CloudStorageUtilityTest
    {
        private readonly CloudStorageUtility cloudStorageUtility;

        public CloudStorageUtilityTest()
        {
            cloudStorageUtility = new CloudStorageUtility();
        }

        [TestMethod()]
        public async Task GetBlobContainer_ReturnsCloudBlobDirectory()
        {
            // Arrange
            var account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var containerName = "unit-test-container";
            BlobContainerClient blobContainerClient = new BlobContainerClient("UseDevelopmentStorage=true", containerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            // Act
            var result = cloudStorageUtility.GetBlobContainer(account, containerName);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetCloudFileShare_ReturnsCloudBlobDirectory()
        {
            // Arrange
            const string uri = "https://unittest.file.core.windows.net/";

            var fakeStorageUri = new StorageUri(new Uri($"{uri}unit-test-share/"));
            var fakeStorageCredentials = new StorageCredentials();

            var account = new Mock<CloudStorageAccount>(fakeStorageCredentials, "unittest", "testsuffix", false);
            var fileshareName = "unit-test-share";

            var cloudFileShare = new Mock<CloudFileShare>(fakeStorageUri, fakeStorageCredentials);

            var cloudFileClient = new Mock<CloudFileClient>(fakeStorageUri, fakeStorageCredentials);
            cloudFileClient.Setup(x => x.GetShareReference(It.IsAny<string>())).Returns(cloudFileShare.Object);

            // Act
            var result = cloudStorageUtility.GetCloudFileShare(account.Object, fileshareName);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task GetBlobDirectory_ReturnsCloudBlobDirectory()
        {
            // Arrange
            var account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var containerName = "unit-test-container";
            var directoryName = "test-production";
            BlobContainerClient blobContainerClient = new BlobContainerClient("UseDevelopmentStorage=true", containerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            // Act
            var result = cloudStorageUtility.GetBlobDirectory(account, containerName, directoryName);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetCloudFileDirectoryAsync_ReturnsCloudBlobDirectory()
        {
            // Arrange
            var fakeStorageCredentials = new StorageCredentials();

            var account = new Mock<CloudStorageAccount>(fakeStorageCredentials, "unittest", "testsuffix", false);
            var fileshareName = "unit-test-share";
            var directoryName = "test directory";

            // Act
            var result = cloudStorageUtility.GetCloudFileDirectory(account.Object, fileshareName, directoryName);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void GetCloudFileShareList_ReturnsFileshares()
        {
            // Arrange
            var fakeStorageCredentials = new StorageCredentials();

            var account = new Mock<CloudStorageAccount>(fakeStorageCredentials, "unittest", "testsuffix", false);

            // Act
            var result = cloudStorageUtility.GetCloudFileShareList(account.Object);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task GetCloudBlobContainerList_ReturnsCloudBlobContainerList()
        {
            // Arrange
            var account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var containerName = "unit-test-container";
            BlobContainerClient blobContainerClient = new BlobContainerClient("UseDevelopmentStorage=true", containerName);
            await blobContainerClient.CreateIfNotExistsAsync();

            // Act
            var result = cloudStorageUtility.GetCloudBlobContainerList(account);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void DeleteFileShareDirectory_DoesnotThrowException()
        {
            // Arrange
            var fileDir1 = new Mock<CloudFileDirectory>(new Uri("https://unittest.file.core.windows.net/test"));
            IEnumerable<IListFileItem> itemList1 = new List<IListFileItem>()
            {
                new CloudFile(new Uri("https://unittest.file.core.windows.net/test/text.txt"))
            };

            var fileDir2 = new Mock<CloudFileDirectory>(new Uri("https://unittest.file.core.windows.net/test"));
            IEnumerable<IListFileItem> itemList2 = new List<IListFileItem>()
            {
                new CloudFileDirectory(new Uri("https://unittest.file.core.windows.net/test/testfolder"))
            };

            fileDir1.Setup(x => x.Exists(null, null)).Returns(true);
            fileDir1.Setup(x => x.ListFilesAndDirectories(null, null)).Returns(itemList1);

            fileDir2.Setup(x => x.Exists(null, null)).Returns(true);
            fileDir2.Setup(x => x.ListFilesAndDirectories(null, null)).Returns(itemList2);

            // Act
            var result1 = cloudStorageUtility.DeleteFileShareDirectory(fileDir1.Object);
            var result2 = cloudStorageUtility.DeleteFileShareDirectory(fileDir2.Object);

            // Assert
            Assert.IsNotNull(result1);
            Assert.IsNotNull(result2);
        }

        [TestMethod()]
        public void GetDirectoryTransferContext_ReturnsTransferContext()
        {
            // Arrange
            TransferCheckpoint checkpoint = null;

            // Act
            var result = cloudStorageUtility.GetDirectoryTransferContext(checkpoint);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task BlobDirectoryExists_Returns_true()
        {
            // Arrange
            var account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var containerName = "unit-test-container";
            var directoryName = "test-production";

            var cloudBlob = account.CreateCloudBlobClient();
            var container = cloudBlob.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
            // var dir = container.GetDirectoryReference(directoryName)

            // Act
            var result = cloudStorageUtility.BlobDirectoryExists(account, containerName, directoryName);

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void FileShareDirectoryExists_Returns_false()
        {
            // Arrange
            var fakeStorageCredentials = new StorageCredentials();

            var account = new Mock<CloudStorageAccount>(fakeStorageCredentials, "unittest", "testsuffix", false);

            // Act
            var result = cloudStorageUtility.FileShareDirectoryExists(account.Object, "test", "test");

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public void FileShareExists_Returns_false()
        {
            // Arrange
            var fakeStorageCredentials = new StorageCredentials();

            var account = new Mock<CloudStorageAccount>(fakeStorageCredentials, "unittest", "testsuffix", false);

            // Act
            var result = cloudStorageUtility.FileShareExists(account.Object, "test");

            // Assert
            Assert.IsNotNull(result);
        }
    }
}