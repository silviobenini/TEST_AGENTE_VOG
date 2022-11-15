using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agente_VOG.Common
{
     
    public class Lista_AnagraficaVarianteImballaggi
    {
        [JsonProperty("data")]
        public List<clsAnagraficaVarianteImballaggi> data;
    }


    public class clsAnagraficaVarianteImballaggi {
        public string code { get; set; }
        public string internalNameDE { get; set; }
        public string internalNameIT { get; set; }
        public string nameDE { get; set; }
        public string nameIT { get; set; }
        //public string packagingProductionGroup { get; set; }
        public string workPlan { get; set; } 
        public bool locked { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }

    public class Lista_Stabilimento
    {
        [JsonProperty("data")]
        public List<clsStabilimento> data;
    }


    public class clsStabilimento
    {
        public string code { get; set; }
        public string abbreviation { get; set; }
        public string warehouse { get; set; }
        public bool locked { get; set; }
        public DateTime lastUpdateTime { get; set; } 
    }

    public class Lista_Cooperativa
    {
        [JsonProperty("data")]
        public List<clsCooperativa> data;
    }


    public class clsCooperativa
    {
        public string code { get; set; }
        public string searchString { get; set; }
        public string address { get; set; }
        public bool locked { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }

    public class Lista_Indirizzi
    {
        [JsonProperty("data")]
        public List<clsIndirizzi> data;
    }


    public class clsIndirizzi
    {
        public string uuid { get; set; }
        public int? personNumber { get; set; }
        public string searchString { get; set; }
        public string street { get; set; }
        public string zip { get; set; }
        public string place { get; set; }
        public string province { get; set; }
        public string nation { get; set; }
        public string language { get; set; }
        public bool locked { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }

    public class Lista_varieta_articoli
    {
        [JsonProperty("data")]
        public List<clsVarieta_articoli> data;
    }


    public class clsVarieta_articoli
    {
        public string code { get; set; }
        public string nameDE { get; set; }
        public string nameIT { get; set; }
        public string type { get; set; }
        public float? weightInBin { get; set; } 
        public bool locked { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }

    public class Lista_gruppo_qualita
    {
        [JsonProperty("data")]
        public List<clsGruppo_qualita> data;
    }


    public class clsGruppo_qualita
    {
        public string code { get; set; }
        public string nameDE { get; set; }
        public string nameIT { get; set; } 
        public DateTime lastUpdateTime { get; set; }
    }

    public class Lista_gruppo_calibro
    {
        [JsonProperty("data")]
        public List<clsGruppo_calibro> data;
    }


    public class clsGruppo_calibro
    {
        public string code { get; set; }
        public string nameDE { get; set; }
        public string nameIT { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }

    public class Lista_calibro
    {
        [JsonProperty("data")]
        public List<clsCalibro> data;
    }


    public class clsCalibro
    {
        public string code { get; set; }
        public string nameDE { get; set; }
        public string nameIT { get; set; }
        public string group { get; set; }
        public bool locked { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }

    public class Lista_qualita
    {
        [JsonProperty("data")]
        public List<clsQualita> data;
    }


    public class clsQualita
    {
        public string code { get; set; }
        public string nameDE { get; set; }
        public string nameIT { get; set; }
        public string group { get; set; }
        public string type { get; set; }
        public bool locked { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }

    public class Lista_coltivazione
    {
        [JsonProperty("data")]
        public List<clsColtivazione> data;
    }


    public class clsColtivazione
    {
        public string code { get; set; }
        public string nameDE { get; set; }
        public string nameIT { get; set; } 
        public string type { get; set; }
        public bool locked { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }

    public class Lista_colore
    {
        [JsonProperty("data")]
        public List<clsColore> data;
    }


    public class clsColore
    {
        public string code { get; set; }
        public string nameDE { get; set; }
        public string nameIT { get; set; } 
        public bool locked { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }

    public class Lista_Workplan
    {
        [JsonProperty("data")]
        public List<clsWorkplan> data;
    }


    public class clsWorkplan
    {
        public string code { get; set; } 
        public string nameDE { get; set; }
        public string nameIT { get; set; }  
        public bool locked { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }


    public class Lista_CodificaArticoli
    {
        [JsonProperty("data")]
        public List<clsCodificaArticoli> data;
    }


    public class clsCodificaArticoli
    {
        public string ppsCode { get; set; }
        public string warehouse { get; set; }
        public string qualityGroup { get; set; }
        public string caliberGroup { get; set; }
        public string growingType { get; set; }
        public string colour { get; set; }
        public string item { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }


    public class Lista_CodificaCaratteristiche
    {
        [JsonProperty("data")]
        public List<clsCodificaCaratteristiche> data;
    }


    public class clsCodificaCaratteristiche
    {
        public string propertyType { get; set; }
        public string ppsCode { get; set; }
        public string item { get; set; }
        public string warehouse { get; set; }
        public string qualityGroup { get; set; }
        public string caliberGroup { get; set; }
        public string growingType { get; set; }
        public string colour { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }

}
