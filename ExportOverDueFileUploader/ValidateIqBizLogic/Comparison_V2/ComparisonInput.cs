using Newtonsoft.Json.Linq;

namespace ExportOverDueFileUploader.ValidateIqBizLogic.Comparison_V2
{
    public enum DocumentType
    {
        FI,
        GD
    }

    public enum ComparisonType
    {
        Import = 1,
        Export = 2
    }

    public class ComparisonInput
    {
        public string Payload { get; set; } = string.Empty;
        public long? Id { get; set; }
        public DocumentType Type { get; set; } = DocumentType.FI;
        public List<RelatedRecord> RelatedRecords
        {
            get => _relatedRecords;
            set
            {
                NotSerialized.Clear();
                foreach (var relatedRecord in value)
                {
                    NotSerialized.Add(relatedRecord.NotSerialized);
                }
                _relatedRecords = value;
            }
        }

        private List<RelatedRecord> _relatedRecords = [];
        public List<JToken> NotSerialized { get; } = [];
    }

    public class RelatedRecord
    {
        public long? RelationId { get; set; }
        private string _Payload = string.Empty;
        public string Payload
        {
            get => _Payload;
            set
            {
                _Payload = value;
                NotSerialized = JToken.Parse(value);
            }
        }
        public JToken NotSerialized { get; private set; } = JValue.CreateNull();
    }
}
