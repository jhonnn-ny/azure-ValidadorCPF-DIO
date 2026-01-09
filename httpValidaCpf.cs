using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace httpValidaCpf
{
    public class CpfRequest
    {
        public string Cpf { get; set; }
    }

    public class CpfResponse
    {
        public bool Valido { get; set; }
        public string Cpf { get; set; }
        public string Mensagem { get; set; }
    }

    public static class HttpValidaCpf
    {
        [FunctionName("httpValidaCpf")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                log.LogInformation("=== Iniciando validação de CPF ===");
                log.LogInformation($"Método: {req.Method}");
                log.LogInformation($"Content-Type: {req.ContentType}");

                string cpf = "";

                // Tenta pegar da query string primeiro (GET ou POST)
                cpf = req.Query["cpf"];
                log.LogInformation($"CPF da query string: [{cpf}]");

                // Se não veio na query string e é POST, lê do body
                if (string.IsNullOrWhiteSpace(cpf) && req.Method == "POST")
                {
                    string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                    log.LogInformation($"Body recebido (raw): [{requestBody}]");
                    log.LogInformation($"Tamanho do body: {requestBody.Length}");

                    if (!string.IsNullOrWhiteSpace(requestBody))
                    {
                        // Tenta primeiro como JSON
                        try
                        {
                            var data = JsonConvert.DeserializeObject<CpfRequest>(requestBody);
                            cpf = data?.Cpf ?? "";
                            log.LogInformation($"CPF extraído do JSON: [{cpf}]");
                        }
                        catch (Exception jsonEx)
                        {
                            // Se não for JSON válido, usa o texto puro
                            log.LogInformation($"Não é JSON válido, usando texto puro. Erro: {jsonEx.Message}");
                            cpf = requestBody.Trim();
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(cpf))
                {
                    log.LogWarning("CPF está vazio ou nulo");
                    return new BadRequestObjectResult(new CpfResponse
                    {
                        Valido = false,
                        Cpf = null,
                        Mensagem = "Nenhum CPF foi enviado. Envie via query string (?cpf=12345678900) ou no body como JSON {\"cpf\": \"12345678900\"} ou texto puro."
                    });
                }

                // Remove espaços, quebras de linha e tabs
                cpf = cpf.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "");
                log.LogInformation($"CPF após trim: [{cpf}]");

                // Remove qualquer caractere que não seja número
                string cpfLimpo = System.Text.RegularExpressions.Regex.Replace(cpf, @"[^\d]", "");
                log.LogInformation($"CPF limpo (só números): [{cpfLimpo}]");
                log.LogInformation($"Quantidade de dígitos: {cpfLimpo.Length}");

                // Valida se tem exatamente 11 dígitos
                if (cpfLimpo.Length != 11)
                {
                    log.LogWarning($"CPF não possui 11 dígitos. Possui: {cpfLimpo.Length}");
                    return new BadRequestObjectResult(new CpfResponse
                    {
                        Valido = false,
                        Cpf = cpf,
                        Mensagem = $"CPF deve conter exatamente 11 números. Você enviou {cpfLimpo.Length} dígitos."
                    });
                }

                string cpfFormatado = FormatarCPF(cpfLimpo);
                log.LogInformation($"CPF formatado: {cpfFormatado}");

                bool cpfValido = ValidaCPF(cpfLimpo);
                log.LogInformation($"Resultado da validação: {cpfValido}");

                if (!cpfValido)
                {
                    log.LogInformation($"CPF inválido: {cpf}");
                    return new BadRequestObjectResult(new CpfResponse
                    {
                        Valido = false,
                        Cpf = cpfFormatado,
                        Mensagem = "CPF inválido. Verifique os dígitos informados e tente novamente."
                    });
                }

                log.LogInformation($"=== CPF válido: {cpfFormatado} ===");
                return new OkObjectResult(new CpfResponse
                {
                    Valido = true,
                    Cpf = cpfFormatado,
                    Mensagem = "CPF válido!"
                });
            }
            catch (Exception ex)
            {
                log.LogError($"ERRO EXCEPTION: {ex.Message}");
                log.LogError($"Stack trace: {ex.StackTrace}");
                return new ObjectResult(new CpfResponse
                {
                    Valido = false,
                    Cpf = null,
                    Mensagem = $"Erro ao processar a solicitação: {ex.Message}"
                }) { StatusCode = 500 };
            }
        }

        public static string FormatarCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11)
                return cpf;

            return string.Format("{0}.{1}.{2}-{3}", 
                cpf.Substring(0, 3), 
                cpf.Substring(3, 3), 
                cpf.Substring(6, 3), 
                cpf.Substring(9, 2));
        }

        public static bool ValidaCPF(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return false;

            cpf = cpf.Replace(".", "").Replace("-", "").Replace(" ", "");
            
            if (cpf.Length != 11 || !System.Text.RegularExpressions.Regex.IsMatch(cpf, @"^\d{11}$"))
                return false;

            // Verifica se todos os dígitos são iguais
            bool todosIguais = true;
            for (int i = 1; i < cpf.Length; i++)
            {
                if (cpf[i] != cpf[0])
                {
                    todosIguais = false;
                    break;
                }
            }
            if (todosIguais)
                return false;

            // Calcula primeiro dígito verificador
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            
            int soma = 0;
            for (int i = 0; i < 9; i++)
            {
                soma += (cpf[i] - '0') * multiplicador1[i];
            }

            int resto = soma % 11;
            int digito1 = resto < 2 ? 0 : 11 - resto;
            
            // Calcula segundo dígito verificador
            soma = 0;
            for (int i = 0; i < 10; i++)
            {
                soma += (cpf[i] - '0') * multiplicador2[i];
            }
            
            resto = soma % 11;
            int digito2 = resto < 2 ? 0 : 11 - resto;
            
            return cpf[9] - '0' == digito1 && cpf[10] - '0' == digito2;
        }
    }
}