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
    class Server
    {
        public static void Start()
        {
            // Set up the server
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);
            server.Start();
            Console.WriteLine("Server started. Waiting for client connection...");

            // Run the server logic in a loop
            while (true)
            {
                // Accept a client connection
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client connected.");

                // Send the file list to the client
                NetworkStream stream = client.GetStream();
                SendFileList(stream);

                // Close the client
                client.Close();
            }
        }

        private static void SendFileList(NetworkStream stream)
        {
            // Read the search word from the client
            byte[] data = new byte[1024];
            int bytesReceived = stream.Read(data, 0, data.Length);
            string searchWord = Encoding.UTF8.GetString(data, 0, bytesReceived);

            // Search for .txt files containing the word
            string[] txtFiles = Directory.GetFiles(@"D:\FileFinder\Test", "*.txt");
            Console.WriteLine("Files containing the word '" + searchWord + "':");
            List<string> matchingFiles = new List<string>();
            for (int i = 0; i < txtFiles.Length; i++)
            {
                string fileContents = File.ReadAllText(txtFiles[i]);
                if (fileContents.Contains(searchWord))
                {
                    Console.WriteLine(i + ": " + Path.GetFileName(txtFiles[i]));
                    matchingFiles.Add(Path.GetFileName(txtFiles[i]));
                }
            }

            // If no matching files were found, send a message to the client
            if (matchingFiles.Count == 0)
            {
                Console.WriteLine("No matching files found");
                data = Encoding.UTF8.GetBytes("No matching files found");
                stream.Write(data, 0, data.Length);
                return;
            }

            // Send the list of matching files to the client
            string fileList = string.Join("\n", matchingFiles);
            data = Encoding.UTF8.GetBytes(fileList);
            stream.Write(data, 0, data.Length);

            // Read the file index from the client
            bytesReceived = stream.Read(data, 0, data.Length);
            int fileIndex = int.Parse(Encoding.UTF8.GetString(data, 0, bytesReceived));

            // Read the selected file into a string and convert it to a byte array
            string fileData = File.ReadAllText(txtFiles[fileIndex]);
            data = Encoding.UTF8.GetBytes(fileData);

            // Send the byte array to the client
            stream.Write(data, 0, data.Length);
        }
    }
}
