using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agente_VOG.Common
{
    class clsGiacenze
    {
        public class Lista_Giacenze
        {
            [JsonProperty("data")]
            public List<clsGiacenza> data;
        }


        public class clsGiacenza
        {
            public string warehouse { get; set; }
            public string storage { get; set; } 
            public int cropYear { get; set; }
            public string variety { get; set; }
            public string caliber { get; set; }
            public string colour { get; set; }
            public string quality { get; set; }
            public string growingType { get; set; } 
            public int inStock { get; set; }
            public int available { get; set; }
            public int closed { get; set; }
            public string calendarWeekDay { get; set; } 
            public string age { get; set; }
            public DateTime lastUpdateTime { get; set; } 
        }

    }
}
