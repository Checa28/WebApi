using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using Microsoft.EntityFrameworkCore;
using webapi.Custom;
using webapi.Models;
using webapi.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccesoController : Controller
    {
        private readonly WebapiContext _webapiContext;
        private readonly Utils _utils;
        public AccesoController(WebapiContext webapiContext, Utils utils)
        {
            _webapiContext = webapiContext;
            _utils = utils;
        }

        [HttpPost]
        [Route("Registrarse")]
        public async Task<IActionResult> Registrarse(UsuarioDTO objeto)
        {
            var modeloUsuario = new Usuario
        {
            Nombre = objeto.Nombre,
            Correo = objeto.Correo,
            Clave = _utils.encriptarSHA256(objeto.Clave)
        };

        await _webapiContext.Usuarios.AddAsync(modeloUsuario);
        await _webapiContext.SaveChangesAsync();

            if(modeloUsuario.IdUsuario != 0) 
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            else
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO objeto)
        {
            var usuarioEncontrado = await _webapiContext.Usuarios
            .Where(u =>
                u.Correo == objeto.Correo &&
                u.Clave == _utils.encriptarSHA256(objeto.Clave)
             ).FirstOrDefaultAsync();

            if (usuarioEncontrado == null)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "" });
            else
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token = _utils.generarJWT(usuarioEncontrado) });
        }
    }
}
