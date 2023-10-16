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

## Banco de Dados

O banco de dados utilizado na aplicação foi o SQLite. Ele é amplamente conhecido por sua confiabilidade, eficiência e facilidade de uso. Costuma ser utilizado em aplicações que precisam utilizar um banco de dados local. 

## Funcionamento


Nesta tela principal, o usuário tem a opção de escolher: Registrar uma nova ordem de produção, Listar ordens existentes e Alterar seu estado, Cadastrar novos produtos , Gerenciar a quantidade de seus estoques e Sair da aplicação.

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/efabd923-275a-4b19-9759-b6a695e61445")

Ao escolher a opção Registrar Ordem, o usuário deve inserir o código do Produto, a quantidade que irá ser utilizada no processo e a data estipulada para a entrega.

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/d58e9f75-b556-4e9b-a725-a52b9e7d1e1c")

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/0399b7ee-83bf-46ee-aca3-1d0139f9c611")

A aplicação permite listar Ordens já existentes para realizar acompanhamento e Gerar relatórios, são separadas em dois estados: Em andamento e Concluídas; Além disso, é possível gerar um relatório em PDF com os dados.

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/0c0dfc8c-dcfc-4d82-a700-70e26b18e4ff)

Listando ordens em andamento; Ao pressionar a tecla ENTER, podemos ver com mais detalhes as informações da ordem e também é possível alterar o seu estado.

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/076c781a-c0e1-4f3a-8cf2-75efc09900b9)

Temos a opção de voltar pressionando ESC ou alterar o estado pressionando ENTER:

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/3ee1dee4-e184-4fe1-ab13-733617e7963d)

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/bcad3f2f-3826-4a99-9e21-ce306573352a)

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/21d89272-74a1-4e98-9f13-f2db8ec51c98)

Agora a ordem se encontra no estado Concluído.

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/df2bbc7d-3501-4cd0-8fd6-0a29a1bfe310)

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/b7b97e70-1fa7-415e-83fd-8ea234f0500e)

Como dito anteriormente, também é possível gerar um relatório PDF das ordens:

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/5fc11f82-24b0-4c6e-bff0-eafc7d086a37)

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/4382efbf-5404-4226-a9cc-7009714c379b)

Voltando para o Menu Principal, temos a opção de Cadastrar Produto; É solicitado o nome e a quantidade inicial de materiais para fabricação do produto.

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/e17552e2-5d19-4221-91c7-d2d0d8008917)

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/4af45538-9fe8-4a7f-b7cd-bef89e4c27f1)

Também podemos Gerenciar o Produto, caso chegue mais materiais para fabricação.

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/4437f136-eecf-4f4d-8cd6-832660210d4f)

Assim quando for realizada uma nova ordem, a quantidade de materiais serão suficientes:

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/8d311b64-ea4e-406f-887b-d0f3b395b9ed)

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
















