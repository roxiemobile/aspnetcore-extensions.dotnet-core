namespace RoxieMobile.AspNetCore.Net.Http.Headers
{
    // GitHub.com — dotnet/corefx
    // @link https://github.com/dotnet/corefx/blob/master/src/System.Net.Mail/src/System/Net/Mime/MediaTypeNames.cs

    public static class MediaTypeNames
    {
        public static class Text
        {
            public const string Html = "text/html";
            public const string Plain = "text/plain";
            public const string RichText = "text/richtext";
            public const string Xml = "text/xml";
        }

        public static class Application
        {
            public const string AndroidPackage = "application/vnd.android.package-archive";
            public const string FormUrlEncoded = "application/x-www-form-urlencoded";
            public const string Json = "application/json";
            public const string OctetStream = "application/octet-stream";
            public const string Pdf = "application/pdf";
            public const string Rtf = "application/rtf";
            public const string Soap = "application/soap+xml";
            public const string Xml = "application/xml";
            public const string Zip = "application/zip";
        }

        public static class Image
        {
            public const string Gif = "image/gif";
            public const string Jpeg = "image/jpeg";
            public const string Png = "image/png";
            public const string Tiff = "image/tiff";
        }
    }
}