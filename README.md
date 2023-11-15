# [Backend] ATY - Plataforma de Dados Meteorológicos

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

## Equipe e contato

Esse projeto foi sendo desenvolvido por Romildo C Marques$^1$ e Victor Gabriel F Ferrari$^2$, alunos de Tecnologia em Análise e Desenvolvimento de Sistemas ([IFPR - Foz](https://ifpr.edu.br/foz-do-iguacu/superior/tecnologia-em-analise-e-desenvolvimento-de-sistemas-superior/)), sob orientação do professor Daniel Di Domenico$^3$ e da professora Marcela Turim Koschevic$^4$.

$^1$romildodcm@gmail.com
$^2$victorfonsecaferrari@gmail.com 
$^3$daniel.domenico@ifpr.edu.br
$^4$marcela.turim@ifpr.edu.br

