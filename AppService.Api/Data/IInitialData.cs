using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppService.Api.Data
{
    public interface IInitialData
    {
        Type EntityType { get; }

        IEnumerable<object> GetData();
    }
}
