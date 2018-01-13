using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace DebugLib
{
    public class RuntimeDiag : IDisposable
    {
        private HiPerfTimer m_Timer;
        protected bool m_bIsRunning;
        private bool m_bShowAllDiags;
        protected string m_strFuncName;
        protected LogFile m_LogFile;

        private double m_dMin;
        private double m_dMax;
        private double m_dAverage;
        int nCount;

        private bool m_bDisposed;

        public RuntimeDiag(string strFuncName, LogFile logFile, bool bPrintAllDiags = true)
        {
            m_strFuncName = strFuncName;
            m_bShowAllDiags = bPrintAllDiags;
            m_LogFile = logFile;
            m_bIsRunning = false;

            m_dMin = double.MaxValue;
            m_dMax = 0.0;
            nCount = 0;

            m_Timer = new HiPerfTimer();

            m_bDisposed = false;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool bDisposing)
        {
            if (!m_bDisposed)
            {
                if (bDisposing)
                {
                    Log(nCount > 0,
                      "Statistics for {0} - Average Runtime: {1:N6} ms, Min Runtime {2:N6} ms, Max Runtime {3:N6} ms", m_strFuncName, m_dAverage, m_dMin, m_dMax);
                    m_bDisposed = true;
                }
            }
        }

        ~RuntimeDiag()
        {
            Dispose(false);
        }

        protected void Log(bool bCondition, string strToLog, params object[] parms)
        {
            Debug.WriteLineIf(bCondition, string.Format(strToLog, parms));
            if (m_LogFile != null && bCondition)
            {
                m_LogFile.Log(string.Format(strToLog, parms));
            }
        }

        public void StartTiming()
        {
            m_Timer.Start();
            m_bIsRunning = true;
        }

        protected double GetElapsedTime()
        {
            m_Timer.Stop();
            double dElapsedTime = m_Timer.Duration * (double)1000;

            if (m_dMin > dElapsedTime)
                m_dMin = dElapsedTime;

            if (m_dMax < dElapsedTime)
                m_dMax = dElapsedTime;

            m_dAverage = (((double)nCount * m_dAverage) + dElapsedTime) / (nCount + 1);
            nCount++;

            return dElapsedTime;
        }

        public virtual void EndTiming()
        {
            if (m_bIsRunning)
            {
                double dElapsedTime = GetElapsedTime();

                Log(m_bShowAllDiags, "Elapsed Time of {0}: {1} ms", m_strFuncName, dElapsedTime);
                m_bIsRunning = false;
            }
        }
    }
}
