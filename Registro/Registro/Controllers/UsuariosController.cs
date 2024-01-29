using Microsoft.AspNetCore.Mvc;
using Registro.Models;
using System.Data;
using System.Data.SqlClient;

namespace Registro.Controllers
{
    public class UsuariosController : Controller
    {
        public IActionResult Index()
        {
            using (SqlConnection con = new(Configuration["ConnectionStrings:conexion"]))
            {
                using (SqlCommand cmd = new("SP_USUARIOS", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    con.Open();
                    SqlDataAdapter da = new(cmd);
                    DataTable dt = new();
                    da.Fill(dt);
                    da.Dispose();
                    List<UsuarioModel> lista = new();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        lista.Add(new UsuarioModel()
                        {
                            IdUsuario = Convert.ToInt32(dt.Rows[i][0]),
                            Nombre = (dt.Rows[i][1]).ToString(),
                            Edad = Convert.ToInt32(dt.Rows[i][2]),
                            Correo = (dt.Rows[i][3]).ToString()
                        });
                    }
                    ViewBag.Usuarios = lista;
                    con.Close();
                }
                return View();

            }
        }

        public IConfiguration Configuration { get;}

        public UsuariosController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IActionResult Registrar() { return View(); }


        [HttpPost]
        public IActionResult Registrar(UsuarioModel usuario)
        {
            using (SqlConnection con= new(Configuration["ConnectionStrings:conexion"]))
            {
                using (SqlCommand cmd = new("SP_REGISTRAR", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Nombre", System.Data.SqlDbType.VarChar).Value = usuario.Nombre;
                    cmd.Parameters.Add("@Edad", System.Data.SqlDbType.Int).Value = usuario.Edad;
                    cmd.Parameters.Add("@Correo", System.Data.SqlDbType.VarChar).Value = usuario.Correo;
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            
            return Redirect("Index");

        }
    }
}
