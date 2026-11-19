# Uso de IA como copiloto — principais prompts

A IA generativa foi usada como **copiloto de desenvolvimento**. Todo código gerado foi lido, ajustado e validado pela equipe (compilação, `dotnet test` e testes manuais no Swagger). A regra de risco e o contrato REST vêm da Ficha da CitizenTech, definida pela equipe e compartilhada entre Mobile, SOA, C# e QA.

## Prompts principais

1. **Arquitetura**
   > "A partir desta Ficha da CitizenTech (modelo de domínio, regra de risco e contrato REST), proponha uma solução .NET 8 em camadas (Domain, Application, Infrastructure, Api, Tests) com EF Core + SQLite, mantendo nomes em português iguais aos da API FastAPI."

2. **Domínio e POO**
   > "Crie as entidades Corrego, Sensor, Leitura, Morador e Alerta com encapsulamento (setters privados, construtores que validam invariantes), uma classe base abstrata EntidadeBase, a regra de risco como Strategy (IClassificadorRisco) e um exemplo de polimorfismo para as notificações dos moradores prioritários."

3. **EF Core e LINQ**
   > "Escreva o AppDbContext com uma configuração Fluent API por entidade, relacionamentos 1‑N, enums salvos como texto e seed com HasData. Nos repositórios, use projeções Select para DTO e um GroupBy por nível de risco para as estatísticas das últimas 24 h."

4. **Web API**
   > "Implemente os controllers seguindo exatamente o contrato da Ficha, com actions assíncronas, CancellationToken, ProducesResponseType, CreatedAtAction, ValidationProblem e um tratador global de exceções que converta exceções de domínio em ProblemDetails. JSON em camelCase e enums como NORMAL/ATENCAO/ALERTA/CRITICO."

5. **Testes**
   > "Crie testes xUnit com [Theory]/[InlineData] cobrindo todos os limites da regra de risco, testes das invariantes das entidades e testes do LeituraService com Moq (leitura crítica gera alerta e lista prioritários; leitura normal não gera)."

6. **Documentação**
   > "Gere um README em português com a Ficha, descrição, diagrama mermaid das camadas, tabela de conceitos × critérios de avaliação, instruções para rodar e testar, tabela de endpoints e exemplos."

## O que a equipe revisou/ajustou

- Regra de risco conferida com a Ficha e com a implementação da API SOA (mesmos limites e mesmos nomes de enum).
- Garantia de que **nenhum dado pessoal de morador** aparece em logs ou mensagens de erro.
- Dados de seed 100% fictícios (sem CPF, telefones `11 90000-000X`).
- Testes executados localmente e evidências registradas em `docs/evidencias/`.
