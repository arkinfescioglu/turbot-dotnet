using System;
using System.Collections.Generic;

namespace OliWorkshop.Turbo.PlugCode
{
    public class PluginCollection<TContainer, TPlugin> : Dictionary<string, TPlugin>
        where TPlugin : ServicesConfiguration<TContainer>
    {
    }
}
