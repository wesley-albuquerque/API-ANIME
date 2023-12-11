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
                                                Database=personal_site;
                                                User=wesley;
                                                Password=waa123";
        public MySqlConnection Conexao { get; set; }
        public string Sql { get; set; }
        public MySqlCommand Cmd { get; set; }
        public Avaliacao Avaliacao { get; set; }
        public APIController()
        {
            Conexao = new MySqlConnection(StrinConnection);
        }

        [HttpPost("insere-{tabela}")]
        public IActionResult insereTabelasAuxiliares(string tabela, [FromBody] TabelaAuxiliares tabelaAuxiliares)
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
                    Sql = $"INSERT INTO {tabela} (nome) VALUES (@nome)";

                    Cmd = new MySqlCommand(Sql, Conexao);
                    Cmd.Parameters.AddWithValue("@nome", tabelaAuxiliares.Nome);

                    Cmd.ExecuteNonQuery();
                    Conexao.Close();

                    return Ok($"{tabela} cadastrado com sucesso");
                }

            }
            catch (Exception ex)
            {
                Conexao.Close();
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
                    Conexao.Close();
                    string jsonRegistros = JsonConvert.SerializeObject(registros);
                    return Ok(jsonRegistros);
                }
                else
                {
                    Conexao.Close();
                    return BadRequest($"Nenhum {tabela} cadastrado");
                }

            }
            catch (Exception ex)
            {
                Conexao.Close();
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
                    Conexao.Close();
                    return Ok($"{tabela} excluído com sucesso");
                }
                else
                {
                    Conexao.Close();
                    return BadRequest($"Id({id}) não encontrado no banco de dados");
                }
            }
            catch (Exception ex)
            {
                Conexao.Close();
                return BadRequest($"Erro ao excluir {tabela}: " + ex);
            }
        }

        [HttpPost("insere-animes")]
        public IActionResult InsereNovaAvaliacao([FromBody] AvaliacaoVO avaliacao)
        {
            try
            {
                Avaliacao = new Avaliacao(Conexao);
                var registrosDropado = Avaliacao.BuscaAvalDropado(avaliacao);
                if (registrosDropado != null)
                {
                    return BadRequest($"Cadastro não permitido. Anime {registrosDropado} é marcado como dropado!");
                }
                var registrosDuplicado = Avaliacao.BuscaAvalDuplicada(avaliacao);
                if (registrosDuplicado != null)
                {
                    return BadRequest($"Cadastro não permitido. A temporada {registrosDuplicado.temporada_aval} do anime {registrosDuplicado.nome_anime} já avaliada anteriormente");
                }
                Conexao.Open();
                Sql = $"INSERT INTO animes_avaliacao ({string.Join(", ", typeof(AvaliacaoVO).GetProperties().Select(propriedade => propriedade.Name))}) " +
                      $"VALUES ({string.Join(", ", typeof(AvaliacaoVO).GetProperties().Select(propriedade => $"@{propriedade.Name}"))})";

                Cmd = new MySqlCommand(Sql, Conexao);
                foreach (var propriedade in typeof(AvaliacaoVO).GetProperties())
                {
                    var value = propriedade.GetValue(avaliacao);
                    Cmd.Parameters.AddWithValue($"@{propriedade.Name}", value ?? null);
                }

                Cmd.ExecuteNonQuery();
                Conexao.Close();

                return Ok($"Avaliação cadastrado com sucesso");
            }
            catch (Exception ex)
            {
                Conexao.Close();
                return BadRequest($"Erro ao inserir dados: {ex.Message}");
            }
        }
    }
}