using alertbot.rest;
using Newtonsoft.Json;
using servicecontrolhub.monitors.protocol.dtos;
using servicecontrolhub.rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace aviatorbot.rest
{
    public class PresentDiagnosticsRequestProcessor : IRequestProcessor, IPresentDiagnosticsObservable
    {
        #region vars
        List<IDiagnosticsPresenter> diagnosticPresenters = new List<IDiagnosticsPresenter>();

        public void Add(IDiagnosticsPresenter presenter)
        {
            if (!diagnosticPresenters.Contains(presenter))
                diagnosticPresenters.Add(presenter);
        }
        public void Remove(IDiagnosticsPresenter resulter)
        {
            diagnosticPresenters.Remove(resulter);
        }
        #endregion

        public async Task<(HttpStatusCode, string)> ProcessRequestData(string data)
        {

            HttpStatusCode code = HttpStatusCode.BadRequest;            

            try
            {

                var diagnosticsData = JsonConvert.DeserializeObject<serviceDiagnosticsDto>(data);

                code = HttpStatusCode.NotFound;

                foreach (var dp in diagnosticPresenters)
                {
                    try
                    {
                        await dp.PresentDiagnosicsData(diagnosticsData);
                        code = HttpStatusCode.OK;

                    } catch (Exception ex)
                    {

                    }
                }

            } catch (Exception ex)
            {
                code = HttpStatusCode.InternalServerError;
            }

            return (code, code.ToString());
        }

        public async Task<(HttpStatusCode, string)> ProcessRequest()
        {
            HttpStatusCode code = HttpStatusCode.NotFound;
            string responseText = code.ToString();            
            return (code, responseText);
        }
    }
}
