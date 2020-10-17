using Microsoft.AspNetCore.Mvc;
using Openline.Terra.Api.Context;
using Openline.Terra.Api.Models.Base;
using Openline.Terra.Api.Repository.Base;
using System;
using System.Collections.Generic;

namespace Openline.Terra.Api.Controllers.Base
{
    public class CustomControllerBaseUnidade<TRepository, TModel> : ControllerBase
        where TRepository : RepositoryUnidade<TModel>
        where TModel : ModelBaseUnidade
    {
        [HttpGet("GetAll")]
        public virtual ActionResult GetAll([FromQuery] int empresaId, int unidadeId, int? skip, int? take)
        {
            try
            {
                if (empresaId == 0) return BadRequest("Empresa id informado não pode ser zero");
                if (empresaId == 0) return BadRequest("Unidade id informado não pode ser zero");
                if (skip == 0 || take == 0) return BadRequest("skip ou take informado não pode ser zero");

                var repository = Activator.CreateInstance<TRepository>();

                var query = repository.GetAll(empresaId, unidadeId);

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
            catch (Exception ex)
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

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

        [HttpGet("Get")]
        public virtual ActionResult Get([FromQuery] int empresaId, int unidadeId, int? id)
        {
            try
            {
                if (empresaId == 0) return BadRequest("Empresa id informado não pode ser zero");
                if (unidadeId == 0) return BadRequest("Unidade id informado não pode ser zero");
                if (id == 0) return BadRequest("Id informado não pode ser zero");

                var repository = Activator.CreateInstance<TRepository>();

                var entity = repository.Get(id, empresaId, unidadeId);

                return Ok(entity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }
        }

    }

}
