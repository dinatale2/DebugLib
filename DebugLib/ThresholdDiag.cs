using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DebugLib
{
    public class ThresholdDiag : RuntimeDiag
    {
        private double m_dThreshold;

        public ThresholdDiag(string strFuncName, LogFile logFile, double dThreshold) : base(strFuncName, logFile, false)
        {
            m_dThreshold = dThreshold;
        }

        public override void EndTiming()
        {
            if (m_bIsRunning)
            {
                double dElapsedTime = GetElapsedTime();

                Log(dElapsedTime >= m_dThreshold, "Threshold broken with elapsed time of {0}: {1} ms", m_strFuncName, dElapsedTime);
                m_bIsRunning = false;
            }
        }
    }
}
