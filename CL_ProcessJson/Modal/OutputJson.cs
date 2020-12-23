using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
namespace CL_ProcessJson.Moda.Output
{
    #region MyRegion
    [DataContract]
    public class ActivityPartyObj
    {
        [DataMember]
        public string LogicalName { get; set; }
        [DataMember]
        public string LogicalSetName { get; set; }
        [DataMember]
        public Guid UniqueID { get; set; }
    }
    [DataContract]
    public class FinalOutput
    {
        [DataMember]
        public List<ActivityPartyObj> FROM { get; set; }
        [DataMember]
        public List<ActivityPartyObj> TO { get; set; }
        [DataMember]
        public List<ActivityPartyObj> CC { get; set; }
        [DataMember]
        public List<ActivityPartyObj> BCC { get; set; }
        [DataMember]
        public List<Guid> TEMPLATE { get; set; }
    }
    #endregion
}
