namespace CL_ProcessJson.Modal.Json2
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract(Name = "Rootobject")]
    public class ConfigJson
    {[DataMember]
        public Template[] Template { get; set; }
        [DataMember]
        public From[] From { get; set; }
        [DataMember]
        public To[] To { get; set; }
        [DataMember]
        public CC[] CC { get; set; }
        [DataMember]
        public BCC[] BCC { get; set; }
    }
    [DataContract]
    public class Template
    {
        [DataMember]
        public string TemplateId { get; set; }
    }
    [DataContract]
    public class From
    {
        [DataMember]
        public string ActivityPartyEntityLogicalName { get; set; }
        [DataMember]
        public string ActivityPartyRecordId { get; set; }
        [DataMember]
        public string FetchXML { get; set; }
    }

    [DataContract]
    public class To
    {
        [DataMember]
        public string ActivityPartyEntityLogicalName { get; set; }
        [DataMember]
        public string ActivityPartyRecordId { get; set; }
        [DataMember]
        public string FetchXML { get; set; }
    }

    [DataContract]
    public class CC
    {
        [DataMember]
        public string ActivityPartyEntityLogicalName { get; set; }
        [DataMember]
        public string ActivityPartyRecordId { get; set; }
        [DataMember]
        public string FetchXML { get; set; }
    }

    [DataContract]
    public class BCC
    {
        [DataMember]
        public string ActivityPartyEntityLogicalName { get; set; }
        [DataMember]
        public string ActivityPartyRecordId { get; set; }
        [DataMember]
        public string FetchXML { get; set; }
    }

}
