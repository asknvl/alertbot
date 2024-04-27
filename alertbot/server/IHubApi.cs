using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alertbot.server
{
    public interface IHubApi
    {
        Task<int> KeepAliveRequest();
    }
}
