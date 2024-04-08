using servicecontrolhub.monitors.protocol.dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alertbot.rest
{
    public interface IDiagnosticsPresenter
    {
        Task PresentDiagnosicsData(serviceDiagnosticsDto data);
    }
}
