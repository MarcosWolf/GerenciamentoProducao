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


Nesta tela principal, o usuário tem a opção de escolher: Registrar uma nova ordem de produção, Listar ordens existentes e Alterar seu estado, Cadastrar novos produtos e Gerenciar a quantidade de seus estoques.

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

Agora a ordem se encontra no estado Concluído

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/df2bbc7d-3501-4cd0-8fd6-0a29a1bfe310)

![image](https://github.com/MarcosWolf/GerenciamentoProducao/assets/26293082/b7b97e70-1fa7-415e-83fd-8ea234f0500e)











