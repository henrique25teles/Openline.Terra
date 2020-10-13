using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Openline.Terra.Api.Context;
using Openline.Terra.Api.Controllers.Base;
using Openline.Terra.Api.Models;
using Openline.Terra.Api.Repository;
using Openline.Terra.Api.Repository.Base;

namespace Openline.Terra.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaidaController : CustomControllerBaseUnidade<RepositoryUnidade<Saidas>, Saidas>
    {
        [HttpGet("GetAllSaidas")]
        public override ActionResult<IEnumerable<Saidas>> GetAll()
        {
            return base.GetAll();
        }

        //[HttpGet("GetAllSaidas")]
        //public ActionResult<IEnumerable<Saidas>> GetAllSaidas()
        //{
        //    var saidaRepository = new SaidaRepository();

        //    var query = saidaRepository.GetAll(1, 1);

        //    query.Where(x => x.ValorTotal, TipoCriterio.MaiorOuIgual, "100");
        //    query.Skip(50);
        //    query.Take(120);

        //    var retorno = query.Run();

        //    return Ok(retorno);
        //}
    }
}