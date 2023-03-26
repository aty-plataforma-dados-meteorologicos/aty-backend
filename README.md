# [Backend] ATY - Plataforma de Dados Meteorológicos

## Introdução

A plataforma de dados meteorológicos consiste em um sistema web, em que os usuários, pessoas e instituições públicas ou privadas, poderão se cadastrar para compartilhar ou consumir dados meteorológicos.

Os colaboradores, aqueles que desejam enviar dados meteorológicos para a plataforma, registrarão suas estações meteorológicas, informando nome, localização, instituições parceiras, se a estação e os dados estarão públicos e principalmente, selecionar os tipos de dados que serão enviados para o sistema com base nos sensores que a estação meteorológica possui. Uma estação poderá ter um ou mais mantenedores e cada mantenedor, que também é um usuário que faz uso de dados da plataforma, estará responsável por nenhuma ou várias estações.

Dentre os dados que o mantenedor poderá selecionar para que a estação envie, então os de sensores de temperatura, umidade, pressão, radiação solar direta, radiação solar global, direção do vento, pluviômetro e anemômetro, com a possibilidade de expansão mediante solicitação.

Após a conclusão do cadastro de uma estação meteorológica, será gerado um token que é necessário para o envio dos dados, esse envio poderá ocorrer de maneira automatizada, onde a estação usa protocolo MQTT, ou de maneira manual, em que o usuário irá utilizar a interface web para a inserção dos dados.

No backend terá a implementação do endpoint com o broker MQTT que receberá os dados das estações meteorológicas e os identifica por meio de um token que será único para cada estação, esses dados serão tratados e inseridos em um banco de dados de séries temporais. Através de uma API REST esses dados poderão ser disponibilizados para aplicações, com essa mesma API, também será feita a comunicação do frontend.

Essa aplicação terá dois bancos de dados, o primeiro é SQL, e neste estarão armazenados os cadastros de usuários, de estações meteorológicas, parceiros, status, etc. No segundo, um banco NoSQL para séries temporais, no qual estarão as informações meteorológicas.

Ao acessar o sistema, um usuário mantenedor, verá uma tela onde estarão em uma tabela as suas estações que administra, em uma segunda tabela estão as estações que ele favoritou no sistema para consumir dados. Para todas essas estações serão exibidas na tabela alguns parâmetros como status de envio de dados (online ou offline), status (ativa ou inativa), usuários que não são mantenedores, verão apenas a segunda tabela.

Ao acessar a página de uma estação, será exibido às dashboards com os dados recebidos durante o dia corrente, juntamente com as informações de status e de acesso aos dados por API. Haverá também uma seção de dados filtrados por parâmetros específicos.

Uma tela de pesquisa apresentará a lista com as estações disponíveis e ativas. Haverá uma seção onde o usuário poderá fazer dois tipos de pesquisa. O primeiro tipo de pesquisa será pelo identificador único de cada estação e o segundo com filtro de estado e cidade. Estações públicas poderão ser acessadas e favoritadas por qualquer usuário. Nas estações privadas será necessário realizar uma solicitação e aguardar a aprovação dos mantenedores.

## Tecnologias

O backend do projeto será desenvolvido utilizando a stack [.NET Core](https://learn.microsoft.com/pt-br/dotnet/core/introduction) da Microsoft, tendo o [C#](https://learn.microsoft.com/pt-br/dotnet/csharp/tour-of-csharp/) como linguagem de programação, com o banco de dados [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) e o banco de dados NoSQL [InfluxDB](https://www.influxdata.com/products/influxdb-overview/). O sistema de mensageria será o [MQTT](https://mqtt.org/), que será utilizado para o envio de dados das estações meteorológicas para o backend. A API será desenvolvida utilizando o padrão [REST](https://www.redhat.com/pt-br/topics/api/what-is-a-rest-api).

## O Repositório

Neste repositório estará o código fonte do backend da plataforma de dados meteorológicos. O código fonte do frontend pode ser encontrado em [aty-frontend](https://github.com/aty-plataforma-dados-meteorologicos/aty-frontend). Ao longo do desenvolvimento, também será adicionado aqui a documentação do projeto, seja para implementar ou para usar.

## Executar o projeto

Para executar o projeto é necessário baixar a instalar a [IDE Visual Studio](https://visualstudio.microsoft.com/pt-br/downloads/), o [.NET Core 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) e instalar o banco de dados [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads). Para o banco de dados NoSQL, é necessário baixar e instalar o [InfluxDB](https://www.influxdata.com/products/influxdb-overview/), recomendamos usar o Docker para executar instâncias com os banco de dados, para ambos tem imagem disponível no [Docker Hub](https://hub.docker.com/).

## Equipe e contato

Esse projeto está sendo desenvolvido por Romildo C Marques$^1$ e Victor Gabriel F Ferrari$^2$, alunos de Tecnologia em Análise e Desenvolvimento de Sistemas ([IFPR - Foz](https://ifpr.edu.br/foz-do-iguacu/superior/tecnologia-em-analise-e-desenvolvimento-de-sistemas-superior/)), sob orientação do professor Daniel Di Domenico$^3$ e da professora Marcela Turim Koschevic$^4$.

$^1$romildodcm@gmail.com
$^2$victorfonsecaferrari@gmail.com 
$^3$daniel.domenico@ifpr.edu.br
$^4$marcela.turim@ifpr.edu.br

