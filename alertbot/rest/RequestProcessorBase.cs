using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace servicecontrolhub.rest
{
    public interface IRequestProcessor
    {
        Task<(HttpStatusCode, string)> ProcessRequestData(string data);
    }
}
