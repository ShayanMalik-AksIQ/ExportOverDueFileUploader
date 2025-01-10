using Newtonsoft.Json.Linq;
namespace ExportOverDueFileUploader.Modles
{
    public class FiGds
    {
        public string FiPayload { get; set; } = string.Empty;
        public long? FiId { get; set; }
        public List<Gd> Gds
        {
            get => _gds;
            set
            {
                GdsNotSerialized.Clear();
                foreach (var gds in value)
                {
                    GdsNotSerialized.Add(gds.GdNotSerialized);
                }
                _gds = value;
            }
        }

        private List<Gd> _gds = [];
        public List<JToken> GdsNotSerialized { get; } = [];
    }

    public class Gd
    {
        public long? GdFiId { get; set; }
        private string _gdPayload = string.Empty;
        public string GdPayload
        {
            get => _gdPayload;
            set
            {
                _gdPayload = value;
                GdNotSerialized = JToken.Parse(value);
            }
        }
        public JToken GdNotSerialized { get; private set; } = JValue.CreateNull();
    }
}
