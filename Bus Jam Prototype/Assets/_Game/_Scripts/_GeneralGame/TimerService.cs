using System;
using System.Globalization;
using UnityEngine;

namespace _Game._Scripts._GeneralGame
{
    public static class TimerService
    {
        private static readonly string _constraintTimerKey = "constraint_timer";
        private static readonly float _constraintTimerDuration = 5f; // minutes
        
        public static bool CanPlayGame(out TimeSpan timeSpan)
        {
            if (!PlayerPrefs.HasKey(_constraintTimerKey))
            {
                timeSpan = StartFreshConstraintTimer();
                return false;
            }
            if (IsConstraintTimerExpired())
            {
                timeSpan = TimeSpan.Zero;
                return true;
            }
            
            timeSpan = GetRemainingConstraintTime();
            return false;
            
        }
        
        public static TimeSpan StartFreshConstraintTimer()
        {
            DateTime constraintExpirationTime = DateTime.Now.AddMinutes(_constraintTimerDuration);
            PlayerPrefs.SetString(_constraintTimerKey, 
                constraintExpirationTime.ToString(CultureInfo.InvariantCulture));
            return TimeSpan.FromMinutes(_constraintTimerDuration);
        }
        
        public static TimeSpan GetRemainingConstraintTime()
        {
            if (!PlayerPrefs.HasKey(_constraintTimerKey))
            {
                return StartFreshConstraintTimer();
            }
            
            DateTime expirationTime = DateTime.Parse(PlayerPrefs.GetString(_constraintTimerKey));
            return expirationTime - DateTime.Now;
        }
        
        private static bool IsConstraintTimerExpired()
        {
            DateTime expirationTime = DateTime.Parse(PlayerPrefs.GetString(_constraintTimerKey));
            return expirationTime < DateTime.Now;
        }
        
        public static void AddRemoveTimeToConstraintTimer(float seconds)
        {
            DateTime expirationTime = DateTime.Parse(PlayerPrefs.GetString(_constraintTimerKey));
            expirationTime = expirationTime.AddSeconds(seconds);
            PlayerPrefs.SetString(_constraintTimerKey, expirationTime.ToString(CultureInfo.InvariantCulture));
        }
    }
}