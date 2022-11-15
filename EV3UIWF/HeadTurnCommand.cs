using System;
using System.Linq;

namespace EV3UIWF
{
    public class HeadTurnCommand
    {
        private readonly int _degree; //in centimeters
        private readonly int _torque; //number in % from 1 to 100, positive - forward, negative backwards

        public HeadTurnCommand(int degree, int torque)
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
            var bytesDegree = BitConverter.GetBytes(_degree);
            var bytesTorque = BitConverter.GetBytes(_torque);

            return bytesDegree.Concat(bytesTorque.ToArray()).ToArray();
        }
    }
}