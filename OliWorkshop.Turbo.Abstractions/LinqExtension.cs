using System;
using System.Linq;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;

namespace OliWorkshop.Turbo.Abstractions
{
    /// <summary>
    /// Extensiones comunes para todo proyecto basadas en Linq
    /// que ayudan a la semticidad del codigo, la fluidez y la lucidez
    /// de la codificacion
    /// Ayuda a la reutilizacion y hacer operaciones bien recurrentes
    /// para mejor la arquitectura y codificacion del sistema
    /// </summary>
    public static class LinqExtension
    {
        /// <summary>
        /// Delegado que apunta a metodos y expreisones lambdas capaces
        /// que ser manejas procesos de reducion
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TR"></typeparam>
        /// <param name="value"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public delegate TR Reducer<TValue, TR>(TValue value, TR result);

        /// <summary>
        /// Delegado que apunta a metodos y expreisones lambdas capaces
        /// de generar valores al azar
        /// </summary>
        /// <typeparam name="TResult">Tipo del resultado genrado</typeparam>
        /// <returns></returns>
        public delegate TResult Seeder<TResult>();

        /// <summary>
        /// Agrega una clasura where si se pasa un valor true como condicion
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="condition"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IQueryable<T> FilterIf<T>(this IQueryable<T> query, bool condition, Predicate<T> predicate)
        {
            if (condition)
            {
                return query.Where(x => predicate.Invoke(x));
            }
            else
            {
                return query;
            }
        }

        /// <summary>
        /// Metodo de extension para diccionario
        /// que posean una lista como valor
        /// Este metodo ayuda a introducir valores
        /// a la lista de los diccionario de forma mas directa
        /// y facil
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="X"></typeparam>
        /// <param name="keyValuePairs"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IDictionary<X, IList<T>> AddToList<T, X>(
            this IDictionary<X, IList<T>> keyValuePairs,
            X key,
            T value )
        {
            // de debe comprobar que exista la llave
            if (keyValuePairs.ContainsKey(key))
            {
                keyValuePairs.Add(key, new List<T> { value });
            }
            else
            {
                keyValuePairs[key].Add(value);
            }
            return keyValuePairs;
        }

        /// <summary>
        /// Ayuda a trabajar con diccionarios de listas
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="X"></typeparam>
        /// <param name="dic"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Dictionary<X,List<T>> AddToList<T, X>(
            this Dictionary<X, List<T>> dic,
            X key,
            T value)
        {
            dic.AddToList(key, value);
            return dic;
        }

            /// <summary>
            /// Simple iteracion para los diccionarios
            /// que posean lista como valores
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <typeparam name="X"></typeparam>
            /// <param name="keyValuePairs"></param>
            /// <param name="key"></param>
            /// <param name="action"></param>
            /// <returns></returns>
            public static IDictionary<X, IList<T>> ForEach<T, X>(
            this IDictionary<X, IList<T>> keyValuePairs,
             X key,
             Action<T> action)
        {
            foreach (T item in keyValuePairs[key])
            {
                action(item);
            }
            return keyValuePairs;
        }

        /// <summary>
        /// Este curioso metodo ayuda a cuidar la fluidez de la consultas
        /// mediante una ingeniosa tecnica
        /// En concreto permite hacer where anidados pasado primeramente
        /// una funcion que recibe como parametro el valor objetivo de la consulta
        /// y dicha funcion devolvera algun valor de la propiedad, campo o metodo
        /// que despues podra ser pasado por otro criterio que se encargue
        /// de filtarlo finalmente para el resultado de la consulta engenrar
        /// </summary>
        /// <typeparam name="T">Objetivo de la consulta</typeparam>
        /// <typeparam name="X">Valor de campo, propiedad o retorno de un  metodo de T</typeparam>
        /// <param name="enumerable">Extiende el IEnumerable</param>
        /// <param name="func">Funcion para buscar un valor de T</param>
        /// <param name="criteria">Criterio de consulta</param>
        /// <returns></returns>
        public static IEnumerable<T> In<T, X>(this IEnumerable<T> enumerable, Func<T, X> func, Func<X, bool> criteria)
        {
            return enumerable.Where(x => criteria(func(x)));
        }

        /// <summary>
        /// Lanza una excepcion que se le pasa como arguemnto de tipo
        /// si se encuentra al menos un elemento que coincida con el predicado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TErr"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="criteria"></param>
        /// <returns></returns>
        public static IEnumerable<T> ThrowIf<T, TErr>(this IEnumerable<T> enumerable, Func<T, bool> criteria)
        where TErr: Exception, new()
        {
            var subq = enumerable.Where(x => criteria(x)).Count();
            if (subq > 0)
            {
                throw new TErr();
            }
            return enumerable;
        }

        /// <summary>
        /// Lanza una excepcion que se establece como mediante una accion
        /// si se encuentra al menos un elemento que coincida con el predicado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="criteria"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<T> ThrowIf<T>(this IEnumerable<T> enumerable, Func<T, bool> criteria, Action action)
        {
            var subq = enumerable.Where(x => criteria(x)).Count();
            if (subq > 0)
            {
                action();
            }
            return enumerable;
        }

        /// <summary>
        /// Lanza una excepcion que se establece como mediante una accion
        /// si se encuentra al menos un elemento que coincida con el predicado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="criteria"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static IEnumerable<T> ThrowIf<T>(this IEnumerable<T> enumerable, Func<T, bool> criteria, Action<T> action)
        {
            var subq = enumerable
                .Where(x => criteria(x))
                .Count();

            if (subq > 0)
            {
                action(
                     enumerable
                    .Where(x => criteria(x))
                    .First()
                );
            }
            return enumerable;
        }

        /// <summary>
        /// Ayuda a filtrar por varios criterios end ependecia del tipo
        /// que evalue el criterio
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
       /*public static IEnumerable<T> WhereCast<T, X>
            (this IEnumerable<T> enumerable, Predicate<X> predicate)
        {
            
            return enumerable.Where(x => (x) );
        }*/

        /// <summary>
        /// Ayuda a saber si una instancia de delegado es un predicate
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static bool IsPredicate(this Delegate x)
        {
            MethodInfo methodInfo = x.GetMethodInfo();
            return methodInfo.GetParameters().Length == 1 
                &&
                methodInfo.ReturnParameter.ParameterType.Name == nameof(Boolean);
        }

        /// <summary>
        /// Meto de ayuda privado que permite saber si un Tipo se le puede pasar
        /// como primer parametro a un delegado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x"></param>
        /// <returns></returns>
        private static bool CastParameter<T>(Delegate x)
        {
            var parameter = x.GetMethodInfo()
                .GetParameters()
                .First()
                .ParameterType;
            Type target = typeof(T);
            return target.Equals(parameter) || parameter.IsSubclassOf(target) || parameter.IsAssignableFrom(target);
        }

        /// <summary>
        /// Soporte del where clasico de linq pero
        /// para consultas predicados asincronos que
        /// devuelven una tarea
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IEnumerable<T> WhereAsync<T>(this IEnumerable<T> enumerable, Func<T, Task<bool>> predicate)
        {
            return enumerable.Where(x => {
                Task<bool> result = predicate(x);
                result.Wait();
                return result.Result;
            });
        }

        /// <summary>
        /// Mediante un criterio evalua que elementos se deben eliminar en una lista
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        public static void Delete<T>(this IList<T> list, Predicate<T> predicate)
        {
            list
                .Where((x) => predicate(x))
                .Execute(x => list.Remove(x));
        }

        /// <summary>
        /// Iteracion sencilla sobre un numero de elementos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="action"></param>
        public static IEnumerable<T> Execute<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T item in enumerable)
            {
                action(item);
            }
            return enumerable;
        }

        /// <summary>
        /// Crea un diccionario a partir de un enumerable 
        /// tomando una propiedad, campo o valor de metodo
        /// como la llave del diccionario
        /// Nota: sebe tomar en el predicado un valor que se entienda unico sino falla
        /// la creacion del diccionario
        /// </summary>
        /// <typeparam name="X">Tipo inferido por la devolucion del predicado
        /// que establece la llave</typeparam>
        /// <typeparam name="T">Objetivo de la consulta</typeparam>
        /// <param name="query">Extender IEnumerable</param>
        /// <param name="predicate">Funcion que define la llave de cada valor</param>
        /// <returns></returns>
        public static IDictionary<X, T> ToDictionary<X, T>(this IEnumerable<T> query, Func<T, X> predicate)
        {
            var result =  new Dictionary<X, T>();
            foreach (T item in query)
            {
                /// aqui lo se hace es simple para convertir un resultado enumerable de ona consulta
                /// a un diccionario, lo hace es definir en el predicado cual es la propiedad,
                /// campo o valor derivado del objetivo que va a ser la llave del diccionario
                /// entonces procedemos a iterar sobre los resultados agregandolos al diccionario
                /// y estableciendo como su llave el valor correspondiente
                /// luego el compilador prodra inferir el tipado del diccionario
                result.Add(predicate(item), item);
            }
            return result;
        }

        /// <summary>
        /// Es similar a IDictionary con una diferencia sutil
        /// los valores por llave pueden estar repetidos y en vez
        /// de asiciar una llave con un valor se asocia con una lista de valores
        /// Es un metodo util para estructurar listas clasificadas y colleciones
        /// bien complejas
        /// </summary>
        /// <typeparam name="X"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IDictionary<X, IList<T>> ToDictionaryOfLists<X, T>(this IEnumerable<T> query, Func<T, X> predicate)
        {
            var result = new Dictionary<X, IList<T>>();
            foreach (T item in query)
            {
                // se toma referencia d ela llave para no repetir la llamada
                X key = predicate(item);
                /// hay detalles de esta implementacion se comentan en el body
                /// de ToDictionary
                /// Mientras que hay un cambio importante, se espera que el predicado mas
                /// que devolver una llave devuelva un valor que
                /// clasifique dentro de otra collecion
                /// los resultados objetivos
                if (result.ContainsKey(key))
                {
                    result[key].Add(item);
                }
                else
                {
                    result.Add(predicate(item), new List<T> { item });
                }
            }
            return result;
        }

        /// <summary>
        /// Este metodo de ayuda para los tipos string es util para obtener
        /// a partir de cadenas con cierto patron de formato
        /// un diccionario de llave valor
        /// tipo un archivo .ini simple
        /// o el siguiente formato de ejemplo: key: value, key2: value2
        /// formato para el cual se puede 
        /// efectuar una llamada asi: raw.ParseToKeyPairs(',',':') o
        /// para archvos con formatos de configuracion: raw.ParseToKeyPairs('\n','=') 
        /// </summary>
        /// <param name="str">Se extiende los tipos cadenas</param>
        /// <param name="seg">token que separa lo segmentos</param>
        /// <param name="pairs">token que separa las llaves y los valores</param>
        /// <returns></returns>
        public static Dictionary<string, string> ParseKeyPairs(this string str, char seg, char pairs)
        {
            var result =  new Dictionary<string, string>();
            
            // se itera sobre cada trozo divido por el primer token
            foreach (string item in str.Split(seg))
            {
                // se divide entonces cada trozo obtenido del segumento
                string[] chunks = item.Split(pairs);

                /// se comprueba que solo haya dos string tras la division
                if (chunks.Length != 2)
                {
                    /// importante es como emitir un error de sintaxis
                    throw new Exception("Bad error to parse of pairs separator token");
                }

                /// se agrega el resultado de la division del separador de los pares
                /// llave y valor
                result.Add(chunks[0], chunks[1]);
            }
            return result;
        }

        /// <summary>
        /// Este metodo sirve para generar un valor escalar a partir del resultado de
        /// una consulta
        /// </summary>
        /// <typeparam name="TCurrent"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="reducer"></param>
        /// <param name="initial"></param>
        /// <returns></returns>
        public static TResult Reduce<TCurrent, TResult>(
            this IEnumerable<TCurrent> enumerable,
            Reducer<TCurrent, TResult> reducer,
            TResult initial
            )
        {
            TResult result = initial;
            foreach (TCurrent current in enumerable)
            {
                result = reducer(current, result);
            }
            return result;
        }

        /// <summary>
        /// Este metodo de extension se agrega a todos los objetos de la biblioteca base .Net
        /// y devuelve true si el valor de la instancia aparece en alguno de los valores 
        /// pasados como argumento
        /// En fin simplifica condicionales de este tipo: 
        /// manera estandar => if(value == 3 || value == 5 || value == 7)
        /// con este metodo => if(value.IsHere(3, 5, 7))
        /// </summary>
        /// <param name="element"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsHere(this object element, params object[] values)
        {
            return values.Contains(element);
        }

        /// <summary>
        /// Utilidad simple que ayuda a cualquier objeto
        /// a devolver el valor de una propiedad si se conoce su
        /// nombre de propiedad
        /// </summary>
        /// <param name="value"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetPropertyValue(this object value, string key)
        {
            return value.GetType().GetProperty(key).GetValue(value);
        }

        /// <summary>
        /// Version de IsHere pero en un IQueryable
        /// es decir agrega un filtro IsHere a la consulta
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static IQueryable<T> WhereIn<T>(this IQueryable<T> element, params object[] values)
        {
            return element.Where(x => values.Contains(x));
        }

        /// <summary>
        /// Nombre mas semantico para Substring
        /// </summary>
        /// <param name="str"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static string TakeUntil(this string str, int limit, int offset = 0)
        {
            return string.IsNullOrEmpty(str) ? str : str.Substring(offset, limit);
        }

        /// <summary>
        /// Este metodo es un simpel helper que ayuda
        /// a generar sequencia tanto al azar o basadas en
        /// alguna clase de patron atravez de un delgado o 
        /// expresion lambda que se le pase como argumento
        /// </summary>
        /// <typeparam name="TCurrent"></typeparam>
        /// <param name="count"></param>
        /// <param name="seeder"></param>
        /// <returns></returns>
        public static TCurrent[] Generate<TCurrent>(
            int count,
             Seeder<TCurrent> seeder)
        {
            // se genra un arreglo con la dimesion requerida por el parametro
            TCurrent[] currents = new TCurrent[count];

            /// luego se itera sobre el arreglo completo
            /// para que el delago genere los valores
            for (int i = 0; i < count; i++)
            {
                currents[i] = seeder();
            }
            return currents;
        }
    }
}
