using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace servicecontrolhub.monitors.protocol.dtos
{
    public class serviceDiagnosticsDto
    {        
        [JsonProperty]
        public string service_name { get; set; }
        [JsonProperty]
        public bool cheсk_result { get; set; }
        [JsonProperty]
        public List<errorDto>? errors { get; set; }
    }

    public class errorDto
    {
        [JsonProperty]
        public string entity { get; set; }
        [JsonProperty]
        public string description { get; set; }
    }
}
