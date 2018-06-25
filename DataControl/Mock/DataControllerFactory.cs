using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataControl.Mock
{
    public class DataControllerFactory : IDataControllerFactory
    {
        public IDataController Create(string[] words)
        {
            return new DataController(new MoqWordsRepository(words));
        }
    }
}
