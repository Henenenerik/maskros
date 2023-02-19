# Maskros

## TODO:

### Stridsregler

En enhet som går in i en eller flera fientliga stridszoner **måste** slåss mot samtliga fiender.

Flera fiendeenheter **kan** bekämpas samtidigt i en strid så länge de angränsar varandra.

Flera anfallsenheter **kan** delta tillsammans i en strid mot samma mål.

En anfallande enhet kan **endast** delta i en strid per tur.

Anfallaren väljer ordning på striderna, samt vilka enheter som deltar i vilken strid.

### Lösningsförslag

1. När ingen anfallare är markerad visas vilka enheter som **måste** slåss denna tur, samt vilka enheter som **får** slåss.
2. Spelaren väljer en enhet att utgå från.
3. GUIt beräknar vilka fiendeenheter som kan delta i striden och highlightar dessa.
4. Spelaren får möjlighet att lägga till/ta bort deltagande enheter. 
5. Ifall inga enheter är kvar, gå till steg 1.
6. Spelaren trycker på utför strid knappen för att genomföra striden. 
