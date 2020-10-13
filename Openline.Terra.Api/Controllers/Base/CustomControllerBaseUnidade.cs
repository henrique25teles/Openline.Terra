using Microsoft.AspNetCore.Mvc;
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
        public virtual ActionResult<IEnumerable<TModel>> GetAll()
        {
            var repository = Activator.CreateInstance<TRepository>();

            var query = repository.GetAll(1, 1);

            var retorno = query.Run();

            return Ok(retorno);
        }
    }
}
