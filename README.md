# Azure Test – HTTP Trigger Function (.NET)

Projeto de **Azure Functions** desenvolvido em **C# (.NET)** com **HTTP Trigger**, utilizado para testes de lógica de negócio, validação de dados e estudo de deploy no Azure.

## Visão Geral

Este projeto implementa uma Azure Function exposta via HTTP, permitindo a execução de lógica serverless sob demanda.  
O objetivo principal é validar conceitos como:
- Execução de funções serverless
- Estrutura de projetos Azure Functions
- Testes locais e empacotamento para deploy
- Integração com o ecossistema Azure

## Stack Tecnológica

- **Linguagem:** C#
- **Runtime:** .NET 6+  
- **Plataforma:** Azure Functions  
- **Trigger:** HTTP Trigger  
- **Ferramentas:**  
  - Azure Functions Core Tools  
  - Azure CLI

  
azure-test/

├── httpValidaCpf.cs # Azure Function (HTTP Trigger)

├── httpValidaCpf.csproj # Configuração do projeto .NET

├── host.json # Configuração global da Function App

├── local.settings.json # Configurações locais (não versionar secrets)

├── deploy.zip # Pacote para deploy manual

├── .gitignore

└── README.md

## ⚙️ Funcionamento da Function

- A função é acionada via **requisição HTTP**
- Recebe parâmetros via **query string ou body**
- Executa lógica de validação (ex: CPF)
- Retorna resposta HTTP (`200`, `400`, etc.) conforme o resultado

Fluxo simplificado:

Client → HTTP Request → Azure Function → Lógica de validação → HTTP Response

bash
Copiar código

## ▶️ Execução Local

### Pré-requisitos

- .NET SDK 6.0 ou superior  
- Azure Functions Core Tools  
- Node.js (dependência do Core Tools)

### Passos

```bash
git clone https://github.com/jhonnn-ny/azure-ValidadorCPF-DIO.git
cd azure-test
dotnet build
func start
A função ficará disponível em:

bash
Copiar código
http://localhost:7071/api/httpValidaCpf
Exemplo de teste com curl:

bash
Copiar código
curl "http://localhost:7071/api/httpValidaCpf?cpf=12345678909"
☁️ Deploy no Azure
Deploy via Azure Functions Core Tools
bash
Copiar código
func azure functionapp publish <NOME_DA_FUNCTION_APP>
