using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DO
{
    public struct Drone
    {
        public int Id { get; set; }
        public string Model { get; set; }
        public WeightCategories MaxWeight { get; set; }
        public bool isActive { get; set; }

        public override string ToString()
        {
            return string.Format("Id is: {0}\nmodel: {1}\nmax weight is: " +
                " {4}\n", Id, Model, MaxWeight);
        }
    }
}

