using System.Collections.Generic;
using Newtonsoft.Json;

namespace CaseService.Services.DTO
{
    public class CreateSpecimenDTO {

        [JsonProperty(PropertyName = "BlockId")]
        public long BlockId { get; set; }

        [JsonProperty(PropertyName = "TissueType")]
        public string TissueType { get; set; }

        [JsonProperty(PropertyName = "Slides")]
        public List<CreateSlideDTO> Slides;
    }
}