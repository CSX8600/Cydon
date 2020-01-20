using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cydon.Data.Base
{
    public abstract class BaseCache
    {
        public abstract string Name { get; }
        internal abstract void Update();
    }
}
