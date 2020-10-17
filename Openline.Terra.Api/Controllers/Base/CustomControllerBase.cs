using Microsoft.AspNetCore.Mvc;
using Openline.Terra.Api.Context;
using Openline.Terra.Api.Models.Base;
using Openline.Terra.Api.Repository.Base;
using System;

namespace Openline.Terra.Api.Controllers.Base
{
    public class CustomControllerBase<TRepository, TModel> : ControllerBase
        where TRepository : Repository<TModel>
        where TModel : ModelBase
    {
        [HttpGet("GetAll")]
        public virtual ActionResult GetAll([FromQuery] int? skip, int? take)
        {
            try
            {
                if (skip == 0 || take == 0) return BadRequest("skip ou take informado não pode ser zero");

                var repository = Activator.CreateInstance<TRepository>();

                var query = repository.GetAll();

                query.OrderBy(x => x.Id, OrderDirection.Desc);

                if (skip.HasValue) query.Skip(skip.Value);
                if (take.HasValue) query.Take(take.Value);

                var lista = query.Run();
                var count = query.Count();

                if (count == 0) return NoContent();

                return Ok(new 
                { 
                    count,
                    lista
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPost("Insert")]
        public virtual ActionResult Insert([FromBody] TModel entity)
        {
            try
            {
                var repository = Activator.CreateInstance<TRepository>();

                repository.Add(entity);

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("Get")]
        public virtual ActionResult Get([FromQuery] int? id)
        {
            try
            {
                if (id == 0) return BadRequest("Id informado não pode ser zero");

                var repository = Activator.CreateInstance<TRepository>();

                var entity = repository.Get(id);

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpPut("Update")]
        public virtual ActionResult Update([FromBody] TModel entity)
        {
            throw new NotImplementedException();
        }

        [HttpDelete("Delete")]
        public virtual ActionResult Delete([FromQuery] int? id)
        {
            throw new NotImplementedException();
        }
    }
}
