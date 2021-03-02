using Microsoft.AspNetCore.Mvc;
using OliWorkshop.Turbo.Data;
using System;
using System.Threading.Tasks;

namespace PremiumTesh.TwitterNotifier.Support
{
    /// <summary>
    /// El controlador generico sirve para implementar un api rest
    /// de las entidades que se necesiten mediante el protocolo http
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    [ApiController]
    public class FromDatabaseAttribute<TEntity, TId> : ControllerBase
        where TEntity : class, IBaseEntity<TId>
    {
        public FromDatabaseAttribute(IRepository<TEntity> repository)
        {
            Repository = repository;
        }

        public IRepository<TEntity> Repository { get; set; }

        /// <summary>
        /// Devuelve un listado de todos los registros de datos
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            return Ok(await Repository.FindAll());
        }

        /// <summary>
        /// Devueleve un listado filtrado de registros de datos
        /// </summary>
        /// <param name="page"></param>
        /// <param name="length"></param>
        /// <param name="search"></param>
        /// <param name="sort"></param>
        /// <param name="sortType"></param>
        /// <returns></returns>
        [HttpGet("page/{page}")]
        public async Task<IActionResult> GetAsync([FromRoute] int page, int length)
        {
            return Ok(await Repository.Find(page, length));
        }

        /// <summary>
        /// Devuelve un unico registro basado en su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<TEntity>> GetByIdAsync([FromRoute] ulong id)
        {
            return Ok(await Repository.FindOne(id));
        }

        /// <summary>
        /// Agrega un registro con datos que entran en el cuerpo de la peticion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] TEntity data)
        {
            try
            {
                var result = await Repository.StoreAnsyc(data);
                return StatusCode(201, new
                {
                    data.Id,
                    data.AtCtreated
                });
            }
            catch (Exception)
            {
                // ignore for now
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Actualiza un registro con datos que entran en el cuerpo de la peticion
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<NoContentResult> UpdateAsync([FromBody] TEntity data)
        {
            try
            {
                await Repository.UpdateAsync(data);
            }
            catch (Exception)
            {
                // ignore for now
            }
            return NoContent();
        }

        /// <summary>
        /// Elimina un registro basado en su id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<NoContentResult> DeleteAsync([FromRoute] ulong id)
        {
            try
            {
                await Repository.DeleteAsync(id);
            }
            catch (Exception)
            {
                // ignore for now
            }
            return NoContent();
        }
    }
}