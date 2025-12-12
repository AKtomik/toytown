using UnityEngine;

public class SetSpeed : MonoBehaviour
{
        public void StopSpeed()
        {
            Settings.SpeedUp = 0;
        }
        public void NormaleSpeed()
        {
            Settings.SpeedUp = 1;
        }

        public void DoubleSpeed()
        {
            Settings.SpeedUp = 2;
        }
        public void TripleSpeed()
        {
            Settings.SpeedUp = 3;
        }

        public void FiveSpeed()
        {
            Settings.SpeedUp = 5;
        }

        public void TenSpeed()
        {
            Settings.SpeedUp = 10;
        }
}
