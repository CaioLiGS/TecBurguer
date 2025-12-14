
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TecBurguer.Models;

namespace TecBurguer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly DBTecBurguerContext _context;

        public UsuariosController(DBTecBurguerContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios
        [HttpGet("Listar")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            if (_context.Usuarios == null)
            {
                return NotFound();
            }
            return await _context.Usuarios.ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            if (_context.Usuarios == null)
            {
                return NotFound();
            }
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                return NotFound();
            }

            return usuario;
        }

        // PUT: api/Usuarios/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPatch("update/{id}")]
        public async Task<IActionResult> PatchUsuario(int id, [FromBody] Usuario dadosParciais)
        {
            // 1. Encontrar o usuário existente no banco
            var usuarioDoBanco = await _context.Usuarios.FindAsync(id);
            if (usuarioDoBanco == null)
            {
                return NotFound();
            }

            // 2. Aplicar manualmente as atualizações parciais
            //    Isso garante que só mudamos o que veio na requisição (ex: o CEP)
            //    e não apagamos os outros campos (Nome, Email) para null.

            if (dadosParciais.Nome != null)
            {
                usuarioDoBanco.Nome = dadosParciais.Nome;
            }
            if (dadosParciais.Email != null)
            {
                usuarioDoBanco.Email = dadosParciais.Email;
            }
            if (dadosParciais.Cep != null)
            {
                usuarioDoBanco.Cep = dadosParciais.Cep;
            }
            if (dadosParciais.Estado != null)
            {
                usuarioDoBanco.Estado = dadosParciais.Estado;
            }
            if (dadosParciais.Cidade != null)
            {
                usuarioDoBanco.Cidade = dadosParciais.Cidade;
            }
            if (dadosParciais.Bairro != null)
            {
                usuarioDoBanco.Bairro = dadosParciais.Bairro;
            }
            if (dadosParciais.Rua != null)
            {
                usuarioDoBanco.Rua = dadosParciais.Rua;
            }
            if (dadosParciais.Complemento != null)
            {
                usuarioDoBanco.Complemento = dadosParciais.Complemento;
            }
            if (dadosParciais.Servico != null)
            {
                usuarioDoBanco.Servico = dadosParciais.Servico;
            }
            // OBS: Não atualize Senha ou NivelAcesso assim, a menos que seja intencional.

            // 3. Marcar a entidade como modificada e salvar
            //    (Opcional, pois o EF Core geralmente detecta mudanças, mas é uma boa prática)
            _context.Entry(usuarioDoBanco).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Usuarios
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            if (_context.Usuarios == null)
            {
                return Problem("Entity set 'DBTecBurguerContext.Usuarios'  is null.");
            }
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsuario", new { id = usuario.IdUsuario }, usuario);
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            if (_context.Usuarios == null)
            {
                return NotFound();
            }
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return (_context.Usuarios?.Any(e => e.IdUsuario == id)).GetValueOrDefault();
        }
    }
}
