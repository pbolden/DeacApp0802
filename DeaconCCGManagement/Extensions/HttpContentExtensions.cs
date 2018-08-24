using System.Collections.Generic;

namespace DeaconCCGManagement.Extensions
{
    //public static class HttpContentExtensions
    //{
    //    public static async Task<HttpPostedData> ParseMultipartAsync(this HttpContent postedContent)
    //    {
    //        // Get the content
    //        var provider = await postedContent.ReadAsMultipartAsync();

    //        // Create two dictionaries
    //        var files = new Dictionary<string, HttpPostedFile>(StringComparer.InvariantCultureIgnoreCase);
    //        var fields = new Dictionary<string, HttpPostedField>(StringComparer.InvariantCultureIgnoreCase);

    //        // Sort the content into files and fields
    //        foreach (var content in provider.Contents)
    //        {
    //            var fieldName = content.Headers.ContentDisposition.Name.Trim('"');

    //            // Has a file name so it must be a file
    //            if (!string.IsNullOrEmpty(content.Headers.ContentDisposition.FileName))
    //            {
    //                var file = await content.ReadAsByteArrayAsync();
    //                var fileName = content.Headers.ContentDisposition.FileName.Trim('"');
    //                var mimeType = content.Headers.ContentType.MediaType;

    //                files.Add(fieldName, new HttpPostedFile(fileName, mimeType, file));
    //            }
    //            else // Otherwise, it's a field.
    //            {
    //                var data = await content.ReadAsStringAsync();
    //                fields.Add(fieldName, new HttpPostedField(fieldName, data));
    //            }
    //        }

    //        // Return the result
    //        return new HttpPostedData(fields, files);
    //    }
    //}

    public class HttpPostedData
    {
        public IDictionary<string, HttpPostedField> Fields { get; private set; }
        public IDictionary<string, HttpPostedFile> Files { get; private set; }

        public HttpPostedData(IDictionary<string, HttpPostedField> fields,
                              IDictionary<string, HttpPostedFile> files)
        {
            Fields = fields;
            Files = files;
        }
    }

    public class HttpPostedFile
    {
        public string FileName { get; private set; }
        public string MimeType { get; set; }
        public byte[] File { get; private set; }

        public HttpPostedFile(string fileName, string mimeType, byte[] file)
        {
            FileName = fileName;
            MimeType = mimeType;
            File = file;
        }
    }

    public class HttpPostedField
    {
        public string Name { get; private set; }
        public string Value { get; private set; }

        public HttpPostedField(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}