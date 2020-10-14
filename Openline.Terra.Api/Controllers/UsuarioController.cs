using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Openline.Terra.Api.Controllers.Base;
using Openline.Terra.Api.Models;
using Openline.Terra.Api.Repository;
using Openline.Terra.Api.Repository.Base;

namespace Openline.Terra.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : CustomControllerBase<Repository<Usuario>, Usuario>
    {
        
    }
}