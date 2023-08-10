﻿using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.Infrastructure.Repositories
{
    public class EventMonitoringRepository : Repository<EventMonitoring>, IEventMonitoringRepository
    {
        public EventMonitoringRepository(DatabaseContext context) : base(context)
        {
        }
    }
}
