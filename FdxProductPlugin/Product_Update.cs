using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace FdxProductPlugin
{
    public class Product_Update : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            //Extract the tracing service for use in debugging sandboxed plug-ins....
            ITracingService tracingService =
                (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            //Obtain execution contest from the service provider....
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            int step = 0;

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity productEntity = (Entity)context.InputParameters["Target"];

                if (productEntity.LogicalName != "product")
                    return;

                Entity oppProduct = new Entity();
                EntityCollection oppProducts = new EntityCollection();
                Guid oppProductId = Guid.Empty;

                try
                {
                    step = 1;
                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                    //Get current user information.....
                    step = 2;
                    WhoAmIResponse response = (WhoAmIResponse)service.Execute(new WhoAmIRequest());

                    //Retrieve Opportunity products from Product....
                    step = 3;
                    if (productEntity.Attributes.Contains("fdx_revenuetype"))
                    {
                        oppProducts = CRMQueryExpression.getOpportunityProducts(productEntity.Id, service);
                        step = 4;
                        foreach(Entity oppProd in oppProducts.Entities)
                        {
                            step = 5;
                            oppProductId = new Guid(((AliasedValue)oppProd.Attributes["opportunityId"]).Value.ToString());

                            step = 6;
                            oppProduct = new Entity
                            {
                                Id = oppProductId,
                                LogicalName = "opportunityproduct"
                            };

                            step = 7;
                            oppProduct.Attributes["fdx_revenuetype"] = productEntity.Attributes["fdx_revenuetype"];
                            step = 8;
                            service.Update(oppProduct);
                        }
                    }
                }
                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException(string.Format("An error occurred in the Product_Update plug-in at Step {0}.", step), ex);
                }
                catch (Exception ex)
                {
                    tracingService.Trace("Product_Update: step {0}, {1}", step, ex.ToString());
                    throw;
                }

            }
        }
    }
}
