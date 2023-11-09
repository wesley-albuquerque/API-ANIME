using API_ANIME.Model;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;

namespace API_ANIME.Controllers
{
    [ApiController]
    [Route("api-anime")]
    public class APIController : ControllerBase
    {
        private readonly string StrinConnection = @"Server = localhost;
                                                Database=animes;
                                                User=wesley;
                                                Password=waa123";
        public MySqlConnection Conexao { get; set; }
        public string Sql { get; set; }
        public MySqlCommand Cmd { get; set; }
        public APIController()
        {
            Conexao = new MySqlConnection(StrinConnection);
        }

        [HttpPost("insere-{tabela}")]
        public IActionResult insereTabelasAuxiliares(string tabela, [FromBody] TabelaAuxiliares tabelaAuxiliar)
        {
            try
            {
                Conexao.Open();
                Sql = $"INSERT INTO {tabela} (nome) VALUES (@nome)";

                Cmd = new MySqlCommand(Sql, Conexao);
                Cmd.Parameters.AddWithValue("@nome", tabelaAuxiliar.Nome);

                Cmd.ExecuteNonQuery();

                return Ok($"{tabela} cadastrado com sucesso");
            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao inserir dados: {ex.Message}");
            }
        }

        [HttpGet("consulta-{tabela}/{id?}")]
        public IActionResult consultaTabelaAuxiliares(string tabela, int? id)
        {
            try
            {
                Conexao.Open();

                if (id.HasValue)
                {
                    Sql = $"SELECT * from {tabela} WHERE id = @id";
                    Cmd = new MySqlCommand(Sql, Conexao);
                    Cmd.Parameters.AddWithValue("@id", id.Value);
                }
                else
                {
                    Sql = $"SELECT * from {tabela}";
                    Cmd = new MySqlCommand(Sql, Conexao);
                }

                MySqlDataReader objeto = Cmd.ExecuteReader();

                List<TabelaAuxiliares> registros = new List<TabelaAuxiliares>();

                while (objeto.Read())
                {
                    TabelaAuxiliares registro = new TabelaAuxiliares
                    {
                        Id = int.Parse(objeto["id"].ToString()),
                        Nome = objeto["nome"].ToString()
                    };
                    registros.Add(registro);
                }
                if (registros.Count > 0)
                {
                    string jsonRegistros = JsonConvert.SerializeObject(registros);
                    return Ok(jsonRegistros);
                }
                else
                {
                    return BadRequest($"Nenhum {tabela} cadastrado");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Erro ao inserir dados: {ex.Message}");
            }
        }

        [HttpPost("atualiza-{tabela}")]
        public IActionResult atualizaTabelaAuxiliares(string tabela, [FromBody] TabelaAuxiliares tabelaAuxiliares)
        {
            try
            {
                Conexao.Open();
                Sql = $"SELECT * FROM {tabela} WHERE nome = '{tabelaAuxiliares.Nome}'";
                Cmd = new MySqlCommand(Sql, Conexao);
                MySqlDataReader objeto = Cmd.ExecuteReader();
                if (objeto.Read())
                {
                    return BadRequest($"{tabelaAuxiliares.Nome} já cadastrado no id {objeto["id"]}");
                }
                else
                {
                    Conexao.Close();
                    Conexao.Open();
                    Sql = $"UPDATE {tabela} set nome = @nome WHERE id =" + tabelaAuxiliares.Id;
                    Cmd = new MySqlCommand(Sql, Conexao);
                    Cmd.Parameters.AddWithValue("@nome", tabelaAuxiliares.Nome);
                    Cmd.ExecuteNonQuery();
                    Conexao.Close();
                    return Ok($"{tabela} atualizado com sucesso");
                }

                

            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao atualizar gênero" + ex);
            }
        }

        [HttpDelete("delete-{tabela}/{id}")]
        public IActionResult deleteTabelaAuxiliares(string tabela, int id)
        {
            try
            {
                Conexao.Open();
                Sql = $"DELETE FROM {tabela} WHERE id = {id}";
                Cmd = new MySqlCommand(Sql, Conexao);
                if (Cmd.ExecuteNonQuery() > 0)
                {
                    return Ok($"{tabela} excluído com sucesso");
                }
                else
                {
                    return BadRequest($"Id({id}) não encontrado no banco de dados");
                }
            }
            catch(Exception ex)
            {
                return BadRequest($"Erro ao excluir {tabela}: " + ex);
            }
            

        }



    }
}