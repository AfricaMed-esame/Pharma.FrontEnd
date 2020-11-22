using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace SureNkap
{
    public class Files
    {
        public static byte[] ReadBytes(string filePath)
        {

            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length; // get file length
                buffer = new byte[length]; // create buffer
                int count; // actual number of bytes read
                int sum = 0; // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count; // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }

            return buffer;
        }

        public static string ReadContent(string filename)
        {
            try
            {
                //setting the file name path    
                var path = System.Web.Hosting.HostingEnvironment.MapPath(System.Configuration.ConfigurationManager.AppSettings["templatePath"]) + filename;
                //check if the file exists in the location.
                if (!File.Exists(path))
                    throw new FileNotFoundException(); // throw an exception here.
                //start reading the file. i have used Encoding 1256 to support arabic text also.
                StreamReader sr = new StreamReader(@path, System.Text.Encoding.GetEncoding(1256));
                var retVal = sr.ReadToEnd();
                sr.Close();
                return retVal;
            }
            catch (Exception ex)
            {
                throw new Exception("Error Reading File." + ex.Message);
            }
        }
    }
}