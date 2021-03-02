using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace OliWorkshop.Turbo.PlugCode
{
    /// <summary>
    /// Extensiones del core de BackendForest
    /// </summary>
    public static class CoreExtensions
    {
        /// <summary>
        /// Permite acceder directamente a traves del <see cref="IServiceProvider"/>
        /// al cargador de plugins que este registrado en el sistema
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static PluginLoader GetPluginLoader(this IServiceProvider provider)
        {
            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            return provider.GetService<PluginLoader>();
        }

        /// <summary>
        /// Metodo de extension que ayuda a conectar plugins complementarios basados
        /// en un provedor de servicios
        /// </summary>
        /// <param name="loader"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static PluginLoader PlugComplementaryPlugins(this PluginLoader loader, IServiceProvider provider)
        {
            if (loader is null)
            {
                throw new ArgumentNullException(nameof(loader));
            }

            if (provider is null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            // conecta los plugins complementarios y les pasa el provedor de servicios
            // que se pasa como argumento
            loader.Plug<IPluginComplementary>( p => p.OnServices(provider));
            return loader;
        }
    }
}
