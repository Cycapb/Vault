using System.Web.Mvc;
using Vault.Infrastructure.Filters;

namespace Vault
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new VaultErrorAttribute());
        }
    }
}