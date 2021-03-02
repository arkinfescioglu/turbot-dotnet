using BackendForest.Common;
using BackendForest.Common.Linq;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using OliWorkshop.Turbo.Abstractions;

namespace OliWorkshop.Turbo.PlugCode
{
    /// <summary>
    /// Delegado que apunta a los metodos o expressiones lambda
    /// que recolectan servicios
    /// </summary>
    /// <param name="instance">Instancia de los objetos que implementa la interfaz</param>
    /// <param name="services">Collector de servicios</param>
    public delegate void InterfaceLoader(object instance, IServiceCollection services);

    /// <summary>
    /// Delegado que apunta a los metodos o expressiones lambda
    /// que recolectan servicios basados en tipos
    /// </summary>
    /// <param name="type"></param>
    /// <param name="services"></param>
    public delegate void TypeLoader(Type type, IServiceCollection services);

    /// <summary>
    /// Delegado encargado de procesar los diagnosticos que devuelven
    /// los distintos plugins
    /// </summary>
    /// <param name="diagnostic"></param>
    public delegate void DiagnosticDelegate(DiagnosticPlugins diagnostic);

    /// <summary>
    /// Clase importante en la infrastructura del sistema para gestion de funcionalidades,
    /// la extension y desacople de los artefactos de implementacion
    /// Esta clase permite mediante a codigo incorporado en distribucion o assembly
    /// posterior desarrollados extender los roles y funciones del sistema
    /// Mediante plugins, artefactos y puntos de extension para proveer activos,
    /// funciones, agregar eventos y nuevas capacidades al sistema
    /// </summary>
    public class PluginLoader
    {
        /// <summary>
        /// Nombre estatico de la carpeta en donde se instalan o se guardan
        /// instancia temporales de ensamblados y otros elementos relacionados con el
        /// contexto central
        /// </summary>
        public const string FolderOfAssemblies = "bf-assets";

        /// <summary>
        /// Carga de los plugins que esta clase ha encontrado
        /// </summary>
        List<Type> PluginAssets { get; } = new List<Type>();

        /// <summary>
        /// Instancias de los plugins
        /// </summary>
        List<IPlugin> Plugins { get; } = new List<IPlugin>();

        /// <summary>
        /// Instancias de los delegados que conforman el API exportable de los plugins
        /// </summary>
        List<Delegate> ApiExports { get; } = new List<Delegate>();

        /// <summary>
        /// Registro de delegados para la configuracion de plugins determiandos
        /// </summary>
        Dictionary<string, Delegate> PluginsConfigure { get; } = new Dictionary<string, Delegate>();

        /// <summary>
        /// Directorio de trabajo
        /// </summary>
        public string WorkingDirectory { get; }

        /// <summary>
        /// Lista de manejadores de diagnosticos
        /// </summary>
        public List<DiagnosticDelegate> HandlerDiagnostics { get; } = new List<DiagnosticDelegate>();

        /// <summary>
        /// Carga un plugin desde un argumento de tipo
        /// </summary>
        /// <typeparam name="TPlugin"></typeparam>
        /// <returns></returns>
        public PluginLoader Load<TPlugin>()
            where TPlugin : IPlugin
        {
            return Load(typeof(TPlugin));
        }

        /// <summary>
        /// Carga un plugin desde un argumento de tipo
        /// </summary>
        /// <typeparam name="TPlugin"></typeparam>
        /// <returns></returns>
        public PluginLoader Load<TPlugin>(Action<TPlugin> action)
            where TPlugin : IPlugin
        {
            return When(action).Load(typeof(TPlugin));
        }

        /// <summary>
        /// Agrega un tipo como plugin
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public PluginLoader Load(Type type)
        {
            PluginAssets.Add(type);
            return this;
        }

        /// <summary>
        /// Agrega un una lista de tipos como plugins
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public PluginLoader Load(IEnumerable<Type> types)
        {
            PluginAssets.AddRange(types);
            return this;
        }

        /// <summary>
        /// Agrega todos los plugins disponibles de el assembly
        /// que pasa como argumento
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public PluginLoader Load(Assembly assembly)
        {
            PluginAssets.AddRange(assembly.FromContract<IPlugin>());
            return this;
        }

        /// <summary>
        /// Agrega todos los plugins disponibles de el assembly
        /// que pasa como argumento y ademas evalua un criterio
        /// de acceptacion
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public PluginLoader Load(Assembly assembly, Predicate<Type> predicate)
        {
            PluginAssets
                .AddRange(
                    assembly
                    .FromContract<IPlugin>()
                    .Where(x => predicate(x))
                );
            return this;
        }

        /// <summary>
        /// Agrega todos los plugins disponibles de el assembly
        /// que carga con la ubicacion que se pasa como argumento
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public PluginLoader Load(string path)
        {
            Load(Assembly.LoadFrom(path));
            return this;
        }

        /// <summary>
        /// Agrega todos los plugins disponibles de el assembly
        /// que carga con la ubicacion que se pasa como argumento
        /// y ademas evalua un criterio
        /// de acceptacion
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public PluginLoader Load(string path, Predicate<Type> predicate)
        {
            Load(Assembly.LoadFrom(path));
            return this;
        }

        /// <summary>
        /// Carga todos los assemblies desde un directorio
        /// </summary>
        /// <param name="path"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public PluginLoader LoadFromDirectory(string path, Predicate<Type> predicate = null)
        {
            var files = Directory.GetFiles(path, ".dll$");
            foreach (string current in files)
            {
                // si el predicado es diferente de null
                // entonces se procede a evaluar dicho predicado
                if (predicate is null)
                {
                    Load(path, predicate);
                }
                else
                {
                    Load(path);
                }
            }
            return this;
        }

        /// <summary>
        /// Register a configuration for a plugin
        /// </summary>
        /// <typeparam name="TPlugin"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public PluginLoader When<TPlugin>(Action<TPlugin> action)
        {
            PluginsConfigure.Add(typeof(TPlugin).FullName, action);
            return this;
        }

        /// <summary>
        /// Conecta un plugin a traves de una interfaz mediante
        /// una accion que defina dicha conexion entre el sistema
        /// y el plugins
        /// </summary>
        /// <typeparam name="TPlugin"></typeparam>
        /// <param name="action"></param>
        public void Plug<TPlugin>(Action<TPlugin> action)
        {
           

            foreach(TPlugin plugin in PluginsWith<TPlugin>())
            {
                try {
                    action(plugin);
                }
                catch (PluginException err)
                {
                   // ignore for moment
                }
            }
        }

        /// <summary>
        /// Este metodo conecta plugins que exporten funciones mediantes delegados
        /// </summary>
        public void PlugApi()
        {
            Plug<IPluginApiProvider>(plugin => ApiExports.AddRange(plugin.ExportApi()));
        }

        /// <summary>
        /// Importa un delegado que esta a disposicion mediante su tipo
        /// </summary>
        /// <param name="delegate"></param>
        public void ImportApi(Delegate @delegate)
        {
            ApiExports.Add(@delegate);
        }

        /// <summary>
        /// Invoca un metodo delegado mediante su tipo
        /// </summary>
        /// <typeparam name="TDelegate"></typeparam>
        /// <returns></returns>
        public TDelegate Use<TDelegate>() where TDelegate: Delegate
        {
            return ApiExports.OfType<TDelegate>().First();
        }

        /// <summary>
        /// Eliminar de la mamoria los plugins de un tipo de interface
        /// </summary>
        /// <typeparam name="TPlugin"></typeparam>
        public void Free<TPlugin>()
        {

        }

        /// <summary>
        /// Eliminar de la memoria un plugins que cumpla con un criterio especifico
        /// </summary>
        /// <typeparam name="TPlugin"></typeparam>
        /// <param name="predicate"></param>
        public void Free<TPlugin>(Predicate<TPlugin> predicate)
        {

        }

        /// <summary>
        /// Este metodo mediante una accion permite interar sobre los plugins disponibles
        /// </summary>
        /// <param name="action"></param>
        public void ProcessInfo(Action<string, string, IDictionary<string, string>> action)
        {
            CheckAvaliable();
            Plugins.ForEach(x => action(x.PluginID, x.PluginType, x.GetInfo()));
        }

        /// <summary>
        /// Ejecuta los diagnosticos establecidos por cada plugins
        /// que implemente el metodo de ejecucion de diagnostico
        /// </summary>
        /// <param name="provider"></param>
        public void RunDiagnostics(IServiceProvider provider)
        {
            CheckAvaliable();
            Plugins.Execute(action: plugin =>
            {
                 try
                 {
                    var diagnostic = plugin.GetEnvoriomentDiagnostic(provider);
                    HandlerDiagnostics
                    .ForEach(handler => handler.Invoke(diagnostic));
                 }
                 catch (NotImplementedException)
                 {
                     // ignore
                 }
            });
        }

        /// <summary>
        /// Agrega un manejador de diagnostico muy basico que 
        /// visualiza los errores y mensajes de advertencia por consola
        /// </summary>
        /// <returns></returns>
        public PluginLoader AddConsoleDiagnostic(bool stopping = false)
        {
            HandlerDiagnostics.Add(d => 
            {

                if (!d.Success)
                {
                    string message = $"The plugin {d.Plugin} diagnostic envorioment is not success";
                    if (stopping)
                    {
                        throw new PluginException(message);
                    }
                    else
                    {
                    }
                }
            });
            return this;
        }

        /// <summary>
        /// Ayuda a conseguir instancias de plugins que son compatibles
        /// con la interfaz que se pasa como argumento
        /// Nota: este metodo realiza un prcoeso de instanciar
        /// todos los plugin activos y pasarlo
        /// a lista de insatncias de plugins
        /// </summary>
        /// <typeparam name="TPlugin"></typeparam>
        /// <returns></returns>
        private IEnumerable<TPlugin> PluginsWith<TPlugin>()
        {
            // cada vez que se encuentren plugins se carga las insatncias
            CheckAvaliable();

            // consula que gestiona los pluigns que se requieren
            return from current in Plugins
                   where current
                   .GetType()
                   .GetInterface(typeof(TPlugin).Name) != null
                   select (TPlugin)current;
        }

        // Checkea si un plugins esta disponible para agregarlos a una lista de instancias
        private void CheckAvaliable()
        {
            if (PluginAssets.Count > 0)
            {
                Plugins.AddRange(
                    PluginAssets
                    .Instance<IPlugin>()
                    .Execute(Configure)
                    );
                PluginAssets.Clear();
            }
        }

        /// <summary>
        /// Administra configuracion mediante las acciones
        /// que se establecieron para determinados plugins
        /// </summary>
        /// <param name="obj"></param>
        private void Configure(IPlugin obj)
        {
            string id = obj.GetType().FullName;
            if (PluginsConfigure.ContainsKey(id))
            {
                PluginsConfigure[id].DynamicInvoke(obj);
            }
        }
    }

    /// <summary>
    /// Estructura de diagnostico de los plugins
    /// </summary>
    public readonly struct DiagnosticPlugins
    {
        public string Plugin { get; }
        public bool Success { get; }
        public List<string> WarningMessages { get; }
        public List<string> ErrorMessages { get; }

        public DiagnosticPlugins(string plugin, bool success)
        {
            Plugin = plugin;
            Success = success;

            WarningMessages = new List<string>();

            ErrorMessages = new List<string>();
        }

    }

    /// <summary>
    /// Contracto primario de los plugins capaces de concectarse al sistema
    /// de Backend Forest
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Identificador categorico del plugins
        /// </summary>
        public string PluginID { get; }

        /// <summary>
        /// Identificador categorico del plugins
        /// </summary>
        public string PluginType { get; }

        /// <summary>
        /// Devuelve infromacion basica del plugins
        /// </summary>
        /// <returns></returns>
        public IDictionary<string, string> GetInfo();

        /// <summary>
        /// Devuelve un diagnositco bien simple acerca del entono
        /// en donde se esta conectado el plugins
        /// </summary>
        /// <param name="provider">
        /// A traves de este argumento se pueden realiza algunas comprobaciones importantes
        /// o casi todas en muchos casos, aunque tambien hay tipos estatico
        /// que proveen infromacion de los entonos que no hace falta pasar 
        /// como argumento
        /// </param>
        /// <returns></returns>
        public DiagnosticPlugins GetEnvoriomentDiagnostic(IServiceProvider provider);

    }

    /// <summary>
    /// Plugin para conectarse con servicios
    /// </summary>
    public interface IPluginService : IPlugin
    {
        /// <summary>
        /// Metodo que configura servicios del sisetma
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureService(IServiceCollection services);

    }

    /// <summary>
    /// Plugin para conectarse con servicios
    /// </summary>
    public interface IPluginApiProvider : IPlugin
    {
        /// <summary>
        /// Metodo que configura servicios del sisetma
        /// </summary>
        /// <param name="services"></param>
        public IEnumerable<Delegate> ExportApi();
    }

    /// <summary>
    /// Tipos de plugins que el proposito de complementar servicios u otros
    /// plugins que cargue el sistema, entonces dicha interfaz permite
    /// que ciertos plugins que la implementen sean capaz de acceder
    /// al <see cref="IServiceProvider"/> para propositos complementarios
    /// </summary>
    public interface IPluginComplementary
    {
        /// <summary>
        /// Metodo al se le sirve el provedor de infrastructura de los servicios
        /// del sistema
        /// </summary>
        /// <param name="provider"></param>
        public void OnServices(IServiceProvider provider);
    }
}
