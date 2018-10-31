using Newtonsoft.Json;

namespace CaseService.Services.DTO {
    public class CreateSlideDTO {

        [JsonProperty(PropertyName = "ProtocolName")]
        public string ProtocolName { get; set; }

        [JsonProperty(PropertyName = "ProtocolDescription")]
        public string ProtocolDescription { get; set; }
    }
}