using Newtonsoft.Json.Converters;

namespace KA.Entities.Attributes
{
    public class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            // 사용 : [JsonConverter(typeof(CustomDataTimeConverter))]
            base.DateTimeFormat = "yyyy-MM-dd";
        }

        public CustomDateTimeConverter(string format)
        {
            // 사용 : [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
            base.DateTimeFormat = format;
        }
    }

    public class CustomFullDateTimeConverter : IsoDateTimeConverter
    {
        public CustomFullDateTimeConverter()
        {
            base.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        }

        public CustomFullDateTimeConverter(string format)
        {
            base.DateTimeFormat = format;
        }
    }
}
