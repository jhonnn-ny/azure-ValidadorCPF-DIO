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
