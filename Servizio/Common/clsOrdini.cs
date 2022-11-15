using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agente_VOG.Common
{
    class clsOrdini
    {
        public class Lista_RigheOrdini
        {
            [JsonProperty("data")]
            public List<clsRigheOrdine> data;
        }


        public class clsRigheOrdine
        {
            public string systemName { get; set; }
            public string orderNumber { get; set; }
            public byte positionNumber { get; set; }
            public string numerationCode { get; set; }
            public string warehouse { get; set; }
            public int internalOrderNumber { get; set; }
            public int internalPositionNumber { get; set; }
            public int internalOrderYear { get; set; }
            public DateTime orderDate { get; set; }
            public DateTime loadingDate { get; set; }
            public string deliveryDate { get; set; }  //causa arriva a null dalla chiamata
            public string invoiceRecipient { get; set; }
            public string consignee { get; set; }
            public string deliveryAddress { get; set; }
            public string freighter { get; set; }
            public string employee { get; set; } 
            public string storage { get; set; }
            public string variety { get; set; }
            public string quality { get; set; }
            public string caliber { get; set; }
            public string growing { get; set; }
            public string colour { get; set; }
            public int cropYear { get; set; }
            public string palletization { get; set; }
            public string packagingVariant { get; set; }
            public int kgGros { get; set; }
            public int kgNet { get; set; }
            public int quantityPallets { get; set; }
            public int quantityPackaging { get; set; }
            public bool cancelled { get; set; }
            public bool closed { get; set; }
            public DateTime lastUpdateTime { get; set; }
        }
    }

    public class Lista_StatoOrdini
    {
        [JsonProperty("data")]
        public List<clsStatoOrdine> data;
    }


    public class clsStatoOrdine
    {
        public string systemName { get; set; }
        public string ppsLocation { get; set; }
        public int orderID { get; set; }
        public string orderNumber { get; set; }
        public byte positionNumber { get; set; }
        public string warehouse { get; set; }
        public int InternalOrderNumber { get; set; }
        public int InternalPositionNumber { get; set; }
        public int internalOrderYear { get; set; }
        public int splitNumber { get; set; }
        public string productionState { get; set; }
        public string lineId { get; set; }
        public int quantityPallets { get; set; }
        public int quantityPackagingProduced { get; set; }
        public string primOrderRef { get; set; }
        public string primOrderPrio { get; set; }
        public DateTime lastUpdateTime { get; set; }
    }
}
