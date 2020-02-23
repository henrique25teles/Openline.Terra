using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Openline.Terra.Api.Models;
using Openline.Terra.Api.Repository;

namespace Openline.Terra.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaidaController : ControllerBase
    {

        [HttpGet("GetAllSaidas")]
        public ActionResult<IEnumerable<Saidas>> GetAllUsuarios()
        {
            var saidaRepository = new SaidaRepository();

            var retorno = saidaRepository.GetAll(1, 1);

            return Ok(retorno);
        }
    }
}