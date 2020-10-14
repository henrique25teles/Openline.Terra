using Microsoft.AspNetCore.Mvc;
using Openline.Terra.Api.Controllers.Base;
using Openline.Terra.Api.Models;
using Openline.Terra.Api.Repository.Base;

namespace Openline.Terra.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaidaController : CustomControllerBaseUnidade<RepositoryUnidade<Saidas>, Saidas>
    {
        
    }
}