# Copilot Instructions

## Project Guidelines
- En este proyecto ya existía `UserReadModel` en el módulo de organizaciones (dominio y configuración), por lo que no se debe duplicar en infraestructura.
- El usuario prefiere un enfoque genérico para consumo de eventos de integración: un handler base en BuildingBlocks que gestione idempotencia vía Inbox y registro final, delegando la lógica de negocio al módulo (capa Application).
- El usuario prefiere que los contratos de repositorio se ubiquen en la capa de dominio, no en la capa de aplicación.