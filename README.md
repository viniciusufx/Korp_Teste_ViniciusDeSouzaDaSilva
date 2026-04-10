# Korp_Teste_Vinicius de Souza da Silva

## Pré-requisitos gerais
Antes de iniciar, certifique-se de ter instalado:
- Windows PowerShell (recomendado executar como Administrador)

# Backend -> korp-services

## Tecnologias utilizadas
- `.NET 10 SDK`
- `ASP.NET Core Web API`
- `Swagger`

## Packages principais
- `Microsoft.AspNetCore.OpenApi 10.0.3`
- `MongoDB.Driver 3.7.1`
- `Swashbuckle.AspNetCore 10.1.7`
- `Swashbuckle.AspNetCore.Annotations 10.1.7`

## Preparação do ambiente
- Abra o PowerShell como administrador e execute:
    - `winget install Microsoft.DotNet.SDK.10`
    - `winget install MongoDB.Server`
> ⚠️ Após a instalação, feche e reabra o terminal para garantir que o .NET seja reconhecido.
## Execução do Backend
- Acesse o diretório de um service
    - `cd korp-services/korp-billing-service`
- Restaurar dependências
    - `dotnet restore`
- Compilar o projeto
    - `dotnet build`
- Executar o serviço
    - `dotnet run`

## Importante
- Repita os mesmos passos para todos os serviços dentro de:
    - `korp-services/korp-Stock-service`

## Resultado esperado
- korp-billing-service rodando em(Swagger disponível)
    - [https://localhost:5001](https://localhost:5001)
    - [http://localhost:5000](http://localhost:5000)
- korp-Stock-service rodando em(Swagger disponível)
    - [https://localhost:5003](https://localhost:5003)
    - [http://localhost:5002](http://localhost:5002)

# Frontend -> korp-invoicing-frontend

## Tecnologias utilizadas:
- `Node.js 24.14.1`
- `npm 11.12.1`
- `Angular 21.2.7`

## Packages principais: 
- `@angular/animations 21.2.7`
- `@angular/build 21.2.7`
- `@angular/cli 21.2.7`
- `@angular/common 21.2.7`
- `@angular/compiler 21.2.7`
- `@angular/compiler-cli 21.2.7`
- `@angular/core 21.2.7`
- `@angular/forms 21.2.7`
- `@angular/platform-browser 21.2.7`
- `@angular/router 21.2.7`
- `rxjs 7.8.2`
- `typescript 5.9.2`
- `vitest 4.1.4`
- `bootstrap 5.3.8`
- `ngx-toastr 20.0.5`

## Preparação do ambiente
- Abra o terminal do Windows com Powershell.
- Instale o NVM for Windows, gerenciador de versões do Node.js.
    - `winget install CoreyButler.NVMforWindows`
- Reiniciar o terminal. Feche e abra novamente o PowerShell.
- Baixe e instale a versão 24.14.1 do Node.js.
    - `nvm install 24.14.1`
- Defina a versão 24.14.1 como ativa no ambiente atual.
    - `nvm use 24.14.1`
- O PowerShell bloqueia execução de scripts por padrão. Execute este comando uma vez no PowerShell
    - `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`
- Instale o Angular CLI globalmente:
    - `npm install -g @angular/cli`

## Execução do Frontend
- Dentro da pasta do frontend:
    - `korp-invoicing-frontend`
- Instale as dependencias do projeto
    - `npm install`
- Rode o Frontend
    - `ng serve`

## Resultado esperado
- korp-invoicing-frontend rodando em
    - [http://localhost:4200/](http://localhost:4200/)