using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlApi
{
    public static class BlFactory
    {
        /// <summary>
        /// creates and returns an instance of BlObject
        /// </summary>
        public static IBL GetBl()
        {
            return BL.BlObject.Instance;
        }
    }
}
