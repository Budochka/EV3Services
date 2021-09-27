using System;
using System.Runtime.InteropServices;
using System.ServiceModel.Activation;

namespace EV3UI
{
    public class MoveCommand
    {
        private readonly int _distance; //in centimeters
        private readonly int _torque; //number in % from 1 to 100, positive - forward, negative backwards

        public MoveCommand(int distance, int torque)
        {
            _distance = distance;
            _torque = torque;
            if (Math.Abs(_torque) > 100)
            {
                throw new Exception("Invalid torque value");
            }
        }

        public byte[] ToByte()
        {
            int size = Marshal.SizeOf(this);
            byte[] arr = new byte[size];

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }
    }

    public class TurnCommand
    {
        private readonly int _degree; //in centimeters
        private readonly int _torque; //number in % from 1 to 100, positive - forward, negative backwards

        public TurnCommand(int degree, int torque)
        {
            _degree = degree;
            _torque = torque;
            if (Math.Abs(_torque) > 100)
            {
                throw new Exception("Invalid torque value");
            }
        }

        public byte[] ToByte()
        {
            int size = Marshal.SizeOf(this);
            byte[] arr = new byte[size];

            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);

            return arr;
        }
    }
}
