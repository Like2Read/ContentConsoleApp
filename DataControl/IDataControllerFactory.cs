﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataControl
{
    public interface IDataControllerFactory
    {
        IDataController Create(string[] words);
    }
}
