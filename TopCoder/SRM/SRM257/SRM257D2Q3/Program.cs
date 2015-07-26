using System;
using System.Collections;
using System.Text;

public class TimeCard
{
    public string leave(string[] time)
    {
        int[] times = new int[time.Length];
        for (int i=0; i < times.Length; i++) 
        {
            string[] splits = time[i].Split(':', ',');
            int hour = int.Parse(splits[0]);
            if (hour==12)
                hour -=12;
            int min = int.Parse(splits[1]);
            if (splits[2] == "pm")
                hour += 12;
            times[i] = hour*60+min;
        }
        int duration = 0;
        for (int i=0; i < times.Length-1; i+=2) {
            int dif = times[i + 1] - times[i];
            if (dif < 0)
                dif += 24*60;
            duration += dif;
        }
        if (duration > 40 * 60)
            return "ABOVE 40";
        int maxLeft = 20*60;
        int needed = 40*60 - duration;
        if (maxLeft < 40 * 60 - duration)
        {
            return "BELOW 40";
        }
        int clockout = times[times.Length - 1] + needed;
        if (clockout >= 24 * 60)
            clockout -= 24 * 60;
        int outhours = clockout / 60;
        int outmins = clockout % 60;
        bool am = false;
        if (outhours < 12)
        {
            am = true;
        }
        if (outhours > 12)
            outhours -= 12;
        if (outhours == 0)
            outhours += 12;
        return outhours.ToString("00") + ":" + outmins.ToString("00") + "," + (am ? "am" : "pm");
    }
}

namespace SRM257D2Q3
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
