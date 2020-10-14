using Microsoft.AspNetCore.Mvc;
using Openline.Terra.Api.Context;
using Openline.Terra.Api.Models.Base;
using Openline.Terra.Api.Repository.Base;
using System;
using System.Collections.Generic;

namespace Openline.Terra.Api.Controllers.Base
{
    public class CustomControllerBase<TRepository, TModel> : ControllerBase
        where TRepository : Repository<TModel>
        where TModel : ModelBase
    {
        [HttpGet("GetAll")]
        public virtual ActionResult<IEnumerable<TModel>> GetAll([FromQuery] int? skip, int? take)
        {
            var repository = Activator.CreateInstance<TRepository>();

            var query = repository.GetAll();

            query.OrderBy(x => x.Id, OrderDirection.Desc);

            if (skip.HasValue) query.Skip(skip.Value);
            if (take.HasValue) query.Take(take.Value);

            var retorno = query.Run();

            return Ok(retorno);
        }

        [HttpPost("Insert")]
        public virtual ActionResult Insert([FromBody] TModel entity)
        {
            try
            {
                var repository = Activator.CreateInstance<TRepository>();

                repository.Add(entity);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message} {ex.InnerException?.Message}");
            }
        }
    }
}
