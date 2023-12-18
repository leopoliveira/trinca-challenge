# Notas do Autor
Primeiramente, gostaria de agradecer a oportunidade e pelo enorme conhecimento adquirido com a execução e participação desse desafio.

Aprendi práticas, formas de uso e implementação de tecnologias que eu não havia tido contato prático.

Independente do resultado, levo essa prática como uma nova forma de resolver problemas e encantar os clientes.

Obrigado!

# Desafio Técnico Trinca - Informações Gerais Trinca

Na [wiki da repositório original](https://github.com/trinca137/trinca-challenge/wiki/Comece-por-aqui) estão as informações relevantes para realizar o desafio. 

# Desafio Técnico Trinca - Implementações

## Visão Geral

Este projeto é para gerenciar churrascos entre funcionários da Trinca.
O projeto é feito utilizando os conceitos de API Serverless, voltado para [Azure Functions](https://azure.microsoft.com/en-us/products/functions). 

## Atividades

### 1. **Criar novo churrasco**
- **URL:** `http://localhost:7296/api/churras`
- **Verbo HTTP:** POST
  
- **Implementação:**
  - Houve a refatoração do código para que a interface "IEventStore" fosse acessível apenas dentro de seu assembly;
  - Houve refatoração para deixar o código da Function responsável por esse endpoint, padronizado com o restante da aplicação.
  
### 2. Moderar churrasco
- **URL:** `http://localhost:7296/api/churras/{churras-id}/moderar`
- **Verbo HTTP:** PUT
  
- **Implementação:**
  - Ajustes para que apenas os moderadores recebessem o convite após a criação de um churrasco;
  - Os convites passaram a ser enviados para os demais funcionários apenas quando um dos moderadores aceitasse o novo churrasco;
  - Moderadores não recebem mais o convite duplicado após a aprovação do novo churrasco;
  - Quando o novo churrasco é rejeitado, todos os convites pendentes são rejeitados.

### 3. Listar churrascos
- **URL:** `http://localhost:7296/api/churras`
- **Verbo HTTP:** GET
  
- **Implementação:**
  - Os churrascos rejeitados pelos moderadores não aparecem mais na lista de churrascos.

### 4. Listar convites
- **URL:** `http://localhost:7296/api/person/invites`
- **Verbo HTTP:** GET
  
- **Implementação:**
  - Não foram necessários ajustes.

### 5. Aceitar convite
- **URL:** `http://localhost:7296/api/person/invites/{inviteId}/accept`
- **Verbo HTTP:** PUT
  
- **Implementação:**
  - Implementação da lista de compras para cada churrasco;
  - Implementação da contagem de pessoas confirmadas para o churrasco;
  - Ao aceitar o convite, seu status passará a ser de "Confirmado" para a pessoa;
  - A cada convite aceito, um evento para o churrasco relacionado é disparado:
    - Esse evento incrementa a lista de contagem de pessoas confirmadas para o churrasco;
    - Esse evento incrementa a lista de compras do churrasco se baseando na opção da pessoa que confirmou o evento (vegetariana ou não);
  - Quando a quantidade de pessoas que aceitaram o convite for maior ou igual a 7, o status do churrasco automaticamente irá ser alterado para "Confirmado".


### 6. Rejeitar convite
- **URL:** `http://localhost:7296/api/person/invites/{inviteId}/decline`
- **Verbo HTTP:** PUT
  
- **Implementação:**
  - Ao rejeitar o convite, seu status passará a ser "Rejeitado" para a pessoa que o rejeitou e ele irá desaparecer de sua lista de convites;
  - Um convite aceito anteriormente pode ser rejeitado;
    - Quando esse evento acontece, a lista de compras é reduziada proporcionalmente a opção escolhida anteriormente pela pessoa (vegetariana ou não, informada no momento do aceite);
  - Se a quantidade de pessoas que aceitaram o convite for menor do que 7, o status do churrasco automaticamente irá ser alterado para "Pendente de confirmações".

### 7. Lista de compras
- **URL:** `http://localhost:7296/api/churras/{churras-id}/shoppingList`
- **Verbo HTTP:** GET
  
- **Implementação:**
  - Endpoint acessível apenas por moderadores;
  - Ao utilizar o endpoint informado o id do churrasco, sua lista de compras será retornada, contendo:
    - "Summary": Um resumo da lista de compras;
    - "Meat": A quantidade (em kg) de carne da lista de compras até o momento;
    - "Veg": A quantidade (em kg) de vegetais da lista de compras até o momento;
    - As informações são retornadas em um JSON.
    - Ex.:
```json
{
    "Summary": "Actual Churras Shopping List: Meat: 0Kg, Vegetables: 0Kg.",
    "Meat": 0,
    "Veg": 0
}
``` 

### 8. Para ganhar alguns pontinhos
 
- **Implementação:**
  - Criada a camada de Serviços que é responsável por aplicar e executar as regras de negócio;
  - Feita a refatoração das Functions, para que, ao invés de executar a lógica de negócio, apenas fiquem responsáveis pela delegação de responsabilidade a camada de Serviços e retorno para o usuário;
  - Feito ajustes no encapsulamento de classes que não deveriam ser acessíveis na camada da API, como as entidades de domínio, executores de eventos, repositórios e etc;
  - Implementação de condicionais para verificação de erros e elementos nulos durante a execução do código;
  - Melhoria nas mensagens e códigos HTTP de retorno aos endpoints;
  - Segragação de responsabilidade, prezando pelo cumprimento do primeiro princípio SOLID "S" - Single Responsiblity Principle (Princípio da responsabilidade única);
  - Aplicação dos princípios "I" — Interface Segregation Principle (Princípio da Segregação da Interface) e "D" — Dependency Inversion Principle (Princípio da inversão da dependência);
  - Aplicação de algumas práticas do Clean Code, prezando pela clareza nos comentários, nomenclaturas de métodos e variáveis, regra do escoteiro, DRY (Don’t Repeat Yourself), tratamento de erros e boas práticas de escrita de código (como uso do var, por exemplo).

### Sugestões para implementações futuras

##### Sugestões de projeto
- Criação de testes de unidade;
- Criação de mais classes de validação das regras de negócio;
- Implementar autenticação de usuários;
- Separação da solução em mais projetos:
  - Projeto de infra para lidar com as conexões externas, como: e-mails, sms, banco de dados e etc.;
  - Projeto de aplicação para lidar apenas com o "meio de campo" entre o domínio e a camanda de apresentação;
  - Projeto para testes;
- Criação de logs de erros e auditoria na aplicação (aceites, rejeições, alterações e etc.);
- Utilizar algum software para lidar com a menssageria, como RabbitMQ ou Kafka;
- Utilizar o Azure Key Vault ou outro software semelhante para armazenar segredos e dados confidenciais;

##### Sugestões para aplicação
- Incluir opção de informar a quantidade de pessoas em cada convite;
- Incluir a opção de rateio aos interessados no churrasco para o caso da Trinca não arcar com as despesas;
    - Nesse ponto, permitir que o sistema receba o upload dos comprovantes de transferência, pagamentos e etc.;
- Notificações para as pessoas que estão confirmadas no churrasco;
- Lembrete para os moderadores sobre a compra dos itens da lista de compras do churras;
- Notificações diversas;
- Criar uma interface front-end.
