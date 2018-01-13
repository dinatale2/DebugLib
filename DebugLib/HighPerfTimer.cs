using System;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace DebugLib
{
    public class HiPerfTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(
            out long lpFrequency);

        private long startTime, stopTime;
        private long freq;

        // Constructor

        public HiPerfTimer()
        {
            startTime = 0;
            stopTime = 0;

            if (!QueryPerformanceFrequency(out freq))
            {
                // high-performance counter not supported
                throw new Win32Exception();
            }
        }

        // Start the timer
        public void Start()
        {
            QueryPerformanceCounter(out startTime);
        }

        // Stop the timer
        public void Stop()
        {
            QueryPerformanceCounter(out stopTime);
        }

        // Returns the duration of the timer (in seconds)
        public double Duration
        {
            get
            {
                return (double)(stopTime - startTime) / (double)freq;
            }
        }
    }
}