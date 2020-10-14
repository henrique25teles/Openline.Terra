﻿using Microsoft.AspNetCore.Mvc;
using Openline.Terra.Api.Context;
using Openline.Terra.Api.Models.Base;
using Openline.Terra.Api.Repository.Base;
using System;
using System.Collections.Generic;

namespace Openline.Terra.Api.Controllers.Base
{
    public class CustomControllerBaseEmpresa<TRepository, TModel> : ControllerBase
        where TRepository : RepositoryEmpresa<TModel>
        where TModel : ModelBaseEmpresa
    {
        [HttpGet("GetAll")]
        public virtual ActionResult<IEnumerable<TModel>> GetAll([FromQuery] int empresaId, int? skip, int? take)
        {
            var repository = Activator.CreateInstance<TRepository>();

            var query = repository.GetAll(empresaId);

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
