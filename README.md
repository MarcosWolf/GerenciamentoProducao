# Gerenciamento de Ordens de Produção

Um sistema de gestão que permite aos usuários registrar novas ordens, listar as ordens existentes e alterar o estado de andamento das mesmas, cadastrar novos produtos e gerenciar o estoque. Ferramenta desenvolvida utilizando C# e o banco de dados SQLite para armazenar e recuperar informações sobre as ordens e produtos.

### Tecnologias Utilizadas

* [C#](https://dotnet.microsoft.com/pt-br/languages/csharp)
* [SQLite](https://www.sqlite.org/)

### Dependências

A aplicação requer o .NET Framework para executar corretamente. Certifique-se de que o .NET Framework esteja atualizado em seu sistema. Você pode baixar a versão mais recente do .NET Framework no site oficial da Microsoft.

https://dotnet.microsoft.com/pt-br/download/dotnet-framework


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

## Banco de Dados

O banco de dados utilizado na aplicação foi o SQLite. Ele é amplamente conhecido por sua confiabilidade, eficiência e facilidade de uso. Costuma ser utilizado em aplicações que precisam utilizar um banco de dados local. 

[![](https://mermaid.ink/img/pako:eNp9Uc2KAjEMfpWS6-oLlD16lV3wWljCJLrFmdRNU2HQeXerHZQB2V7afj_J1_QCXSIGD12POW8iHhSHIK6ub01UOnOf1_XafSmxLnDvPqKYO7XbT6Slq9I70yiHp0Jw4P8q_BUUizYGaapHotb40hDXDOkOPRu69zkW2lfpmcotWmOJ-3hmHTdo_MacDa3kRkxBYAUD64CR6tQewQLYL9e3ga9HQj0GCDJVHRZLu1E68KaFV1BOVFvMQwa_xz5XlCla0u38DfdtugEzbYhP?type=png)](https://mermaid.live/edit#pako:eNp9Uc2KAjEMfpWS6-oLlD16lV3wWljCJLrFmdRNU2HQeXerHZQB2V7afj_J1_QCXSIGD12POW8iHhSHIK6ub01UOnOf1_XafSmxLnDvPqKYO7XbT6Slq9I70yiHp0Jw4P8q_BUUizYGaapHotb40hDXDOkOPRu69zkW2lfpmcotWmOJ-3hmHTdo_MacDa3kRkxBYAUD64CR6tQewQLYL9e3ga9HQj0GCDJVHRZLu1E68KaFV1BOVFvMQwa_xz5XlCla0u38DfdtugEzbYhP)

## Funcionalidades

- [X] Registrar Ordem de Fabricação
- [X] Lista Interativa com ordens existentes
- [X] Geração de Relatórios em PDF
- [X] Alteração do estado de processo da ordem
- [X] Cadastro de novos produtos
- [X] Gerenciamento de produtos
- [X] Tratamento de exceções e validações de erros

## Funcionamento


Nesta tela principal, o usuário tem a opção de escolher: Registrar uma nova ordem de produção, Listar ordens existentes e Alterar seu estado, Cadastrar novos produtos , Gerenciar a quantidade de seus estoques e Sair da aplicação.

![1](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/75fc48aa-cd72-459c-ad5a-4e87f36be21b)

Ao escolher a opção Registrar Ordem, o usuário deve inserir o código do Produto, a quantidade que irá ser utilizada no processo e a data estipulada para a entrega.

![2](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/0ee8f9c3-c566-431b-b5bf-2ab91a1df40a)

![3](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/14635a76-71d3-4ff7-97bf-268289bc7398)

A aplicação permite listar Ordens já existentes para realizar acompanhamento e Gerar relatórios, são separadas em dois estados: Em andamento e Concluídas; Além disso, é possível gerar um relatório em PDF com os dados.

![4](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/70505338-5963-45c3-b165-035af715f7e8)

Listando ordens em andamento; Ao pressionar a tecla ENTER, podemos ver com mais detalhes as informações da ordem e também é possível alterar o seu estado.

![5](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/13703414-0bd5-4e0e-93bc-7e2edb7e5efb)

Temos a opção de voltar pressionando ESC ou alterar o estado pressionando ENTER:

![6](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/df2d9fad-9865-498e-a2ee-8350d29723a5)

![7](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/040cd7c7-a278-47ee-9625-e96f386b46f8)

![8](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/585b6559-90ca-4a6e-aa9f-c37c7e77ced9)

Agora a ordem se encontra no estado Concluído.

![9](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/df06488d-173b-4e81-bc68-5a902ffbc3e3)

![10](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/ff7a6bc3-60f1-4140-a3c0-4e2c1fb8d4f3)

Como dito anteriormente, também é possível gerar um relatório PDF das ordens:

![11](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/66368e48-0e02-435b-a446-7df62f797ffb)

![12](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/955248d0-e82d-43e2-b983-d878f24168b1)

Voltando para o Menu Principal, temos a opção de Cadastrar Produto; É solicitado o nome e a quantidade inicial de materiais para fabricação do produto.

![13](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/47640a52-68ff-4243-80ca-313c85d124d9)

![14](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/4a695d06-43b9-4fce-b38e-d372a2ba3f02)

Também podemos Gerenciar o Produto, caso chegue mais materiais para fabricação.

![15](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/de26910c-9d9e-4327-9a26-9701b04d4dd8)

Assim quando for realizada uma nova ordem, a quantidade de materiais serão suficientes:

![16](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/9c91cd1c-1d10-4f33-b025-5d2aa3d1fc20)

##  Autor

   <a href="https://www.marcoswolf.com.br/">
    <img style="width:100px" src="https://avatars.githubusercontent.com/u/26293082?v=4" alt=""/>
    <br />    
   </a>
   Marcos Vinícios
   
   <div>
   	<a href="mailto:contato@marcoswolf.com.br"><img src="https://img.shields.io/badge/Gmail-D14836?style=for-the-badge&logo=gmail&logoColor=white"/></a>
   	<a href="https://www.linkedin.com/in/marcoswolf/" target="_blank" rel="noopener noreferrer"><img src="https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white"/></a>
   </div>
</div>
















