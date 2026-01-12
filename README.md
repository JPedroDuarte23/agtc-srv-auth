# 🔐 Agro.Auth.API (Identity Service)

Microsserviço responsável pela gestão de identidade e controle de acesso (IAM) da plataforma AgroSolutions.

## 📋 Responsabilidades
- Cadastro de Usuários (Produtores Rurais).
- Autenticação via **JWT (JSON Web Token)**.
- Gestão de Roles:
  - `Farmer`: Acesso total aos dashboards e gestão de propriedades.
  - `Device`: Token de longa duração para sensores (IoT) enviarem dados.

## 🛠️ Stack Tecnológica
- .NET 8 Web API
- MongoDB (Armazenamento de Users)
- ASP.NET Core Identity

## ⚙️ Configuração (appsettings.json)
```json
{
  "ConnectionStrings": {
    "MongoDb": "mongodb://localhost:27017"
  },
  "JwtSettings": {
    "Secret": "SUA_CHAVE_SECRETA_SUPER_SEGURA_DE_PELO_MENOS_32_CHARS",
    "ExpirationHours": 1,
    "Issuer": "AgroSolutions",
    "Audience": "AgroSolutions"
  }
}