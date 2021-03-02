using System;
using System.Collections.Generic;
using System.Text;

namespace OliWorkshop.Turbo.PlugCode
{
    public abstract class PluginSystem<TContainer, TPlugin>
        where TPlugin : ServicesConfiguration<TContainer>
    {
        public PluginSystem(PluginCollection<TContainer, TPlugin> plugins)
        {
            Plugins = plugins ?? throw new ArgumentNullException(nameof(plugins));
        }

        public PluginCollection<TContainer, TPlugin> Plugins { get; }

        protected abstract void Configure(TContainer container, TPlugin plugin);
    }
}
