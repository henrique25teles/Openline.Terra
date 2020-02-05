using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Openline.Terra.Api.Models.Tabela;
using Openline.Terra.Api.Repository;

namespace Openline.Terra.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TabelaController : ControllerBase
    {
        [HttpGet("GetAllTabelas")]
        public ActionResult<IEnumerable<Tabela>> GetAllTabelas()
        {
            var tabelaRepository = new TabelaRepository();

            var retorno = tabelaRepository.GetAll();

            return Ok(retorno);
        }
    }
}