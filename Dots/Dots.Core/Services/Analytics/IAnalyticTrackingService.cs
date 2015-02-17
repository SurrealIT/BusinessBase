using System;

namespace Dots.Core.Services.Analytics
{
    public interface IAnalyticTrackingService
    {
        void SendEvent(string pageName = "", string title = "", string category = "", string action = "",
            string label = "", long value = 0);

        void SendExeception(string description, bool isFatal);
        void SendTiming(TimeSpan timing, string category, string variable, string label);
    }
}