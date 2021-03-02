using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BackendForest.Common.Linq
{
    /// <summary>
    /// Extensiones para gestion de tipo mediante un assably y escanear classes
    /// Util para importar clases y tipos en tiempo de ejcucion atravez de la carga de assemblies
    /// mediante la API que ofrece .NET para los esamblados
    /// </summary>
    public static class AssemblyExtension
    {
        /// <summary>
        /// Encuentra todos los tipos del assambly que posean una determinada interfaz
        /// </summary>
        /// <typeparam name="T">Importante, se debe pasar una 
        /// interfaz y esta sera el criterio para encontrar los tipos</typeparam>
        /// <param name="assambly">extexsionde assembly</param>
        /// <returns>Un enumerable de tipos</returns>
        public static IEnumerable<Type> FromContract<T>(this Assembly assambly) {
            Type contract = typeof(T);
            if (!contract.IsInterface)
            {
                throw new ArgumentException("The type argument T should be interface");
            }
            return assambly.GetExportedTypes().Where( x =>
                x.IsClass && !(x.GetInterface(contract.Name) is null)            
            );
        }

        /// <summary>
        /// Importante, este metodo ayuda a recuperar los tipos de un assembly
        /// que implementan una interfaz y ademas ejecuta una accion sobre ellos
        /// </summary>
        /// <typeparam name="T">Interfaz a encontrar, pero nota que si la clase
        /// requiere argumentos en el constrcutor no a ser encontrada</typeparam>
        /// <param name="assambly">extension de assembly</param>
        /// <param name="action">acciona ejecutar despues que se hayan creado instancias</param>
        public static void CreateInstances<T>(this Assembly assambly, Action<T> action)
        {
            Type contract = typeof(T);
            if (!contract.IsInterface)
            {
                throw new ArgumentException("The type argument T should be interface");
            }

            // se necesita una consulta que finalmente trasforma los tipos en instancias
            var results = assambly
                .GetExportedTypes()
                .Where(x =>
                    x.IsClass && !(x.GetInterface(contract.Name) is null) && !(x.GetConstructor(Type.EmptyTypes) is null)
                )
                .Select(x =>  // aqui es donde se intancia la clase mediante la invocacion del constrcutor
                    x.GetConstructor(Type.EmptyTypes)
                     .Invoke(null));

            foreach (T instance in results)
            {
                action(instance);
            }
        }

        /// <summary>
        /// Encuentra todos los tipos del assambly que posean una determinada interfaz
        /// y ademas usa un criterio adicional de filtrado
        /// </summary>
        /// <typeparam name="T">Importante, se debe pasar una 
        /// interfaz y esta sera el criterio para encontrar los tipos</typeparam>
        /// <param name="assambly">extexsionde assembly</param>
        /// <returns>un enumerable de tipos</returns>
        public static IEnumerable<Type> FromContract<T>(this Assembly assambly, Func<Type, bool> filter)
        {
            Type contract = typeof(T);
            if (!contract.IsInterface)
            {
                throw new ArgumentException("The type argument T should be interface");
            }
            return assambly.GetExportedTypes().Where(x =>
               x.IsClass && !(x.GetInterface(contract.Name) is null) && filter(x)
            );
        }

        /// <summary>
        /// Filtrar tipos que implemente una determinada interfaz
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="types"></param>
        /// <returns></returns>
        public static IEnumerable<Type> WithContract<T>(this IEnumerable<Type> types)
        {
            return types.Where(x => !(x.GetInterface(typeof(T).Name) is null));
        }

        /// <summary>
        /// Importante este metodo ayuda a filtrar 
        /// por tipos que posean un constructor compatible
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="types"></param>
        /// <param name="typesTraget"></param>
        /// <returns></returns>
        public static IEnumerable<Type> WithConstruct(this IEnumerable<Type> types, Type[] typesTraget)
        {
            return types
                .Where(x => !(x.GetConstructor(typesTraget) is null));
        }


        /// <summary>
        /// Ayuda a encontrar constrcutores de tipos pasados como argumentos
        /// </summary>
        /// <param name="types"></param>
        /// <param name="typesTraget"></param>
        /// <param name="all"></param>
        /// <returns></returns>
        public static IEnumerable<ConstructorInfo> SelectConstruct(this IEnumerable<Type> types, Type[] typesTraget, bool all = false)
        {
            return types
                .Select(x => x.GetConstructor(typesTraget))
                .Where(x => !(x is null) && (x.IsPublic||all));
        }

        /// <summary>
        /// Importante este metodo ayuda a filtrar de una manera directa por un atributo
        /// </summary>
        /// <typeparam name="T">Hay una restricion en este typeparam
        /// para que los tipos hereden de attributos</typeparam>
        /// <param name="types">extension de enumerables con con Type como argumento
        /// no de cualquier enumerable</param>
        /// <returns>devuelve</returns>
        public static IEnumerable<Type> WithAttribute<T>(this IEnumerable<Type> types)
            where T: Attribute
        {
            return types.Where(x => Attribute.IsDefined(x, typeof(T)));
        }

        /// <summary>
        /// Simple herlper para filtar un enumrable de tipo atravez de un argumento de tipo 
        /// </summary>
        /// <typeparam name="T">Tipo por el cual filtrar</typeparam>
        /// <param name="types">se extiende los enumerables de tipos</param>
        /// <returns></returns>
        public static IEnumerable<Type> WhereIs<T>(this IEnumerable<Type> types)
        {
            return types.Where(x =>
            {
                Type currentType = typeof(T);
                return x.Equals(currentType);
            });
        }

        /// <summary>
        /// Este metodo ayuda a filtrar por classes que pueden ser instanceables
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public static IEnumerable<Type> CanBeIntanceable(this IEnumerable<Type> types)
        {
            return types.Where(x => !x.IsAbstract && x.IsVisible);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static object[] Instance(this IEnumerable<Type> types, params object[] parameters)
        {

            return types
                .SelectConstruct(
                    parameters.Select(x => x.GetType()).ToArray()
                 )
                .Select(x => x.Invoke(parameters))
                .ToArray();
        }

        /// <summary>
        /// Este metodo de extension agrega una muy interesante funcionalidad
        /// para filtrar tipos compatible con el arguemto de tipo que se pase
        /// y crear insatncia de dichos tipos a travez de parametros
        /// Para asegurar y ser coherente el metodo encuentra constructores
        /// compatibles con los tipos de los parametros
        /// de no conocer bien los tipos que se pretenden instanciar
        /// el metodo no resolvera ninguna insatncia pero no lanza excepciones
        /// </summary>
        /// <param name="types"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static TContract[] Instance<TContract>(this IEnumerable<Type> types, params object[] parameters)
        {

            return types
                .WithContract<TContract>() // se filtra por los tipos que son compatibles con TContract
                .SelectConstruct(  // lo primero es conseguir los constrcutores
                    parameters.Select(x => x.GetType()).ToArray() // se sacan los tiposde los parametros para encontrar al constructor correspondiente
                 )
                .Select(x => (TContract)x.Invoke(parameters.Length < 1 ? null : parameters)) // se utiliza el operador de trasformacion donde se seleciona las insatncia
                .ToArray();
        }

        public static TContract[] Instance<TContract>(this IEnumerable<Type> types, IServiceProvider service)
        {

            var query = types
                .WhereIs<TContract>() // se filtra por los tipos que son compatibles con TContract
                .Select(
                    x => x
                    .GetConstructors()
                    .Where(
                        y => y
                        .IsPublic && y
                        .GetParameters().Length > 1)
                    .Single()
                ).Select(x => (TContract) x.InvokeWithProvider(service));
            return query.ToArray();
        }


        /// <summary>
        /// Invoka un metodo o constructor sirviendo los parametros con el provedor
        /// de serviciosq que se pase como argumento
        /// </summary>
        /// <param name="method"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static object InvokeWithProvider(this MethodBase method, IServiceProvider provider)
        {
            return method.Invoke(null, provider
                .GetArrayServices(
                    method
                    .GetParameters()
                    .Select(X => X.ParameterType)
                    .ToArray()
                    )
                );
        }

        /// <summary>
        /// Invoka un metodo o constructor sirviendo los parametros con el provedor
        /// de serviciosq que se pase como argumento y adcionalmente se espera
        /// que dicho metodo tenga una instancia, en este caso hay que pasar la instancia
        /// </summary>
        /// <param name="method"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static object InvokeWithProvider(this MethodBase method, object instance, IServiceProvider provider)
        {
            return method.Invoke(instance, provider
                .GetArrayServices(
                    method
                    .GetParameters()
                    .Select(X => X.ParameterType)
                    .ToArray()
                    )
                );
        }

        /// <summary>
        /// Resuelve los parametros de un delgado a traves de un provedor de servicios
        /// </summary>
        /// <param name="delg"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static object InvokeWithProvider(this Delegate delg, IServiceProvider provider)
        {
            var parameters = delg
                .GetMethodInfo()
                .GetParameters()
                .Select(x => x.ParameterType)
                .ToArray();
            return delg.DynamicInvoke(provider.GetArrayServices(parameters));
        }

        /// <summary>
        /// Esta clase resuelve un arreglo servicios atraves de un arreglo de tipos
        /// </summary>
        /// <param name="provider">Extiende IServiceProvider</param>
        /// <param name="types">Arreglo de Tipos</param>
        /// <returns></returns>
        public static object InjectService(this IServiceProvider provider, Type target)
        {
            var ctor = target
                 .GetConstructors().Where(x => x.GetParameters().Count() > 0)
                 .First();
            if (ctor is null)
            {
                return target.GetConstructor(Type.EmptyTypes).Invoke(null);
            }
            return ctor.InvokeWithProvider(provider);

        }

        /// <summary>
        /// Esta clase resuelve un arreglo servicios atraves de un arreglo de tipos
        /// </summary>
        /// <param name="provider">Extiende IServiceProvider</param>
        /// <param name="types">Arreglo de Tipos</param>
        /// <returns></returns>
        public static object[] GetArrayServices(this IServiceProvider provider, Type[] types)
        {
            return types.Select(x => provider.GetService(x)).ToArray();
        }
    }
}
