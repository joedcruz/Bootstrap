using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bootstrap
{
    public interface IWebMenuService
    {
        Task<string> GetWebMenu();
    }
}
