# Gerenciamento de Ordens de Produção

Um sistema de gestão que permite aos usuários registrar novas ordens, listar as ordens existentes e alterar o estado de andamento das mesmas, cadastrar novos produtos e gerenciar o estoque. Ferramenta desenvolvida utilizando C# e o banco de dados SQLite para armazenar e recuperar informações sobre as ordens e produtos.

### Tecnologias Utilizadas

* [C#](https://dotnet.microsoft.com/pt-br/languages/csharp)
* [SQLite](https://www.sqlite.org/)

### Como rodar o projeto

Clone o repositório para o seu computador

```
git clone https://github.com/MarcosWolf/GerenciamentoProducao
```

Acesse a pasta

```
cd GerenciamentoProducao/bin/release/net6.0/
```

Execute o arquivo executável

```
GerenciamentoProducao.exe
```

## Funcionamento


Nesta tela principal, o usuário tem a opção de escolher: Registrar uma nova ordem de produção, Listar ordens existentes e Alterar seu estado, Cadastrar novos produtos e Gerenciar a quantidade de seus estoques.

<img src="https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/efabd923-275a-4b19-9759-b6a695e61445" width="70%" />


Ao escolher a opção Registrar Ordem, o usuário deve inserir o código do Produto, a quantidade que irá ser utilizada no processo e a data estipulada para a entrega.

<img src="https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/d58e9f75-b556-4e9b-a725-a52b9e7d1e1c" width="70%" />

<img src="https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/0399b7ee-83bf-46ee-aca3-1d0139f9c611" width="70%" />






## Banco de Dados

O banco de dados utilizado na aplicação foi o SQLite. Ele é amplamente conhecido por sua confiabilidade, eficiência e facilidade de uso. Costuma ser utilizado em aplicações que precisam utilizar um banco de dados local. 
