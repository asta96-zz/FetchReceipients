using CL_ProcessJson.Moda.Output;
using CL_ProcessJson.Modal.Json2;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;

namespace CL_ProcessJson.BusinessLogic
{
    public class BLogic
    {
        static ITracingService Itracing = null;
        public BLogic(ITracingService tracing)
        {
            Itracing = tracing;
        }
        public static T Deserialize<T>(string response)
        {
            //Stream DeSerializememoryStream = new MemoryStream();
            //StreamWriter writer = new StreamWriter(DeSerializememoryStream);
            //writer.Write(response);
            //writer.Flush();
            byte[] byteArray = Encoding.UTF8.GetBytes(response);
            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            MemoryStream stream = new MemoryStream(byteArray);
            Itracing.Trace("Inside Deserialize");
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            //StreamReader reader = new StreamReader(stream);
            //string x= reader.ReadToEnd();
            var result = (T)serializer.ReadObject(stream);//JsonSerializer.DeserializeAsync<T>(contentStream.Result, Options).Result;            

            return result;
        }
        internal static string Serialize<T>(T obj)
        {
            Itracing.Trace("inside Serialize");
            string result = string.Empty;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(memoryStream, obj);
                StreamReader sr = new StreamReader(memoryStream);
                result = sr.ReadToEnd();
            }
            return result;
        }

        internal Stream ConvertStringToStream(string ValueResponse)
        {
            Itracing.Trace("Inside ConvertStringToStream");
            Stream DeSerializememoryStream = new MemoryStream();
            StreamWriter writer = new StreamWriter(DeSerializememoryStream);
            writer.Write(ValueResponse);
            writer.Flush();
            return DeSerializememoryStream;

        }
        internal string FetchConfigSetting(IOrganizationService service, string KeyName)
        {
            Itracing.Trace("Inside FetchConfigSetting");
            string Fetch = $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                              <entity name='dev_systemsetting'>
                                <attribute name='dev_systemsettingid' />
                                <attribute name='dev_key' />
                                <attribute name='createdon' />
                                <attribute name='dev_value' />
                                <order attribute='dev_key' descending='false' />
                                <filter type='and'>
                                  <condition attribute='dev_key' operator='eq' value='{KeyName}' />
                                </filter>
                              </entity>
                            </fetch>";
            EntityCollection collection = service.RetrieveMultiple(new FetchExpression(Fetch));
            if (collection.Entities.Count > 0)
                return collection.Entities.FirstOrDefault().GetAttributeValue<string>("dev_value");
            return null;
        }


        public EntityCollection ExecuteFetchXml(string Fetch, IOrganizationService service, string RegardingId = null)
        {

            EntityCollection ResultColl = null;
            if (!string.IsNullOrEmpty(RegardingId))
            {
                Fetch = string.Format(Fetch, RegardingId);
            }
            if (!string.IsNullOrEmpty(Fetch))
            {
                ResultColl = service.RetrieveMultiple(new FetchExpression(Fetch));
            }
            return ResultColl;
        }

        public List<ActivityPartyObj> PopulateListActivityObj(EntityCollection collection, string LogicalName, IOrganizationService service)
        {
            Itracing.Trace("Inside PopulateListActivityObj");
            List<ActivityPartyObj> outputProcessed = new List<ActivityPartyObj>();
            try
            {
                ActivityPartyObj item = new ActivityPartyObj();
                if (collection != null && collection.Entities.Count > 0)
                {
                    string LogicalSetName = GetLogicalSetName(LogicalName, service);
                    foreach (Entity ent in collection.Entities)
                    {
                        item = new ActivityPartyObj()
                        {
                            LogicalName = ent.LogicalName,
                            LogicalSetName = LogicalSetName,
                            UniqueID = ent.Id
                        };
                        outputProcessed.Add(item);

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return outputProcessed;
        }
        public string GetLogicalSetName(string LogicalName, IOrganizationService service)
        {
            try
            {
                if (!string.IsNullOrEmpty(LogicalName))
                {
                    RetrieveEntityRequest retrieveRecordMetadataRequest = new RetrieveEntityRequest
                    {
                        EntityFilters = EntityFilters.Entity,
                        LogicalName = LogicalName
                    };
                    RetrieveEntityResponse retrieveRecordMetadataResponse = (RetrieveEntityResponse)service.Execute(retrieveRecordMetadataRequest);
                    EntityMetadata recordMetadata = retrieveRecordMetadataResponse.EntityMetadata;

                    return recordMetadata.CollectionSchemaName.ToLower();
                }
                else
                    return "";

            }
            catch (Exception)
            {

                return "Please Enter correct logical name in ActivityPartyEntityLogicalName property";
            }
           
        }

        public List<ActivityPartyObj> PopulateListActivityObj(List<Guid> UniqueIDCollection, string LogicalName, IOrganizationService service)
        {
            List<ActivityPartyObj> outProcessedList = new List<ActivityPartyObj>();
            ActivityPartyObj item = new ActivityPartyObj();
            string LogicalSetName = GetLogicalSetName(LogicalName, service);

            foreach (Guid id in UniqueIDCollection)
            {
                item = new ActivityPartyObj()
                {
                    LogicalName = LogicalName,
                    LogicalSetName = LogicalSetName,
                    UniqueID = id
                };
                outProcessedList.Add(item);

            }
            //recordMetadata.DisplayCollectionName


            return outProcessedList;
        }



        public string ProcessConfigJson(ConfigJson json, IOrganizationService service, string RegardinId)
        {
            Itracing.Trace("Inside processConfigJson");
            var _from = json.From;
            var _to = json.To;
            var _cc = json.CC;
            var _bcc = json.BCC;
            var template = json.Template;
            //from list
            List<ActivityPartyObj> fromactivityPartyObjs = new List<ActivityPartyObj>();
            foreach (var item in _from)
            {
                // output.FROM.list.AddRange()
                
                if (!string.IsNullOrEmpty(item.ActivityPartyRecordId))
                {
                    List<Guid> recordIDlist = new List<Guid>();
                    Guid UniqueId = Guid.Empty;
                    List<string> IDstrings = new List<string>();
                    IDstrings.AddRange(item.ActivityPartyRecordId.Split(','));
                    IDstrings.ForEach(IdString =>
                    {
                        if (Guid.TryParse(IdString, out UniqueId))
                        {
                            recordIDlist.Add(UniqueId);
                        }
                    });
                    fromactivityPartyObjs.AddRange(PopulateListActivityObj(recordIDlist, item.ActivityPartyEntityLogicalName, service));
                }

                else
                {
                    fromactivityPartyObjs.AddRange(PopulateListActivityObj(ExecuteFetchXml(item.FetchXML, service, RegardinId), item.ActivityPartyEntityLogicalName,service));
                }
            }

            //to list
            List<ActivityPartyObj> toactivityPartyObjs = new List<ActivityPartyObj>();
            foreach (var item in _to)
            {
                // output.FROM.list.AddRange()
                if (!string.IsNullOrEmpty(item.ActivityPartyRecordId))
                {
                    List<Guid> recordIDlist = new List<Guid>();
                    Guid UniqueId = Guid.Empty;

                    List<string> IDstrings = new List<string>();
                    IDstrings.AddRange(item.ActivityPartyRecordId.Split(','));
                    IDstrings.ForEach(IdString =>
                    {
                        if (Guid.TryParse(IdString, out UniqueId))
                        {
                            recordIDlist.Add(UniqueId);
                        }
                    });
                    toactivityPartyObjs.AddRange(PopulateListActivityObj(recordIDlist, item.ActivityPartyEntityLogicalName, service));
                }

                else
                {
                    toactivityPartyObjs.AddRange(PopulateListActivityObj(ExecuteFetchXml(item.FetchXML, service,RegardinId), item.ActivityPartyEntityLogicalName, service));
                }
            }
            //cc list
            List<ActivityPartyObj> ccactivityPartyObjs = new List<ActivityPartyObj>();
            foreach (var item in _cc)
            {
                // output.FROM.list.AddRange()
                 if (!string.IsNullOrEmpty(item.ActivityPartyRecordId))
                {
                    List<Guid> recordIDlist = new List<Guid>();
                    Guid UniqueId = Guid.Empty;

                    List<string> IDstrings = new List<string>();
                    IDstrings.AddRange(item.ActivityPartyRecordId.Split(','));
                    IDstrings.ForEach(IdString =>
                    {
                        if (Guid.TryParse(IdString, out UniqueId))
                        {
                            recordIDlist.Add(UniqueId);
                        }
                    });
                    ccactivityPartyObjs.AddRange(PopulateListActivityObj(recordIDlist, item.ActivityPartyEntityLogicalName, service));
                }

                else
                {
                    ccactivityPartyObjs.AddRange(PopulateListActivityObj(ExecuteFetchXml(item.FetchXML, service, RegardinId), item.ActivityPartyEntityLogicalName, service));
                }
            }

            //bcc list
            List<ActivityPartyObj> bccactivityPartyObjs = new List<ActivityPartyObj>();
            foreach (var item in _bcc)
            {
                // output.FROM.list.AddRange()
                  if (!string.IsNullOrEmpty(item.ActivityPartyRecordId))
                {
                    List<Guid> recordIDlist = new List<Guid>();
                    Guid UniqueId = Guid.Empty;

                    List<string> IDstrings = new List<string>();
                    IDstrings.AddRange(item.ActivityPartyRecordId.Split(','));
                    IDstrings.ForEach(IdString =>
                    {
                        if (Guid.TryParse(IdString, out UniqueId))
                        {
                            recordIDlist.Add(UniqueId);
                        }
                    });
                    bccactivityPartyObjs.AddRange(PopulateListActivityObj(recordIDlist, item.ActivityPartyEntityLogicalName, service));
                }

                else
                {
                    bccactivityPartyObjs.AddRange(PopulateListActivityObj(ExecuteFetchXml(item.FetchXML, service, RegardinId), item.ActivityPartyEntityLogicalName, service));
                }
            }


            List<Guid> TemplateIds = new List<Guid>();
            foreach (var id in template)
            {
                Guid itemId = Guid.Empty;
                if (Guid.TryParse(id.TemplateId, out itemId))
                {
                    TemplateIds.Add(itemId);
                }
            }
            FinalOutput output = new FinalOutput()
            {
                TEMPLATE = TemplateIds ,
                FROM =  fromactivityPartyObjs ,
                TO = toactivityPartyObjs ,
                CC = ccactivityPartyObjs ,
                BCC =bccactivityPartyObjs 
            };
            string x = Serializer.SerializeToJson(output);
            Itracing.Trace("x" + x);
            return x;
        }
    }
}
