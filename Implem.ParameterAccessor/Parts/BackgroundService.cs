﻿using System.Collections.Generic;

namespace Implem.ParameterAccessor.Parts
{
    public class BackgroundService
    {
        public bool Reminder;
        public int ReminderIgnoreConsecutiveExceptionCount;
        public bool SyncByLdap;
        public List<string> SyncByLdapTime;

        public bool TimerEnabled()
        {
            return SyncByLdap;
        }
    }
}