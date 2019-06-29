using System;
namespace Timer_namespace
{
     //[Serializable]
    public class Timer
    {

        public float period;
        public float tAt;
        public bool paused;
        public Timer(float period)
        {
            this.period = period;
            tAt = -1;
            paused = false;
        }

        public void turnOn() {
            tAt = 0;
        }

        public bool isOn()
        {
            return (tAt >= 0);
        }
        public void turnOff()
        {
            tAt = -1;
        }

        public float getCanoncial() {
            float result = 0;
            result = tAt / period;
            if(result > 1.0f)
            {
                result = 1.0f;
            }

            return result;
        }
        public bool updateTimer(float dt) {
            bool result = false;
            tAt += dt;
            if(tAt >= period) {
                tAt = period;
                result = true;
            }
            return result;

        }
    }
}
