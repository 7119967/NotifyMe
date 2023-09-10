﻿using NotifyMe.Core.Entities;
using NotifyMe.Core.Interfaces.Repositories;
using NotifyMe.Infrastructure.Context;

namespace NotifyMe.Infrastructure.Repositories
{
    public class ConfigurationRepository: Repository<Configuration>, IConfigurationRepository
    {
        public ConfigurationRepository(DatabaseContext context) : base(context)
        {
        }
        }
}
