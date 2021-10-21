using System;
using System.Linq;

namespace EV3UIWF
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
            var bytesDist = BitConverter.GetBytes(_distance);
            var bytesTorque = BitConverter.GetBytes(_torque);

            return bytesDist.Concat(bytesTorque.ToArray()).ToArray();
        }
    }
}
