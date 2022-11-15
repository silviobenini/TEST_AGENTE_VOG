using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agente_VOG.Common
{
    public class SubFieldsData
    {
        public string Kistennummer { get; set; }
    }

    public class SubFields
    {
        public SubFieldsData Fields { get; set; } = new SubFieldsData();
    }

    public class Position
    {
        public SubFields[] Data { get; set; }
    }

    public class Collections
    {
        public Position UmlagerungErntewarePosition { get; set; } = new Position();
    }

    public class Umlagerungernteware
    {
        public string VerarbeitungsbetriebKuerzel { get; set; }
        
        [JsonProperty("LagerCode")]
        public string LagerCode { get; set; }
        public string WarenempfaengerNummer { get; set; }
        public string LieferadresseNummer { get; set; }
        public bool Vollstaendig { get; set; }  
    }

    public class Finale
    {
        [JsonProperty("Fields")]
        public Umlagerungernteware Testata { get; set; } = new Umlagerungernteware();
        public Collections Collections { get; set; } = new Collections();
    }

    class clsConferimenti
    {
        public class Conferimenti
        {
            [JsonProperty("data")]
            public List<clsConferimentoDett> data;
        }


        public class clsConferimentoDett
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
