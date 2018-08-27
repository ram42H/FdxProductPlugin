using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdxProductPlugin
{
    public class CRMQueryExpression
    {
        public static EntityCollection getOpportunityProducts(Guid _productId, IOrganizationService _service)
        {
            FetchExpression query;
            EntityCollection oppProducts = new EntityCollection();
            string OppProductQuery = "<fetch top='5000' ><entity name='product' ><attribute name='fdx_revenuetype' /><attribute name='productid' /><attribute name='name' /><filter type='and' ><condition attribute='productid' operator='eq' value='{0}' /></filter><link-entity name='opportunityproduct' from='productid' to='productid' link-type='inner' ><attribute name='fdx_nrr' /><attribute name='fdx_mrr' /><attribute name='opportunityproductid' alias='opportunityId'/><attribute name='fdx_revenuetype' /><link-entity name='opportunity' from='opportunityid' to='opportunityid' link-type='inner' ><attribute name='statecode' /><attribute name='name' /><filter type='and' ><condition attribute='statecode' operator='eq' value='0' /></filter></link-entity></link-entity></entity></fetch>";

            query = new FetchExpression(string.Format(OppProductQuery, _productId));
            oppProducts = _service.RetrieveMultiple(query);

            return oppProducts;
        }
    }
}
