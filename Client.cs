using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FileFinder
{
    class Client
    {
        public static void Start()
        {
            // Connect to the server
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Parse("127.0.0.1"), 8080);
            Console.WriteLine("Connected to the server.");

            // Receive the file list from the server
            NetworkStream stream = client.GetStream();
            ReceiveFileList(stream);

            // Close the client
            client.Close();
        }

        private static void ReceiveFileList(NetworkStream stream)
        {
            // Read the search word from the user
            Console.WriteLine("Enter a search word: ");
            string searchWord = Console.ReadLine();

            // Send the search word to the server
            byte[] data = Encoding.UTF8.GetBytes(searchWord);
            stream.Write(data, 0, data.Length);

            // Receive the file list from the server
            data = new byte[1024];
            int bytesReceived = stream.Read(data, 0, data.Length);
            string fileList = Encoding.UTF8.GetString(data, 0, bytesReceived);

            // If no matching files were found, print a message and return
            if (fileList == "No matching files found")
            {
                Console.WriteLine("No matching files found");
                return;
            }

            // Print the list of matching files
            Console.WriteLine("Files containing the word '" + searchWord + "':");
            Console.WriteLine(fileList);

            // Parse the file list to extract the individual file names
            string[] files = fileList.Split('\n');

            // Ask the user which file they want to receive
            Console.WriteLine("Enter the number of the file you want to receive: ");
            int fileIndex = int.Parse(Console.ReadLine());
            if (fileIndex < 0 || fileIndex >= files.Length)
            {
                Console.WriteLine("Invalid file index.");
                return;
            }

            // Send the file index to the server
            data = Encoding.UTF8.GetBytes(fileIndex.ToString());
            stream.Write(data, 0, data.Length);

            // Receive the selected file from the server
            data = new byte[1024];
            bytesReceived = stream.Read(data, 0, data.Length);
            string fileData = Encoding.UTF8.GetString(data, 0, bytesReceived);

            // Save the file to a local file
            string fileName = files[fileIndex];
            string filePath = @"D:\FileFinder\Recieved\" + fileName;
            File.WriteAllText(filePath, fileData);
            Console.WriteLine("File saved to " + filePath);
        }
    }
}