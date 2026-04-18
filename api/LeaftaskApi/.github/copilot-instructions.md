# Copilot Instructions

## Project Guidelines
- En este proyecto ya existía `UserReadModel` en el módulo de organizaciones (dominio y configuración), por lo que no se debe duplicar en infraestructura.
- El usuario prefiere un enfoque genérico para consumo de eventos de integración: un handler base en BuildingBlocks que gestione idempotencia vía Inbox y registro final, delegando la lógica de negocio al módulo (capa Application).
- El usuario prefiere que los contratos de repositorio se ubiquen en la capa de dominio, no en la capa de aplicación.
- El usuario quiere paginación cursor-based basada en el orden real de la consulta (por ejemplo, created_at), no en comparar Id mayores. El nextCursor debe derivarse del último registro de la página según ese orden.
- Cuando el cursor sea un Guid, se tipa como `Guid?` desde la request para evitar parseos manuales.
- En consultas EF, se debe usar `ToListAsync()` en lugar de `ToList()`.

## Controller Guidelines
- Los controladores deben mantenerse delgados: deben crear una consulta/comando, enviarlo a través de MediatR y permitir que un QueryHandler/CommandHandler delegue a un servicio. 
- Los DTOs de respuesta deben seguir las convenciones de respuesta/consulta del módulo existente en lugar de incluir lógica de acceso a datos o EF en el controlador.