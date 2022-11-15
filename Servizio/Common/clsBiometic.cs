using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agente_VOG.Common
{
    class clsBiometic
    {
        public class ListaVarieta_Biometic
        {
            [JsonProperty("data")]
            public List<clsVarietaBiometic> data;
        }


        public class clsVarietaBiometic
        {
            public int id { get; set; } 
            public string code { get; set; }
            public string nameDe { get; set; }
            public string nameIT { get; set; }
            public string item { get; set; }
            public Boolean locked { get; set; }
            public DateTime lastUpdateTime { get; set; } 
        }


        public class Lista_CalibraturaBiometic
        {
            [JsonProperty("data")]
            public List<clsCalibraturaBiometic> data;
        }


        public class clsCalibraturaBiometic
        {
            public DateTime sortingDate { get; set; }
            public string sortingPlant { get; set; }
            public string variety { get; set; }

            public int harvestYear { get; set; }
            
            public string qualityGroupCode { get; set; }
            public string qualityGroupName { get; set; }

            public string colorGroupCode { get; set; }
            public string colorGroupName { get; set; }

            public float Weight { get; set; }

        }


    }
}
