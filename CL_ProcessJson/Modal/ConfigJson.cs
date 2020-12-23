using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CL_ProcessJson.Modal
{
    #region MyRegion
    [DataContract]
    public class Rootobject
    {
        [DataMember]
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
        public string ActivityEntityName { get; set; }
        [DataMember]
        public string FetchXML { get; set; }
    }
    [DataContract]
    public class To
    {
        [DataMember]
        public string ActivityEntityName { get; set; }
        [DataMember]
        public string FetchXML { get; set; }
    }
    [DataContract]
    public class CC
    {
        [DataMember]
        public string ActivityEntityName { get; set; }
        [DataMember]
        public string FetchXML { get; set; }
    }
    [DataContract]
    public class BCC
    {
        [DataMember]
        public string ActivityEntityName { get; set; }
        [DataMember]
        public string FetchXML { get; set; }
    }

    #endregion


   
}
