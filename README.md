# [Backend] ATY - Plataforma de Dados Meteorológicos

**Projetos Relacionados:**
-  [aty-frontend](https://github.com/aty-plataforma-dados-meteorologicos/aty-frontend): frontend da plataforma de dados meteorológicos.
- [aty-mobile](https://github.com/aty-plataforma-dados-meteorologicos/aty-mobile): aplicativo mobile da plataforma de gerenciamento de estações meteorológicas

## Introdução

A plataforma de dados meteorológicos consiste em um sistema web, em que os usuários, pessoas e instituições públicas ou privadas, podem se cadastrar para compartilhar ou consumir dados meteorológicos.

Os colaboradores, aqueles que desejam enviar dados meteorológicos para a plataforma, registram suas estações meteorológicas, informando nome, localização, instituições parceiras, se a estação e os dados serão públicos e principalmente, selecionar os tipos de dados que serão enviados para o sistema com base nos sensores que a estação meteorológica possui. Uma estação pode ter um ou mais mantenedores e cada mantenedor, que também é um usuário que faz uso de dados da plataforma, está responsável por nenhuma ou várias estações.

Dentre os dados que o mantenedor pode selecionar para que a estação envie, então os de sensores de temperatura, umidade, pressão, radiação solar direta, radiação solar global, direção do vento, pluviômetro e anemômetro, com a possibilidade de expansão mediante solicitação.

Após a conclusão do cadastro de uma estação meteorológica, é gerado um token que é necessário para o envio dos dados, esse envio pode ocorrer de maneira automatizada, cabendo o usuário realizar um script para inserção dos dados, ou manual utilizando a documentação da API.

No backend foi a implementação do endpoint que recebe os dados das estações meteorológicas e os identifica por meio de um token unico para casa ussuário. Os dados são tratados e inseridos em um banco de dados de séries temporais. Através da API REST esses dados são disponibilizados para aplicações, com essa mesma API, também é realizado a comuniicação feita a comunicação do frontend e mobile.

Essa aplicação possui dois bancos de dados, o primeiro é SQL, e neste estão armazenados os cadastros de usuários, de estações meteorológicas, parceiros, status, etc. No segundo, um banco NoSQL para séries temporais, no qual estão as informações meteorológicas.

Ao acessar o sistema, um usuário mantenedor, ve uma tela onde estão em uma tabela as suas estações que administra e que ele favoritou no sistema para consumir dados. Para todas essas estações são exibidas na tabela alguns parâmetros como status de envio de dados (ativa ou inativa).

Ao acessar a página de uma estação, é exibido às dashboards com os dados recebidos durante o dia corrente, juntamente com as informações de status e de acesso aos dados por API. Se tem também uma seção de dados filtrados por parâmetros específicos.

## Tecnologias

O backend do projeto foi desenvolvido utilizando a stack [.NET Core](https://learn.microsoft.com/pt-br/dotnet/core/introduction) da Microsoft, tendo o [C#](https://learn.microsoft.com/pt-br/dotnet/csharp/tour-of-csharp/) como linguagem de programação, com o banco de dados [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) e o banco de dados NoSQL [InfluxDB](https://www.influxdata.com/products/influxdb-overview/). A API será desenvolvida utilizando o padrão [REST](https://www.redhat.com/pt-br/topics/api/what-is-a-rest-api).

## O Repositório

Neste repositório está o código fonte do backend da plataforma de dados meteorológicos. O código fonte do frontend pode ser encontrado em [aty-frontend](https://github.com/aty-plataforma-dados-meteorologicos/aty-frontend), e o código fonte para o mobile pode ser encontrado em [aty-mobile](https://github.com/aty-plataforma-dados-meteorologicos/aty-mobile).

## Executar o projeto

Para executar o projeto é necessário baixar a instalar a [IDE Visual Studio](https://visualstudio.microsoft.com/pt-br/downloads/), o [.NET Core 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) e instalar o banco de dados [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads). Para o banco de dados NoSQL, é necessário baixar e instalar o [InfluxDB](https://www.influxdata.com/products/influxdb-overview/), recomendamos usar o Docker para executar instâncias com os banco de dados, para ambos tem imagem disponível no [Docker Hub](https://hub.docker.com/).

Após executar os bancos de dados (SQL Server e InfluxDB) é necessário configurar as respectivas variáveis de ambiente no arquivo `appsettings.json` do projeto `src/AtyBackend.API`, para o SQL Server é necessário configurar a string de conexão, e para o InfluxDB é necessário configurar o endereço do servidor, porta, usuário, senha e bucket na repository `InfluxDbWeatherDataRepository` em `src/AtyBackend.Infrastructure/Repositories`.

Dentro de `src/AtyBackend.API`, no arquivo `appsettings.json` também estão as configurações de envio de e-mail, geração de token JWT e demais parâmetros do sistema, que devem ser configurados de acordo com o ambiente de execução. Após configurar o ambiente, dentro da pasta `src/AtyBackend.API`, execute o comando `dotnet run` para executar o projeto.

## Equipe e contato

Esse projeto foi desenvolvido por Luiz Fernando Freitas Silva$^1$, Romildo C Marques$^2$ e Victor Gabriel F Ferrari$^3$, alunos de Tecnologia em Análise e Desenvolvimento de Sistemas ([IFPR - Foz](https://ifpr.edu.br/foz-do-iguacu/superior/tecnologia-em-analise-e-desenvolvimento-de-sistemas-superior/)), sob orientação dos professore Daniel Di Domenico$^4$, Felippe Alex Scheidt$^5$, Juliana Hoffmann Quiñónez Benacchio$^6$ e professora Marcela Turim Koschevic$^7$.

- $^1$ luizfscontato@outlook.com;
- $^3$ romildodcm@gmail.com;
- $^3$ victorfonsecaferrari@gmail.com;
- $^4$ daniel.domenico@ifpr.edu.br;
- $^5$ felippe.scheidt@ifpr.edu.br;
- $^6$ juliana.benacchio@ifpr.edu.br;
- $^7$ marcela.turim@ifpr.edu.br.

