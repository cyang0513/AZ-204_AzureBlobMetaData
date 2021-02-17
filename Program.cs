using System;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;

namespace AzureBlobMetaData
{
   class Program
   {
      static IConfiguration m_Config;

      static void Main(string[] args)
      {
         Console.WriteLine("Show and edit container/blob property and meta data...SDK V11");
         m_Config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

         var destContainerName = m_Config.GetSection("DestinationContainerName").Value;
         var sourceContainerName = m_Config.GetSection("SourceContainerName").Value;
         var sas = m_Config.GetSection("SasToken").Value;
         var account = m_Config.GetSection("Account").Value;


         var storage = new CloudStorageAccount(new StorageCredentials(sas), account, "", true);

         var client = storage.CreateCloudBlobClient();

         var destRef = client.GetContainerReference(destContainerName);

         HandlePropertyAndMetaData(destRef);

         Console.ReadKey();
      }

      static void HandlePropertyAndMetaData(CloudBlobContainer container)
      {
         container.FetchAttributes();

         Console.WriteLine($"Container: {container.Name}");
         Console.WriteLine($"ETag: {container.Properties.ETag}");
         Console.WriteLine($"LastModified: {container.Properties.LastModified}");
         Console.WriteLine($"LeaseState: {container.Properties.LeaseState}");

         container.Metadata.Add("NewData", DateTime.Now.ToLongTimeString());
         container.SetMetadata();

         foreach (var pair in container.Metadata)
         {
            Console.WriteLine($"Meta data: {pair.Key}, Value: {pair.Value}");
         }
      }
   }
}
