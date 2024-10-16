using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace team_project
{
    public class EventAggregator
    {
        public event Action UserUpdated;

        public void OnUserUpdated()
        {
            UserUpdated?.Invoke();
        }
    }
}
