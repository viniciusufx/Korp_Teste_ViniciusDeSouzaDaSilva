# Back -> korp-services

## Preparação do ambiente
- Abra o terminal do Windows com Powershell.
    - `winget install Microsoft.DotNet.SDK.10`

Tecnologias: `.NET 10 SDK` `ASP.NET Core Web API` `Swagger`

Packages: `Microsoft.AspNetCore.OpenApi 10.0.3` `MongoDB.Driver 3.7.1` `Swashbuckle.AspNetCore 10.1.7` `Swashbuckle.AspNetCore.Annotations 10.1.7`


# Front -> korp-invoicing-frontend

Tecnologias: `Node.js 24.14.1` `npm 11.12.1` `Angular 21.2.7`

Packages: `@angular/build 21.2.6` `@angular/cli 21.2.6` `@angular/common 21.2.7` `@angular/compiler 21.2.7` `@angular/compiler-cli 21.2.7` ` @angular/core 21.2.7` `@angular/forms 21.2.7` `@angular/platform-browser 21.2.7` `@angular/router 21.2.7` `rxjs 7.8.2` `typescript 5.9.3` `vitest 4.1.3`

## Preparação do ambiente
- Abra o terminal do Windows com Powershell.
- Instale o NVM for Windows, gerenciador de versões do Node.js.
    - `winget install CoreyButler.NVMforWindows`
- Feche o terminal e abra-o novamente.
- Baixe e instale a versão 24.14.1 do Node.js.
    - `nvm install 24.14.1`
- Defina a versão 24.14.1 como ativa no ambiente atual.
    - `nvm use 24.14.1`
- O PowerShell bloqueia execução de scripts por padrão. Execute este comando uma vez no PowerShell
    - `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`
- Instale o Angular CLI globalmente:
    - `npm install -g @angular/cli`

## Como executar
- Rode o Frontend
    - `ng serve`