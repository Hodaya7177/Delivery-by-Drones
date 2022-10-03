using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public enum WeightCategories { Heavy, Medium, Light }
    public enum Priorities { Regular, Rapid, Emergency }
    public enum DroneStatus { Maintenance, Available, Delivery, Assigned }
    public enum ParcelStatus { Requested, Assigned, PickedUp, Delivered }
}
