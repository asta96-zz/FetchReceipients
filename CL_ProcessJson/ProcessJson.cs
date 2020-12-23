using CL_ProcessJson.BusinessLogic;
using CL_ProcessJson.Modal.Json2;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Workflow;
using System.Activities;
using System.ServiceModel;
using System;
namespace CL_ProcessJson
{
    public class ProcessJson : CodeActivity
    {
        public IOrganizationService _service;
        public IOrganizationServiceFactory _serviceFactory;
        public IWorkflowContext _context;
        [RequiredArgument]
        [Input("Name")]
        public InArgument<string> Name { get; set; }
        [Input("RegardingID")]
        public InArgument<string> RegardingID { get; set; }
        [RequiredArgument]
        [Output("OutputJsonConfig")]
        public OutArgument<string> OutputJsonConfig { get; set; }
        public ITracingService _tracing;
        protected override void Execute(CodeActivityContext context)
        {
            try
            {
           
            _context = context.GetExtension<IWorkflowContext>();
                
            _serviceFactory = context.GetExtension<IOrganizationServiceFactory>();
            _service = _serviceFactory.CreateOrganizationService(_context.UserId);
            _tracing = context.GetExtension<ITracingService>();
                BLogic logic = new BLogic(_tracing);
                _tracing.Trace("before fetchingConfigsetting");
                string ConfigValue = logic.FetchConfigSetting(_service, this.Name.Get(context));
                _tracing.Trace("after fetchingConfigsetting");
                //var stream = logic.ConvertStringToStream(ConfigValue);
                ConfigJson ComposeIncident = Serializer.DeserializeFromJson<ConfigJson>(ConfigValue);//BLogic.Deserialize<Rootobject>(ConfigValue);                
                string FinalOutput = logic.ProcessConfigJson(ComposeIncident, _service,RegardingID.Get(context));                
                _tracing.Trace("FinalOutput:"+FinalOutput);
              OutputJsonConfig.Set(context, FinalOutput);

            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException(OperationStatus.Failed,ex.Message);
            }
        }
    }
}
