﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Openline.Terra.Api.Models;
using Openline.Terra.Api.Repository;
using Openline.Terra.Api.Repository.Base;

namespace Openline.Terra.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        [HttpPost("GetAllUsuarios")]
        public ActionResult<IEnumerable<Usuario>> GetAllUsuarios()
        {
            var usuarioRepository = new UsuarioRepository();

            var retorno = usuarioRepository.GetAll();

            var repo = new Repository<Usuario>();


            return Ok(retorno);
        }
    }
}