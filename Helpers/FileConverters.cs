namespace GabriniCosmetics.Helpers
{
    public class FileConverters
    {
        public static IFormFile ConvertBase64ToIFormFile(string base64String)
        {
            if (string.IsNullOrWhiteSpace(base64String))
            {
                throw new ArgumentException("Base64 string cannot be null or empty", nameof(base64String));
            }

            // Extract the MIME type and Base64 data
            string[] parts = base64String.Split(',');
            string mimeType = parts[0].Split(':')[1].Split(';')[0]; // Extract MIME type
            string base64Data = parts.Length > 1 ? parts[1] : parts[0];

            // Convert Base64 string to byte array
            byte[] byteArray = Convert.FromBase64String(base64Data);

            // Determine file extension based on MIME type
            string extension = GetFileExtensionFromMimeType(mimeType);

            // Generate a file name
            string fileName = $"file_{Guid.NewGuid()}.{extension}";

            // Create a MemoryStream from the byte array
            var memoryStream = new MemoryStream(byteArray);

            // Create an IFormFile instance
            IFormFile formFile = new FormFile(memoryStream, 0, byteArray.Length, "file", fileName);

            return formFile;
        }

        public static IFormFile ConvertImageToIFormFile(string imagePath)
        {
            if (!File.Exists(imagePath))
                throw new FileNotFoundException("File not found.", imagePath);

            // Read the file into a memory stream
            using var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            memoryStream.Position = 0; // Reset stream position

            // Create an IFormFile
            var fileName = Path.GetFileName(imagePath);
            var formFile = new FormFile(memoryStream, 0, memoryStream.Length, "image", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = GetContentType(imagePath) // Optionally set content type
            };

            return formFile;
        }

        public static string ConvertIFormFileToBase64(IFormFile formFile)
        {
            if (formFile == null)
                throw new ArgumentNullException(nameof(formFile));

            using var memoryStream = new MemoryStream();
            formFile.CopyTo(memoryStream);
            var fileBytes = memoryStream.ToArray();
            return Convert.ToBase64String(fileBytes);
        }

        private static string GetFileExtensionFromMimeType(string mimeType)
        {
            return mimeType.ToLower() switch
            {
                "image/jpeg" => "jpg",
                "image/png" => "png",
                "image/gif" => "gif",
                "image/bmp" => "bmp",
                "image/svg+xml" => "svg",
                "application/pdf" => "pdf",
                "text/plain" => "txt",
                // Add more MIME types and their extensions as needed
                _ => "bin", // Default to binary if MIME type is unknown
            };
        }

        private static string GetContentType(string path)
        {
            var extension = Path.GetExtension(path).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".bmp" => "image/bmp",
                _ => "application/octet-stream",
            };
        }
    }
}
