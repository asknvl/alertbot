using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alertbot.notifier
{
    public interface INotifier
    {
        Task AlertNotify(string message);
    }
}
